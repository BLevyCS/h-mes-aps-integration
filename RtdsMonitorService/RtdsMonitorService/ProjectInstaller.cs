using System.ComponentModel;

namespace RtdsMonitorService
{
  [RunInstaller(true)]
  public partial class ProjectInstaller : System.Configuration.Install.Installer
  {
    public ProjectInstaller()
    {
      InitializeComponent();
    }
  }
}
