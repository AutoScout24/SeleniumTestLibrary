using System.Collections.Generic;

namespace Autoscout24.Scheduler.Models
{
    public class SchedulerSettings
    {
        public List<MachineConfig> MachineConfigs { get; set; }
        public int NoOfMachines { get; set; }
        public int NoOfParallelRun { get; set; }
        public string ScreenshotLocation { get; set; }
        public string ScreenshotUrl { get; set; }
        public bool AppendBuildNumberToPath { get; set; }
        public int NoOfChromeClients { get; set; }
        public int NoOfFirefoxClients { get; set; }
        public int NoOfIeClients { get; set; }
        public List<SeleniumNode> SeleniumClients { get; set; }
        public bool ReRunFailedTests { get; set; }
        public int ReRunFailedTestsLimit { get; set; }
        public int TestsGroupLimit { get; set; }
        public string ContinuousIntegrationSystemCmd { get; set; }
        public string NUnitArgs { get; set; }
    }

    
}
