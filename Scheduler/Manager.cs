using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using Autoscout24.Scheduler.Models;

using log4net;

namespace Autoscout24.Scheduler
{
    public class Manager
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string nUnitArgs;
        private readonly GlobalSettings globalSettings;
        private readonly HostFileSwitcher hostFileSwitcher;
        private readonly SetupCreator setupCreator;
        private readonly NodesManager nodesManager;
        private readonly ResultPrinter resultPrinter;
        private readonly ResultMerger resultMerger;
        private readonly Splitter splitter;                
        private int processIdCounter;        
        private CountdownEvent processesCountdown;                

        public Manager(TestRunSettings testRunSettings, GlobalSettings globalSettings)
        {
            this.globalSettings = globalSettings;
            hostFileSwitcher = new HostFileSwitcher();
            splitter = new Splitter();
            setupCreator = new SetupCreator(testRunSettings);
            resultMerger = new ResultMerger();
            nodesManager = new NodesManager(globalSettings.NoOfParallelProcesses);
            resultPrinter = new ResultPrinter(globalSettings.DeliverySystemCmd);
            nodesManager.InitTestConfigs(globalSettings.SeleniumNodes);            
            nUnitArgs = globalSettings.NUnitArgs;
        }       

        public int Manage(string testAssemblyPath, TestEnvironment environment)
        {
            resultPrinter.PrintMachinesAndParallelProcessesCount(globalSettings.MachineConfigs.Count,globalSettings.NoOfParallelProcesses);
            SetHostsEnvironment(globalSettings.MachineConfigs, environment);            

            var namespaces = GetTestsFromAssembly(testAssemblyPath);
            resultPrinter.PrintListOfNamespaces(namespaces);
            
            processIdCounter = 0;
            processesCountdown = new CountdownEvent(namespaces.Count);

            resultPrinter.UseRoundNo(1);
            resultMerger.ForgetFailedTestCases();
            RunRound(namespaces, testAssemblyPath);

            processesCountdown.Wait();          

            resultPrinter.PrintRoundOne();

            namespaces = new List<string>(resultMerger.GetFailedTestsCases());
            var returnCode = namespaces.Count;

            if (ShouldReRunFailedTests(namespaces))
            {
                // If second run is enabled the return code should be the no of failed test cases in the second run
                // Therefore we have to reset the failed ones from round one
                resultMerger.ForgetFailedTestCases();
                resultPrinter.UseRoundNo(2);
                processesCountdown = new CountdownEvent(namespaces.Count);
                resultPrinter.PrintListOfNamespaces(namespaces);
                RunRound(namespaces, testAssemblyPath);
                processesCountdown.Wait();
                resultPrinter.PrintRoundTwo();

                returnCode = resultMerger.GetFailedTestsCases().Count;
            }            
            
            resultPrinter.PrintGlobalResults();
            return returnCode;
        }

        private bool ShouldReRunFailedTests(List<string> namespaces)
        {
            if (namespaces.Count == 0)
            {
                Console.WriteLine("All tests green from first run! Cool!");
                return false;
            }
            if (globalSettings.ReRunFailedTests == false)
            {
                Console.WriteLine("ReRun of failed tests is disabled");
                return false;
            }
            if (namespaces.Count > globalSettings.ReRunFailedTestsLimit)
            {
                Console.WriteLine("ReRun is disabled because the number of failed test is higher than the current ReRunFailedTestsLimit setting value.");
                return false;
            }
            return true;
        }

        private void RunRound(IEnumerable<string> namespaces, string testAssemblyPath)
        {
            namespaces = namespaces.ToList();
            foreach (var testNamespace in namespaces)
            {                                
                processIdCounter++;
                var testConfig = nodesManager.GetTestConfig(processIdCounter);
                setupCreator.SetupFor(processIdCounter, testAssemblyPath, testConfig);
                RunProcess(processIdCounter, testNamespace, testAssemblyPath);
            }
        }

        private void RunProcess(int processId, string testNamespace, string testAssemblyPath)
        {            
            var testProcessInfo = new ProcessInfo();
            var nUnitPath = Path.Combine(Environment.CurrentDirectory, processId.ToString(CultureInfo.InvariantCulture), @"nunit-console.exe");

            testProcessInfo.ProcessId = processId;                     
            testProcessInfo.FileName = nUnitPath;

            var testResultFilePath = GetTestResultFilePath(processId, testNamespace);
            testNamespace = testNamespace.Replace("\"", "\\\"");
            var arguments = string.Format(nUnitArgs, testResultFilePath, testNamespace, testAssemblyPath);            
            testProcessInfo.CommandArguments = arguments;
            testProcessInfo.ResultFilePath = testResultFilePath;
            testProcessInfo.WorkingDirectory = Path.Combine(Environment.CurrentDirectory, processId.ToString(CultureInfo.InvariantCulture));

            var testProcessContext = new ProcessRunner();
            testProcessContext.ProcessExited += TestProcessRunnerOnProcessExited;
            testProcessContext.Run(testProcessInfo);

            Log.Info(string.Format("Started process for: {0} {1}", testNamespace, processId));
        }             

        private static string GetTestResultFilePath(int processId, string testNamespace)
        {
            var tests = new StringBuilder(testNamespace).ToString();
            if (tests.Contains(","))
            {
                tests = tests.Remove(tests.IndexOf(",", StringComparison.Ordinal));
                tests = tests + "_GroupedTests";
            }
            tests = tests.Replace("\"", "");

            var testResultFileName = Path.Combine(Environment.CurrentDirectory,
                processId.ToString(CultureInfo.InvariantCulture),
                string.Format("{0}-{1}.xml", Guid.NewGuid().ToString().Remove(6), tests));
            return testResultFileName;
        }

        private void TestProcessRunnerOnProcessExited(object sender, ProcessRunnerExitedEventArgs e)
        {
            Log.Info(string.Format("Ended process for: {0} {1}", e.TestsResultFilePath, e.ProcessId));
            Log.Info(string.Format("Output of the process is: {0} {1}", e.ProcessOutput, e.ProcessId));
            nodesManager.FreeNode(e.ProcessId);                                  
            var testsResultsModel = resultMerger.GetResult(e.TestsResultFilePath);
            resultPrinter.PrintIntermediate(testsResultsModel, e.TestsResultFilePath, e.ProcessOutput);
            processesCountdown.Signal();                        
        }

        private void SetHostsEnvironment(List<MachineConfig> machineConfigs, TestEnvironment testEnvironment = TestEnvironment.Ref)
        {
            hostFileSwitcher.SwitchTo(testEnvironment, machineConfigs);
        }

        private List<string> GetTestsFromAssembly(string assemblyPath, int smallTestGroupLimit = 3)
        {            
            return splitter.GetNamespaces(assemblyPath, smallTestGroupLimit);            
        }       
    }
}
