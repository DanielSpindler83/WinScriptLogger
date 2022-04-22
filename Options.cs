using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace WinScriptLogger
{
    internal class Options
    {
        [Option("script", Required = true, HelpText = "Full path of script to run.")]
        public string script { get; set; }

        [Option("logfiledir", Default = null, HelpText = "Default log file output is same directory as script. Here this can be changed to a different path.")]
        public string logfiledir { get; set; }

        [Option("timeoutinsec", Default = 0, HelpText = "Default is no timeout. Timeout in seconds. Once elapsed, script execution considered hung and then terminated.")]
        public int timeoutinsec { get; set; }

        [Usage(ApplicationAlias = "WinScriptLogger.exe")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                // for some weird reason the options are output in alphabetical order - i want script to be show in the example first......not that it really matters
                return new List<Example>() {
                new Example("Run a script and log the full output to a file.", new Options { script = @"C:\temp\myscript.bat", logfiledir = @"C:\logs", timeoutinsec = 1800 })
                };
            }
        }
    }
}