namespace RtdsMonitorService
{
  partial class ProjectInstaller
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.LmsMonitorServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
      this.LmsIntegrationServiceInstaller = new System.ServiceProcess.ServiceInstaller();
      // 
      // LmsMonitorServiceProcessInstaller
      // 
      this.LmsMonitorServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
      this.LmsMonitorServiceProcessInstaller.Password = null;
      this.LmsMonitorServiceProcessInstaller.Username = null;
      // 
      // LmsIntegrationServiceInstaller
      // 
      this.LmsIntegrationServiceInstaller.Description = "Calls BREAD for OEE machines to find current status and reports any change in sta" +
"tus to be picked up by Preactor.";
      this.LmsIntegrationServiceInstaller.DisplayName = "LmsIntegrationService";
      this.LmsIntegrationServiceInstaller.ServiceName = "LmsIntegrationService";
      this.LmsIntegrationServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
      // 
      // ProjectInstaller
      // 
      this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.LmsMonitorServiceProcessInstaller,
            this.LmsIntegrationServiceInstaller});

    }

    #endregion

    private System.ServiceProcess.ServiceProcessInstaller LmsMonitorServiceProcessInstaller;
    private System.ServiceProcess.ServiceInstaller LmsIntegrationServiceInstaller;
  }
}