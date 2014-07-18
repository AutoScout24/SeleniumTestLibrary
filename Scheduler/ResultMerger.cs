using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Autoscout24.Scheduler.Models;

namespace Autoscout24.Scheduler
{
    public class ResultMerger
    {
        private static readonly List<string> FailedTestsCases = new List<string>();

        public ResultModel GetResult(string filePath)
        {
            if (!File.Exists(filePath))
            {
                // TODO handle missing result file
                return new ResultModel();
            }
            var testresultxmldoc = new XmlDocument();
            testresultxmldoc.Load(filePath);

            var mainresultnode = testresultxmldoc.SelectSingleNode("test-results");
            if (mainresultnode == null)
                return new ResultModel();

            int ignoredtests = Convert.ToInt16(mainresultnode.Attributes["ignored"].Value);
            int errors = Convert.ToInt16(mainresultnode.Attributes["errors"].Value);
            int skipped = Convert.ToInt16(mainresultnode.Attributes["skipped"].Value);
            int failures = Convert.ToInt16(mainresultnode.Attributes["failures"].Value);
            int totaltests = Convert.ToInt16(mainresultnode.Attributes["total"].Value);
            int notRun = Convert.ToInt16(mainresultnode.Attributes["not-run"].Value);
            int invalidtests = Convert.ToInt16(mainresultnode.Attributes["invalid"].Value);
            int inconclusivetests = Convert.ToInt16(mainresultnode.Attributes["inconclusive"].Value);

            var testFixture = filePath.Substring(filePath.LastIndexOf("-", StringComparison.Ordinal) + 1);
            testFixture = testFixture.Remove(testFixture.LastIndexOf(".", StringComparison.Ordinal));

            var testsResults = new ResultModel
                {
                    Ignored = ignoredtests,
                    Errors = errors,
                    Failures = failures,
                    Total = totaltests,
                    NotRun = notRun,
                    Skipped = skipped,
                    Invalid = invalidtests,
                    Inconclusive = inconclusivetests,
                    TestFixture = testFixture
                };

            if (errors > 0 || failures > 0)
            {
                var testCases = testresultxmldoc.SelectNodes("//test-case");
                if (testCases == null)
                    throw new Exception("Unable to figure out what kind of result XML is this one.");
                    
                foreach (XmlNode testCase in testCases)
                {
                    var testResultValue = testCase.Attributes["result"].Value;
                    if (testResultValue.Equals("Failure") || testResultValue.Equals("Error"))
                    {                        
                        var testCaseName = testCase.Attributes["name"].Value;                        
                        testsResults.FailedTestCases.Add(testCaseName);
                        if (!FailedTestsCases.Contains(testCaseName))
                        {
                            FailedTestsCases.Add(testCaseName);
                        }
                    }
                }
            }
            
            return testsResults;
        }

        public List<string> GetFailedTestsCases()
        {
            return FailedTestsCases;
        }

        public void ForgetFailedTestCases()
        {
            FailedTestsCases.Clear();
        }
    }
}
