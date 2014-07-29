using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

using AutoScout24.SeleniumTestLibrary.Common;

using Newtonsoft.Json;

namespace AutoScout24.SeleniumTestLibrary.Setup
{
    public static class TestConfigReader
    {
        private const string RequestUri = "http://{0}:{1}/wd/hub/sessions";
        private static readonly int ChromePort;
        private static readonly int FirefoxPort;
        private static readonly int IePort;
        private static readonly string Machine;
        private static bool nodeCalled;

        static TestConfigReader()
        {
            var path = CheckAndReturnPath();        
            if (string.IsNullOrEmpty(path))
                return;
            RunsRemote = true;
            var reader = new StreamReader(path);
            var screenshotPath = reader.ReadLine();
            ScreenshotPath = screenshotPath.Replace("ScreenshotPath:", "");
            var screenshotUrl = reader.ReadLine();
            ScreenshotUrl = screenshotUrl.Replace("ScreenshotUrl:", "");
            var appendBuildNumber = reader.ReadLine();
            AppendBuildNumberToPath = Convert.ToBoolean(appendBuildNumber.Replace("AppendBuildNumberToPath:", ""));
            ChromePort = Convert.ToInt32(reader.ReadLine().Split(':')[1]);
            FirefoxPort = Convert.ToInt32(reader.ReadLine().Split(':')[1]);
            IePort = Convert.ToInt32(reader.ReadLine().Split(':')[1]);
            Machine = reader.ReadLine().Split(':')[1];
        }

        public static string ScreenshotPath { get; set; }
        public static string ScreenshotUrl { get; set; }
        public static bool AppendBuildNumberToPath { get; set; }
        public static bool RunsRemote { get; private set; }

        public static TestConfig GetTestConfig(string browserType)
        {
            var testConfig = new TestConfig {MachineName = Machine, BrowserType = browserType};
            if (!RunsRemote)
            {
                switch (browserType)
                {
                    case BrowserType.RemoteChrome:
                    case BrowserType.Chrome:
                        testConfig.BrowserType = BrowserType.Chrome;
                        return testConfig;
                    case BrowserType.RemoteFirefox:
                    case BrowserType.Firefox:
                        testConfig.BrowserType = BrowserType.Firefox;
                        return testConfig;
                    case BrowserType.RemoteIe:
                    case BrowserType.InternetExplorer32:
                        testConfig.BrowserType = BrowserType.InternetExplorer32;
                        return testConfig;                    
                }
            }

            switch (browserType)
            {
                case BrowserType.RemoteChrome:
                    testConfig.Port = Convert.ToInt32(ChromePort);
                    break;
                case BrowserType.RemoteFirefox:
                    testConfig.Port = Convert.ToInt32(FirefoxPort);
                    break;
                case BrowserType.RemoteIe:
                    testConfig.Port = Convert.ToInt32(IePort);
                    break;
            }

            CallNode(testConfig);
            return testConfig;
        }

        private static void CallNode(TestConfig testConfig)
        {
            try
            {
                if (nodeCalled)
                {
                    return;
                }
                var httpClient = new HttpClient();                
                var response = httpClient.GetStringAsync(String.Format(RequestUri, testConfig.MachineName, testConfig.Port)).Result;
                var deserializedObject = JsonConvert.DeserializeObject<SessionsRoot>(response);
                testConfig.NoOfSessions = deserializedObject.Sessions.Count;
                nodeCalled = true;
            }
            catch (Exception ex)
            {                
                Console.WriteLine(ex);
                throw;                
            }
        }

        private static string CheckAndReturnPath()
        {
            var parent = Directory.GetParent(Process.GetCurrentProcess().MainModule.FileName);
            var path = Path.Combine(parent.FullName, @"test.config");
            return File.Exists(path) ? path : string.Empty;
        }
    }

    public class SessionsRoot
    {
        [JsonProperty("value")]
        public List<Session> Sessions { get; set; }
    }

    public class Session
    {
        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; set; }
    }

    public class Capabilities
    {
        [JsonProperty("browserName")]
        public string BrowserName { get; set; }
    }
}
