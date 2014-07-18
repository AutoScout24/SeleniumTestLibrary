using System;
using System.IO;
using System.Reflection;

using Autoscout24.Scheduler.Models;

using log4net;
using log4net.Config;

namespace Autoscout24.Scheduler
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            XmlConfigurator.Configure();
            var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Started");

            if (args == null || args.Length == 0)
            {
                throw new ArgumentException("No arguments were provided.");
            }
            if (args[0] == null)
            {
                throw new ArgumentException("Test assembly path was not set.");
            }
            if (args[1] == null)
            {
                throw new ArgumentException("Environment (Stable, Ref, Live) was not set.");
            }
            
            var testEnvironment = args[1];
            var testAssemblyPath = args[0];

            if (!File.Exists(testAssemblyPath))
            {
                throw new ArgumentException(string.Format("Given test assembly path is not available or does not exist: {0}.", testAssemblyPath));
            }

            var environment = (TestEnvironment) Enum.Parse(typeof(TestEnvironment), testEnvironment, true);                        

            var globalSettings = new GlobalSettings();
            var testRunSettings = new TestRunSettings();

            var settings = ConfigReader.Settings();

            globalSettings.DeliverySystemCmd = settings.ContinuousIntegrationSystemCmd;
            globalSettings.MachineConfigs = settings.MachineConfigs;
            globalSettings.NoOfParallelProcesses = settings.NoOfParallelRun;
            globalSettings.ReRunFailedTests = settings.ReRunFailedTests;
            globalSettings.ReRunFailedTestsLimit = settings.ReRunFailedTestsLimit;
            globalSettings.SeleniumNodes = settings.SeleniumClients;
            globalSettings.TestEnvironment = TestEnvironment.Ref;
            globalSettings.NUnitArgs = settings.NUnitArgs;

            testRunSettings.AppendBuildNumberToPath = settings.AppendBuildNumberToPath;
            testRunSettings.FilesListPath = @"fileList.txt";
            testRunSettings.ScreenshotPath = settings.ScreenshotLocation;
            testRunSettings.ScreenshotUrl = settings.ScreenshotUrl;

            var manager = new Manager(testRunSettings, globalSettings);
            return manager.Manage(testAssemblyPath, environment);
        }
    }
}
