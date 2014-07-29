using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Autoscout24.Scheduler.Models;

using log4net;

namespace Autoscout24.Scheduler
{
    public class ResultPrinter
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string deliverySystemCmd;
        private readonly ResultModel globalTotals;
        private readonly ResultModel totalsRoundOne;
        private readonly ResultModel totalsRoundTwo;
        private int currentRoundNo;     
        private static readonly object LockObject = new object();

        public ResultPrinter(string deliverySystemCmd)
        {
            this.deliverySystemCmd = deliverySystemCmd;
            totalsRoundOne = new ResultModel();
            totalsRoundTwo = new ResultModel();
            globalTotals = new ResultModel();
        }

        public void UseRoundNo(int roundNo)
        {
            currentRoundNo = roundNo;
        }

        public void PrintIntermediate(ResultModel resultModel, string pathToTestResult, StringBuilder processOutput)
        {
            lock (LockObject)
            {
                Console.WriteLine(processOutput.ToString());
                MergeTestsResults(resultModel);
                Console.WriteLine(deliverySystemCmd, pathToTestResult);    
            }                                    
        }

        public void PrintRoundOne()
        {
            Console.WriteLine();
            Console.WriteLine("Tests results for round one:");
            Console.WriteLine("Fixture or test: {0}", totalsRoundOne.TestFixture);
            Console.WriteLine("Tests run: {0}", totalsRoundOne.Total);
            Console.WriteLine("Tests failed: {0}", totalsRoundOne.Failures);
            Console.WriteLine("Tests with errors: {0}", totalsRoundOne.Errors);
            Console.WriteLine("Tests not run: {0}", totalsRoundOne.NotRun);
            Console.WriteLine("Tests inconclusive: {0}", totalsRoundOne.Inconclusive);
            Console.WriteLine("Tests ignored: {0}", totalsRoundOne.Ignored);
            Console.WriteLine("Tests skipped: {0}", totalsRoundOne.Skipped);
            Console.WriteLine("Tests invalid: {0}", totalsRoundOne.Invalid);
            Console.WriteLine("...........................................");
            Console.WriteLine();
        }

        public void PrintRoundTwo()
        {
            Console.WriteLine();
            Console.WriteLine("Tests results for round two:");
            Console.WriteLine("Fixture or test: {0}", totalsRoundTwo.TestFixture);
            Console.WriteLine("Tests run: {0}", totalsRoundTwo.Total);
            Console.WriteLine("Tests failed: {0}", totalsRoundTwo.Failures);
            Console.WriteLine("Tests with errors: {0}", totalsRoundTwo.Errors);
            Console.WriteLine("Tests not run: {0}", totalsRoundTwo.NotRun);
            Console.WriteLine("Tests inconclusive: {0}", totalsRoundTwo.Inconclusive);
            Console.WriteLine("Tests ignored: {0}", totalsRoundTwo.Ignored);
            Console.WriteLine("Tests skipped: {0}", totalsRoundTwo.Skipped);
            Console.WriteLine("Tests invalid: {0}", totalsRoundTwo.Invalid);
            Console.WriteLine("...........................................");
            Console.WriteLine();
        }

