using System.Collections.Generic;

namespace Autoscout24.Scheduler.Models
{
    public class ResultModel
    {
        public ResultModel()
        {
            FailedTestCases = new List<string>();
        }
        public string TestFixture { get; set; }
        public int Ignored { get; set; }
        public int Skipped { get; set; }
        public int Errors { get; set; }
        public int NotRun { get; set; }
        public int Failures { get; set; }
        public int Total { get; set; }
        public int Invalid { get; set; }
        public int Inconclusive { get; set; }
        public List<string> FailedTestCases { get; set; }
    }
}