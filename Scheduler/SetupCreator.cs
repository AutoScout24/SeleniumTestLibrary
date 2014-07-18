using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Autoscout24.Scheduler.Models;

namespace Autoscout24.Scheduler
{
    public class SetupCreator
    {
        private readonly string screenshotPath;
        private readonly string screenshotUrl;
        private readonly string appendBuildNumberToPath;
        private List<string> fileList;

        public SetupCreator(TestRunSettings testRunSettings)
        {
            screenshotPath = testRunSettings.ScreenshotPath;
            screenshotUrl = testRunSettings.ScreenshotUrl;
            appendBuildNumberToPath = testRunSettings.AppendBuildNumberToPath.ToString();
            fileList = ReadFilesList(testRunSettings.FilesListPath);
        }

        public void SetupFor(int processId, string testAssemblyPath, NodeConfig nodeConfig)
        {
            CopyFiles(processId, testAssemblyPath);
            WriteTestConfigFile(nodeConfig, processId);
        }

        private void CopyFiles(int processId, string testAssemblyPath)
        {
            var tempFolderPath = Path.Combine(Environment.CurrentDirectory, processId.ToString(CultureInfo.InvariantCulture));
            Directory.CreateDirectory(tempFolderPath);
            var copiedAssemblyPath = Path.Combine(tempFolderPath, Path.GetFileName(testAssemblyPath));
            File.Copy(testAssemblyPath, copiedAssemblyPath, true);
            foreach (var file in fileList)
            {
                try
                {
                    var destination = Path.Combine(tempFolderPath, Path.GetFileName(file));
                    File.Copy(file, destination, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);                    
                }                
            }
        }

        private void WriteTestConfigFile(NodeConfig nodeConfig, int processId)
        {
            var path = Path.Combine(Environment.CurrentDirectory, processId.ToString(CultureInfo.InvariantCulture), "test.config");
            var streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(@"ScreenshotPath:{0}", screenshotPath);
            streamWriter.WriteLine(@"ScreenshotUrl:{0}", screenshotUrl);
            streamWriter.WriteLine(@"AppendBuildNumberToPath:{0}", appendBuildNumberToPath);
            streamWriter.WriteLine(@"ChromeClientPort:{0}", nodeConfig.ChromeNode.Port);
            streamWriter.WriteLine(@"FirefoxClientPort:{0}", nodeConfig.FirefoxNode.Port);
            streamWriter.WriteLine(@"IeClientPort:{0}", nodeConfig.IeNode.Port);
            streamWriter.WriteLine(@"Machine:{0}", nodeConfig.ChromeNode.Machine);
            streamWriter.Close();
        }

        private List<string> ReadFilesList(string filesListPath)
        {
            if (!filesListPath.Contains("\\"))
            {
                filesListPath = Path.Combine(Environment.CurrentDirectory, filesListPath);
            }
            if (!File.Exists(filesListPath))
            {
                throw new ArgumentException(string.Format("Unable to find file list file with path: {0}", filesListPath));
            }

            fileList = new List<string>();
            var reader = new StreamReader(filesListPath);
            while (!reader.EndOfStream)
            {
                fileList.Add(Path.Combine(Environment.CurrentDirectory, reader.ReadLine()));
            }
            reader.Close();
            return fileList;
        }
    }
}
