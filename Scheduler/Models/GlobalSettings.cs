using System.Collections.Generic;

namespace Autoscout24.Scheduler.Models
{
    public class GlobalSettings
    {
        public TestEnvironment TestEnvironment { get; set; }
        public int NoOfParallelProcesses { get; set; }
        public bool ReRunFailedTests { get; set; }
        public List<SeleniumNode> SeleniumNodes { get; set; }
        public List<MachineConfig> MachineConfigs { get; set; } 
        public string DeliverySystemCmd { get; set; }
        public string NUnitArgs { get; set; }
        public int ReRunFailedTestsLimit { get; set; }
    }
}