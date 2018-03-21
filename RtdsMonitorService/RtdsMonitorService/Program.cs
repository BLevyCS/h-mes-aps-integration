using System.ServiceProcess;

namespace RtdsMonitorService
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main()
    {
      var servicesToRun = new ServiceBase[] 
          { 
            new RtdsMonitor() 
          };
      ServiceBase.Run(servicesToRun);
    }
  }
}