        public void PrintGlobalResults()
        {
            globalTotals.Failures = totalsRoundOne.Failures + totalsRoundTwo.Failures;
            globalTotals.Total += totalsRoundOne.Total + totalsRoundTwo.Total;
            globalTotals.Errors += totalsRoundOne.Errors + totalsRoundTwo.Total;
            globalTotals.NotRun += totalsRoundOne.NotRun + totalsRoundTwo.NotRun;
            globalTotals.Inconclusive += totalsRoundOne.Inconclusive + totalsRoundTwo.Inconclusive;
            globalTotals.Ignored += totalsRoundOne.Ignored + totalsRoundTwo.Ignored;
            globalTotals.Skipped += totalsRoundOne.Skipped + totalsRoundTwo.Skipped;
            globalTotals.Invalid += totalsRoundOne.Invalid + totalsRoundTwo.Invalid;
            globalTotals.FailedTestCases.AddRange(totalsRoundOne.FailedTestCases);
            globalTotals.FailedTestCases.AddRange(totalsRoundTwo.FailedTestCases);

            Console.WriteLine();
            Console.WriteLine("Tests results for both rounds:");            
            Console.WriteLine("Tests run: {0}", globalTotals.Total);
            Console.WriteLine("Tests failed: {0}", globalTotals.Failures);
            Console.WriteLine("Tests with errors: {0}", globalTotals.Errors);
            Console.WriteLine("Tests not run: {0}", globalTotals.NotRun);
            Console.WriteLine("Tests inconclusive: {0}", globalTotals.Inconclusive);
            Console.WriteLine("Tests ignored: {0}", globalTotals.Ignored);
            Console.WriteLine("Tests skipped: {0}", globalTotals.Skipped);
            Console.WriteLine("Tests invalid: {0}", globalTotals.Invalid);
            Console.WriteLine("...........................................");
            Console.WriteLine();
        }

        public void PrintListOfNamespaces(IEnumerable<string> testsList)
        {
            Console.WriteLine("Will test following fixtures/namespaces:");
            Log.Info("Will test following fixtures/namespaces:");
            Console.WriteLine();
            foreach (var test in testsList)
            {
                Console.WriteLine(test);  
                Log.Info(test);
            }
            Console.WriteLine();
        }

        public void PrintMachinesAndParallelProcessesCount(int machinesCount, int parallelProcessesCount)
        {
            Console.WriteLine("Will run the tests on {0} machines.", machinesCount);
            Log.Info(string.Format("Will run the tests on {0} machines.", machinesCount));
            Console.WriteLine("Will run {0} tests in parallel.", parallelProcessesCount);
            Log.Info(string.Format("Will run {0} tests in parallel.", parallelProcessesCount));
            Console.WriteLine();
        }

        private void MergeTestsResults(ResultModel resultModel)
        {            
            if (currentRoundNo == 1)
            {
                totalsRoundOne.Failures += resultModel.Failures;
                totalsRoundOne.Total += resultModel.Total;
                totalsRoundOne.Errors += resultModel.Errors;
                totalsRoundOne.NotRun += resultModel.NotRun;
                totalsRoundOne.Inconclusive += resultModel.Inconclusive;
                totalsRoundOne.Ignored += resultModel.Ignored;
                totalsRoundOne.Skipped += resultModel.Skipped;
                totalsRoundOne.Invalid += resultModel.Invalid;
                totalsRoundOne.FailedTestCases.AddRange(resultModel.FailedTestCases);
            }
            else
            {
                totalsRoundTwo.Failures += resultModel.Failures;
                totalsRoundTwo.Total += resultModel.Total;
                totalsRoundTwo.Errors += resultModel.Errors;
                totalsRoundTwo.NotRun += resultModel.NotRun;
                totalsRoundTwo.Inconclusive += resultModel.Inconclusive;
                totalsRoundTwo.Ignored += resultModel.Ignored;
                totalsRoundTwo.Skipped += resultModel.Skipped;
                totalsRoundTwo.Invalid += resultModel.Invalid;
                totalsRoundTwo.FailedTestCases.AddRange(resultModel.FailedTestCases);
            }

            Console.WriteLine();
            Console.WriteLine("Tests results for (fixture or test): {0}", resultModel.TestFixture);
            Console.WriteLine("Tests run: {0}", resultModel.Total);
            Console.WriteLine("Tests failed: {0}", resultModel.Failures);
            Console.WriteLine("Tests with errors: {0}", resultModel.Errors);
            Console.WriteLine("Tests not run: {0}", resultModel.NotRun);
            Console.WriteLine("Tests inconclusive: {0}", resultModel.Inconclusive);
            Console.WriteLine("Tests ignored: {0}", resultModel.Ignored);
            Console.WriteLine("Tests skipped: {0}", resultModel.Skipped);
            Console.WriteLine("Tests invalid: {0}", resultModel.Invalid);
            Console.WriteLine("...........................................");
            Console.WriteLine();
        }
    }
}
