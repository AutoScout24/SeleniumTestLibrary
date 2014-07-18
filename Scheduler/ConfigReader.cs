using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using Autoscout24.Scheduler.Models;

using AutoScout24.SeleniumTestLibrary.Common;

namespace Autoscout24.Scheduler
{
    public static class ConfigReader
    {
        private static SchedulerSettings schedulerSettings;

        public static SchedulerSettings Settings()
        {
            if (schedulerSettings != null)
            {
                return schedulerSettings;
            }

            schedulerSettings = new SchedulerSettings
                {
                    NoOfMachines = Convert.ToInt32(ConfigurationManager.AppSettings["NoOfMachines"]),
                    NoOfParallelRun = Convert.ToInt32(ConfigurationManager.AppSettings["NoOfParallelRun"]),
                    NoOfChromeClients = Convert.ToInt32(ConfigurationManager.AppSettings["NoOfChromeClients"]),
                    NoOfFirefoxClients = Convert.ToInt32(ConfigurationManager.AppSettings["NoOfFirefoxClients"]),
                    NoOfIeClients = Convert.ToInt32(ConfigurationManager.AppSettings["NoOfIeClients"]),
                    ScreenshotLocation = ConfigurationManager.AppSettings["ScreenshotPath"],
                    ScreenshotUrl = ConfigurationManager.AppSettings["ScreenshotUrl"],
                    NUnitArgs = ConfigurationManager.AppSettings["NUnitArgs"],
                    AppendBuildNumberToPath = Convert.ToBoolean(ConfigurationManager.AppSettings["AppendBuildNumberToPath"]),
                    ReRunFailedTests = Convert.ToBoolean(ConfigurationManager.AppSettings["ReRunFailedTests"]),
                    ReRunFailedTestsLimit = Convert.ToInt32(ConfigurationManager.AppSettings["ReRunFailedTestsLimit"]),
                    TestsGroupLimit = Convert.ToInt32(ConfigurationManager.AppSettings["TestsGroupLimit"]),
                    
                    ContinuousIntegrationSystemCmd = ConfigurationManager.AppSettings["ContinuousIntegrationSystemCmd"]
                };

            var machineNames = GetMachineNames();
            var seleniumClients = new List<SeleniumNode>();
            foreach (var machineName in machineNames)
            {
                foreach (var clientType in Enum.GetValues(typeof (ClientType)))
                {
                    var clients = GetClientsPorts((ClientType) clientType, machineName).ToList();
                    seleniumClients.AddRange(clients);
                    switch ((ClientType) clientType)
                    {
                        case ClientType.Chrome:
                            schedulerSettings.NoOfChromeClients = clients.Count;
                            break;
                        case ClientType.Firefox:
                            schedulerSettings.NoOfFirefoxClients = clients.Count;
                            break;
                        case ClientType.Ie:
                            schedulerSettings.NoOfIeClients = clients.Count;
                            break;
                    }
                }
            }
            schedulerSettings.SeleniumClients = seleniumClients;
            schedulerSettings.MachineConfigs = GetMachineConfigs();

            return schedulerSettings;
        }

        private static IEnumerable<SeleniumNode> GetClientsPorts(ClientType clientType, string machineName)
        {
            var seleniumClients = new List<SeleniumNode>();
            var noOfClientsAsString = string.Empty;
            var clientPortAsString = string.Empty;
            if (clientType == ClientType.Chrome)
            {
                noOfClientsAsString = "NoOfChromeClients";
                clientPortAsString = "ChromeClient{0}Port";
            }
            if (clientType == ClientType.Firefox)
            {
                noOfClientsAsString = "NoOfFirefoxClients";
                clientPortAsString = "FirefoxClient{0}Port";
            }
            if (clientType == ClientType.Ie)
            {
                noOfClientsAsString = "NoOfIeClients";
                clientPortAsString = "IeClient{0}Port";
            }

            var value = ConfigurationManager.AppSettings[noOfClientsAsString];
            if (value == null)
            {
                return null;
            }
            var noOfMachines = Convert.ToInt32(value);

            for (var i = 0; i < noOfMachines; i++)
            {
                var client = new SeleniumNode
                    {
                        ClientType = clientType,
                        Port = Convert.ToInt32(ConfigurationManager.AppSettings[string.Format(clientPortAsString, i + 1)]),
                        Machine = machineName
                    };
                seleniumClients.Add(client);
            }
            return seleniumClients;
        }

        private static IEnumerable<string> GetMachineNames()
        {
            var machines = new List<string>();
            for (var i = 0; i < schedulerSettings.NoOfMachines; i++)
            {
                machines.Add(ConfigurationManager.AppSettings[string.Format("Machine{0}", i + 1)]);
            }
            return machines;
        }

        private static List<MachineConfig> GetMachineConfigs()
        {
            var machineConfigs = new List<MachineConfig>();
            for (var i = 0; i < schedulerSettings.NoOfMachines; i++)
            {
                machineConfigs.Add(new MachineConfig
                    {
                        MachineName = ConfigurationManager.AppSettings[string.Format("Machine{0}", i + 1)],
                        MachineUsername = ConfigurationManager.AppSettings[string.Format("Machine{0}Username", i + 1)],
                        MachinePassword = ConfigurationManager.AppSettings[string.Format("Machine{0}Password", i + 1)],
                    });
            }
            return machineConfigs;
        }
    }
}
