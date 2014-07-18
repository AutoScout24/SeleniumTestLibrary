namespace Autoscout24.Scheduler.Models
{
    public class NodeConfig
    {
        public SeleniumNode ChromeNode { get; set; }
        public SeleniumNode FirefoxNode { get; set; }
        public SeleniumNode IeNode { get; set; }
    }
}