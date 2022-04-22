using log4net.Appender;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace WinScriptLogger
{
    public class RunBatchScript
    {
        private string Script { get; set; }
        private string LogFileDir { get; set; }
        private int TimeOutInSec { get; set; }

        private static readonly log4net.ILog batchlog = log4net.LogManager.GetLogger("BatchLog");

        public RunBatchScript(string script, string logfiledir, int timeoutinsec)
        {
            this.Script = script;
            this.LogFileDir = logfiledir;
            this.TimeOutInSec = timeoutinsec;

            SetupScriptLogger();

            RunCommand();
        }


        private void RunCommand()
        {
            using (var p = new Process())
            {
                // notice that we're using the Windows shell here and the unix-y 2>&1
                // https://stackoverflow.com/questions/18529662/capture-process-stdout-and-stderr-in-the-correct-ordering
                p.StartInfo.FileName = @"c:\windows\system32\cmd.exe";
                p.StartInfo.Arguments = "/c \"" + Script + " " + "\" 2>&1";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                var output = new StringBuilder();

                using (var outputWaitHandle = new AutoResetEvent(false))
                {
                    p.OutputDataReceived += (sender, e) =>
                    {
                        // attach event handler
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            // here we simply pipe each output line to batchlog
                            batchlog.Info(e.Data);
                        }
                    };

                    // start process
                    p.Start();

                    // begin async read
                    p.BeginOutputReadLine();

                    // timeoutinsec wasnt set from command line so lets wait indefinitely
                    if (TimeOutInSec == 0) 
                    {
                        p.WaitForExit();
                    } 
                    else 
                    {
                        // only wait the timeout period then assume batch is hung
                        if (!p.WaitForExit(TimeOutInSec * 1000)) // if the process has NOT exited before the timeout then..* 1000 to convert to milliseconds
                        {
                            p.Kill();
                            batchlog.Error("Script File has reached the timeout set and failed to complete. Terminating process") ;
                            throw new Exception("Script File has reached the timeout set and failed to complete. Terminating process");
                        }
                    }
                } // end of wait handle using

            } // end of process using 

        } // end of RunCommand


        private void SetupScriptLogger()
        {
            DateTime now = DateTime.Now;

            // Load our BatchLog log appender by name - so we can manipulate it
            var appender = log4net.LogManager.GetRepository()
                             .GetAppenders()
                             .OfType<FileAppender>()
                             .FirstOrDefault(fa => fa.Name == "BatchLog");

            if (LogFileDir == null)
            {
                // Set new filename for our log appender
                appender.File = Path.ChangeExtension(Script, null) + now.ToString("_yyyy-MM-dd_T-HH-mm-ss") + ".log";
            }
            else
            {
                if (Path.IsPathRooted(LogFileDir))
                {
                    if (LogFileDir[LogFileDir.Length - 1] == '\\')
                    {
                        // Set new filename for our log appender
                        appender.File = LogFileDir + Path.GetFileNameWithoutExtension(Script) + now.ToString("_yyyy-MM-dd_T-HH-mm-ss") + ".log";
                    } else
                    {
                        // Set new filename for our log appender
                        appender.File = LogFileDir + "\\" + Path.GetFileNameWithoutExtension(Script) + now.ToString("_yyyy-MM-dd_T-HH-mm-ss") + ".log";
                    }
                }
                else
                {
                    // logfiledir provided was bad - fall back to default
                    batchlog.Warn("logfiledir provided is not an absolute root path. logfiledir = " + LogFileDir);
                    // Set new filename for our log appender
                    appender.File = Path.ChangeExtension(Script, null) + now.ToString("_yyyy-MM-dd_T-HH-mm-ss") + ".log";
                }
            }

            appender.ActivateOptions(); // update appender with new options

            // log the location of the current batchscript.
            batchlog.Info("Batch Script Log: " + appender.File);


            //delete templog files that build up over time -we dont use them - they are part of the log4net logger used for batchlog
            var deleteTargetPath = GetFiles(AppDomain.CurrentDomain.BaseDirectory, "templog.xml.*", SearchOption.TopDirectoryOnly);
            foreach (var deleteFilePath in deleteTargetPath)
                {
                    File.Delete(deleteFilePath);
                }


        } // end of SetupBatchScriptLogging

        /*** Better file path search for files ***/
        private static string[] GetFiles(string sourceFolder, string filters, System.IO.SearchOption searchOption)
        {
            return filters.Split('|').SelectMany(filter => System.IO.Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
        }


    }
}
