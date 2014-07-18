using AutoScout24.SeleniumTestLibrary.Common;

namespace Autoscout24.Scheduler.Models
{
    public class SeleniumNode
    {
        public ClientType ClientType { get; set; }
        public int Port { get; set; }
        public string Machine { get; set; }
    }
}