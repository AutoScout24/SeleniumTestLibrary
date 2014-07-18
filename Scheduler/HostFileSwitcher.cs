using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Autoscout24.Scheduler.Models;

namespace Autoscout24.Scheduler
{
    public class HostFileSwitcher
    {
        public void SwitchTo(TestEnvironment toEnvironment, List<MachineConfig> machineConfigs)
        {
            foreach (var machineConfig in machineConfigs)
            {
                var driveLetter = GetAvailableDriveLetters()[1] + ":";
                if (!LoadDrive(driveLetter, machineConfig.MachineName, machineConfig.MachineUsername, machineConfig.MachinePassword))
                {
                    throw new Exception(string.Format("Unable to set the hosts file for: {0}", machineConfig.MachineName));
                }
                try
                {
                    File.Copy(GetFilePath(toEnvironment), driveLetter + @"\hosts", true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    UnloadDrive(driveLetter);
                }
            }
        }

        private static bool LoadDrive(string driveLetter, string machineName, string userName, string password)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "net.exe",
                    Arguments = string.Format(@"use {0} \\{1}\c$\windows\system32\drivers\etc /user:{2} {3}", driveLetter, machineName, userName, password),
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };

                var process = new Process
                {
                    StartInfo = startInfo
                };
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        private static void UnloadDrive(string driveLetter)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "net.exe",
                        Arguments = string.Format(@"use {0} /delete", driveLetter),
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static string GetFilePath(TestEnvironment testEnvironment)
        {
            switch (testEnvironment)
            {
                case TestEnvironment.Stable:
                    return Path.Combine(Environment.CurrentDirectory, @"Hosts\stable");
                case TestEnvironment.Ref:
                    return Path.Combine(Environment.CurrentDirectory, @"Hosts\ref");
                case TestEnvironment.Live:
                    return Path.Combine(Environment.CurrentDirectory, @"Hosts\live");
            }

            throw new ArgumentException(string.Format("Uknown environment:{0}", testEnvironment));
        }

        private static List<string> GetAvailableDriveLetters()
        {
            var letters = new List<string>();
            for (int i = Convert.ToInt16('a'); i < Convert.ToInt16('z'); i++)
            {
                letters.Add(new string(new[] {(char) i}));
            }
            foreach (var drive in DriveInfo.GetDrives())
            {
                letters.Remove(drive.Name.Substring(0, 1).ToLower());
            }

            return letters;
        }        
    }
}
