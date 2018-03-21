namespace RtdsMonitorService
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.ServiceProcess;
  using Siemens.SimaticIT.OEE.Breads;
  using Siemens.SimaticIT.OEE.Breads.Types;
  using System.Data.SqlClient;
  using System.Threading;
  using System.Configuration;

  public partial class RtdsMonitor : ServiceBase
  {
    private Timer _rtdsTimerPoller;
    private readonly DTMCurrentStatus_BREAD _currentStatusBread = new DTMCurrentStatus_BREAD();
    private readonly Equipment_BREAD _equipmentBread = new Equipment_BREAD();
    private List<DTMCurrentStatus> _mostRecentStatuses;

    public RtdsMonitor()
    {
      ServiceName = "RtdsMonitorService";

      InitializeComponent();
    }

    /// <summary>
    /// Actions to be performed when service starts. 
    /// Pulls equipment from OEE and populates current status for each equipment.
    /// Sets timer tick rate and starts timer.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnStart(string[] args)
    {
      EventLog.WriteEntry("Pulling equipment from OEE and populating initial status", EventLogEntryType.Information);
      var currentEquipmentStatuses = _currentStatusBread.Select(null, -1, -1, "");
      _mostRecentStatuses = new List<DTMCurrentStatus>(currentEquipmentStatuses);

      foreach (var downtimeStatus in _mostRecentStatuses)
        InsertStatus(downtimeStatus);

      var tickTimeInSeconds = Int32.Parse(ConfigurationManager.AppSettings["tickTimeInSeconds"]);
      _rtdsTimerPoller = new Timer(RtdsTimerPoller_Tick, null, 0, tickTimeInSeconds * 1000);
    }

    /// <summary>
    /// Actions to perform when the timer ticks over.
    /// </summary>
    private void RtdsTimerPoller_Tick(object state)
    {
      PollRtdsEquipment();
    }

    /// <summary>
    /// Calls CompareLastStatus on each timeCategory to see if it changed.
    /// </summary>
    private void PollRtdsEquipment()
    {
      var timeCategories = _currentStatusBread.Select(null, -1, -1, "");
      foreach (var timeCategory in timeCategories)
      {
        CompareLastStatus(timeCategory);
      }
    }

    /// <summary>
    /// Checks current status against cached last status.
    /// If it's different, remove the last status, add the new status, and
    /// call InsertStatus to update the previous status, and add the new current status.
    /// </summary>
    /// <param name="currentStatus">Downtime status of one OEE equipment</param>
    public void CompareLastStatus(DTMCurrentStatus currentStatus)
    {
      var lastStatus = _mostRecentStatuses.Find(a => a.EquipmentId.Equals(currentStatus.EquipmentId));

      if (!(lastStatus.TimeCategory.Name.Equals(currentStatus.TimeCategory.Name) &&
          CheckIfEqual(lastStatus.Level1, currentStatus.Level1) &&
          CheckIfEqual(lastStatus.Level2, currentStatus.Level2) &&
          CheckIfEqual(lastStatus.Level3, currentStatus.Level3) &&
          CheckIfEqual(lastStatus.Level4, currentStatus.Level4)))
      {
        _mostRecentStatuses.Remove(lastStatus);
        _mostRecentStatuses.Add(currentStatus);
        InsertStatus(currentStatus, lastStatus);
      }
    }

    /// <summary>
    /// Checks to see if these machine states are equal.
    /// They can be the same status (e.g. "idle" and "idle") or both be null
    /// If they are the same, return true. If not, return false
    /// </summary>
    /// <param name="ms1">Machinestate for previous status</param>
    /// <param name="ms2">Machinestate for current status</param>
    /// <returns>True if ms1 and ms2 have the same name, or are both null. False if they're different</returns>
    public bool CheckIfEqual(MachineState ms1, MachineState ms2)
    {
      var equal = false;
      if ((ms1 == null) && (ms2 == null))
        equal = true;
      else if ((ms1 == null) ^ (ms2 == null))
        equal = true;
      else if (ms1.Name.Equals(ms2.Name))
        equal = true;
      return equal;
    }

    /// <summary>
    /// Calls stored procedure to insert current status and update last status
    /// </summary>
    /// <param name="currentStatus">Current status for a given piece of equipment</param>
    /// <param name="lastStatus">Previous status for a given piece of equipment</param>

    public void InsertStatus(DTMCurrentStatus currentStatus, DTMCurrentStatus lastStatus = null)
    {
      string configConnectionString = ConfigurationManager.ConnectionStrings["MOMDEVDBSRV"].ConnectionString;
      using (var sqlConnection = new SqlConnection(configConnectionString))
      {
        sqlConnection.Open();

        if (lastStatus != null)
          BuildAndExecuteQuery(sqlConnection, lastStatus, true);

        BuildAndExecuteQuery(sqlConnection, currentStatus, false);

        sqlConnection.Close();
      }
    }

    /// <summary>
    /// Builds and runs query to either update an existing downtime with an end time or add a new downtime.
    /// </summary>
    /// <param name="sqlConnection">Connection to Preactor database to use</param>
    /// <param name="status">the machine status to use for populating the parameters in the stored procedure</param>
    /// <param name="update"></param>
    public void BuildAndExecuteQuery(SqlConnection sqlConnection, DTMCurrentStatus status, bool update)
    {
      var dateTypeKey = update ? "@ToDate" : "@FromDate";
      var queryName = update ? "Calendar.UpdateLmsCalendarPeriod" : "Calendar.InsertLmsCalendarPeriod";
      using (var sqlCommand = new SqlCommand(queryName, sqlConnection))
      {
        var equip = _equipmentBread.Select(null, -1, -1, string.Format("{0} = {1}", "EquipmentID", status.EquipmentId));
        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("@ResourceName", equip.First().Name);
        sqlCommand.Parameters.AddWithValue(dateTypeKey, DateTime.Now);
        sqlCommand.Parameters.AddWithValue("@TimeCategory", status.TimeCategory.Name);
        sqlCommand.Parameters.AddWithValue("@Level1", (status.Level1 != null ? status.Level1.Name : ""));
        sqlCommand.Parameters.AddWithValue("@Level2", (status.Level2 != null ? status.Level2.Name : ""));
        sqlCommand.Parameters.AddWithValue("@Level3", (status.Level3 != null ? status.Level3.Name : ""));
        sqlCommand.Parameters.AddWithValue("@Level4", (status.Level4 != null ? status.Level4.Name : ""));
        sqlCommand.ExecuteNonQuery();
      }
    }

    public void FinalizePreactorTableEntries()
    {
      string configConnectionString = ConfigurationManager.ConnectionStrings["MOMDEVDBSRV"].ConnectionString;
      using (var sqlConnection = new SqlConnection(configConnectionString))
      {
        sqlConnection.Open();
        using (var sqlCommand = new SqlCommand("Calendar.FinalizeLmsCalendarPeriods", sqlConnection))
        {
          sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
          sqlCommand.Parameters.AddWithValue("@ToDate", DateTime.Now);
          sqlCommand.ExecuteNonQuery();
        }
        sqlConnection.Close();
      }
    }

    /// <summary>
    /// Actions to be performed when service stops
    /// </summary>
    protected override void OnStop()
    {
      EventLog.WriteEntry("Rtds monitor stopping", EventLogEntryType.Information);
      _rtdsTimerPoller.Dispose();
      FinalizePreactorTableEntries();
    }
  }
}
