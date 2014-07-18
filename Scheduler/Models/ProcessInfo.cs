namespace Autoscout24.Scheduler.Models
{
    public class ProcessInfo
    {
        public string FileName { get; set; }
        public string CommandArguments { get; set; }        
        public string WorkingDirectory { get; set; }
        public string ResultFilePath { get; set; }
        public int ProcessId { get; set; }
    }
}