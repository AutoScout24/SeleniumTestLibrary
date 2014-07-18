using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;

using Autoscout24.Scheduler.Models;

using log4net;

namespace Autoscout24.Scheduler
{
    public class ProcessRunner
    {        
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string resultFilePath;
        private readonly StringBuilder processOutput;
        private readonly StringBuilder processErrorOutput;
        private int processId;

        public event EventHandler<ProcessRunnerExitedEventArgs> ProcessExited;

        public ProcessRunner()
        {            
            processOutput = new StringBuilder();
            processErrorOutput = new StringBuilder();            
        }

        public void Run(ProcessInfo processInfo)
        {
            ThreadPool.QueueUserWorkItem(RunAProcess, processInfo);
        }

        private void RunAProcess(object testProcessInfoParam)
        {
            try
            {
                var testProcessInfo = (ProcessInfo) testProcessInfoParam;
                processId = testProcessInfo.ProcessId;
                resultFilePath = testProcessInfo.ResultFilePath;
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = testProcessInfo.FileName,
                        Arguments = testProcessInfo.CommandArguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        WorkingDirectory = testProcessInfo.WorkingDirectory
                    },
                    EnableRaisingEvents = true
                };

                process.OutputDataReceived += ProcessOnOutputDataReceived;
                process.ErrorDataReceived += ProcessOnErrorDataReceived;
                process.Exited += ProcessOnExited;
                Log.Info(string.Format("Starting the process for {0} {1}", testProcessInfo.ResultFilePath, testProcessInfo.ProcessId));
                process.Start();
                // Wihtout this line the process blocks when there is an error or another thread is used in execution of the test (e.g. HttpClient)
                processOutput.Append(process.StandardOutput.ReadToEnd());
                process.WaitForExit();                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Log.Error(ex);
            }    
        }

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            Log.Info(string.Format("Process on error data received for {0}", processId));
            processErrorOutput.Append(dataReceivedEventArgs.Data);
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            Log.Info(string.Format("Process on output data received for {0}", processId));
            processOutput.Append(dataReceivedEventArgs.Data);
        }

        private void ProcessOnExited(object sender, EventArgs eventArgs)
        {
            Log.Info(string.Format("Process on exited for {0}", processId));

            var theProcess = sender as Process;
            if (theProcess != null)
            {
                using (var reader = theProcess.StandardOutput)
                {
                    var result = reader.ReadToEnd();                    
                    processOutput.AppendLine(result);
                    Console.WriteLine(result);
                }
            }

            var args = new ProcessRunnerExitedEventArgs
            {
                ProcessOutput = processOutput,
                TestsResultFilePath = resultFilePath,
                ProcessId = processId
            };
            OnProcessExited(args);
        }

        protected virtual void OnProcessExited(ProcessRunnerExitedEventArgs e)
        {
            var handler = ProcessExited;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
