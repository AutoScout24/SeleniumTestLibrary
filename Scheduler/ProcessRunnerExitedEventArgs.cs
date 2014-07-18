using System;
using System.Text;

namespace Autoscout24.Scheduler
{
    public class ProcessRunnerExitedEventArgs : EventArgs
    {
        public StringBuilder ProcessOutput { get; set; }
        public string TestsResultFilePath { get; set; }
        public int ProcessId { get; set; }
    }
}