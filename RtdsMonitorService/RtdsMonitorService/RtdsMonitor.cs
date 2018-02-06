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
        private DTMCurrentStatus_BREAD CurrentStatusBread = new DTMCurrentStatus_BREAD();
        private Equipment_BREAD EquipmentBread = new Equipment_BREAD();
        private List<DTMCurrentStatus> MostRecentStatuses;

        public RtdsMonitor()
        {
            this.ServiceName = "RtdsMonitorService";

            InitializeComponent();
        }

        /// <summary>
        /// Actions to be performed when service starts
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            this.EventLog.WriteEntry("Pulling equipment from OEE and populating initial status", EventLogEntryType.Information);
            var timeCats = CurrentStatusBread.Select(null, -1, -1, "");
            MostRecentStatuses = new List<DTMCurrentStatus>(timeCats);
            foreach (var stat in MostRecentStatuses)
                CallPreactorTable(stat);
            var tickTimeInSeconds = Int32.Parse(ConfigurationManager.AppSettings["tickTimeInSeconds"]);
            _rtdsTimerPoller = new Timer(rtdsTimerPoller_Tick, null, 0, tickTimeInSeconds * 1000);
        }

        /// <summary>
        /// Handles the timer ticking over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtdsTimerPoller_Tick(object state)
        {
            RtdsStuff();
        }

        /// <summary>
        /// Looks at each piece of equipment in OEE
        /// </summary>
        private void RtdsStuff()
        {
            var timeCats = CurrentStatusBread.Select(null, -1, -1, "");
            foreach (var cat in timeCats)
            {
                CompareLastStatus(cat);
            }
        }

        /// <summary>
        /// Checks current status against cached last status.
        /// </summary>
        /// <param name="currentStatus">Downtime status of one OEE equipment</param>
        public void CompareLastStatus(DTMCurrentStatus currentStatus)
        {
            var lastStatus = MostRecentStatuses.Find(a => a.EquipmentId.Equals(currentStatus.EquipmentId));

            if (!(lastStatus.TimeCategory.Name.Equals(currentStatus.TimeCategory.Name) &&
                CheckIfEqual(lastStatus.Level1, currentStatus.Level1) &&
                CheckIfEqual(lastStatus.Level2, currentStatus.Level2) &&
                CheckIfEqual(lastStatus.Level3, currentStatus.Level3) &&
                CheckIfEqual(lastStatus.Level4, currentStatus.Level4)))
            {
                MostRecentStatuses.Remove(lastStatus);
                MostRecentStatuses.Add(currentStatus);
                CallPreactorTable(currentStatus, lastStatus);
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
        /// <param name="currentStatus">Current status for a given OEE equipment</param>
        /// <param name="lastStatus">When applicable, last status, to set a "toDate" for</param>
 
        public void CallPreactorTable(DTMCurrentStatus currentStatus, DTMCurrentStatus lastStatus = null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MOMDEVDBSRV"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                if (lastStatus != null)
                {
                    using (SqlCommand cm = new SqlCommand("Calendar.UpdateLmsCalendarPeriod", cn))
                    {
                        var equip = EquipmentBread.Select(null, -1, -1, string.Format("{0} = {1}", "EquipmentID", lastStatus.EquipmentId));
                        cm.CommandType = System.Data.CommandType.StoredProcedure;
                        cm.Parameters.AddWithValue("@ResourceName", equip.First().Name);
                        cm.Parameters.AddWithValue("@ToDate", DateTime.Now);
                        cm.Parameters.AddWithValue("@TimeCategory", lastStatus.TimeCategory.Name);
                        cm.Parameters.AddWithValue("@Level1", (lastStatus.Level1 != null ? lastStatus.Level1.Name : ""));
                        cm.Parameters.AddWithValue("@Level2", (lastStatus.Level2 != null ? lastStatus.Level2.Name : ""));
                        cm.Parameters.AddWithValue("@Level3", (lastStatus.Level3 != null ? lastStatus.Level3.Name : ""));
                        cm.Parameters.AddWithValue("@Level4", (lastStatus.Level4 != null ? lastStatus.Level4.Name : ""));
                        cm.ExecuteNonQuery();
                    }
                }
                using (SqlCommand cm = new SqlCommand("Calendar.InsertLmsCalendarPeriod", cn))
                {
                    var equip = EquipmentBread.Select(null, -1, -1, string.Format("{0} = {1}", "EquipmentID", currentStatus.EquipmentId));
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@ResourceName", equip.First().Name);
                    cm.Parameters.AddWithValue("@FromDate", DateTime.Now);
                    cm.Parameters.AddWithValue("@TimeCategory", currentStatus.TimeCategory.Name);
                    cm.Parameters.AddWithValue("@Level1", (currentStatus.Level1 != null ? currentStatus.Level1.Name : ""));
                    cm.Parameters.AddWithValue("@Level2", (currentStatus.Level2 != null ? currentStatus.Level2.Name : ""));
                    cm.Parameters.AddWithValue("@Level3", (currentStatus.Level3 != null ? currentStatus.Level3.Name : ""));
                    cm.Parameters.AddWithValue("@Level4", (currentStatus.Level4 != null ? currentStatus.Level4.Name : ""));
                    cm.ExecuteNonQuery();
                }
                cn.Close();
            }
        }

        public void FinalizePreactorTableEntries()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MOMDEVDBSRV"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand("Calendar.FinalizeLmsCalendarPeriods", cn))
                {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@ToDate", DateTime.Now);
                    cm.ExecuteNonQuery();
                }
                cn.Close();
            }
        }

        /// <summary>
        /// Actions to be performed when service stops
        /// </summary>
        protected override void OnStop()
        {
            this.EventLog.WriteEntry("Rtds monitor stopping", EventLogEntryType.Information);
            _rtdsTimerPoller.Dispose();
            FinalizePreactorTableEntries();
        }
    }
}
