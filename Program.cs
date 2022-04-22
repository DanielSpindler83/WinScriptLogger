using CommandLine;

namespace WinScriptLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(CommandLineArgs =>
                   {
                       // we accept two command line args - refer Options.cs for details
                       // --script
                       // --logfiledir

                        #if DEBUG
                           System.Console.WriteLine(CommandLineArgs.script);
                           System.Console.WriteLine(CommandLineArgs.logfiledir);
                           System.Console.WriteLine(CommandLineArgs.timeoutinsec);
                        #endif


                       var runBatchScriptTask = new RunBatchScript(CommandLineArgs.script,CommandLineArgs.logfiledir, CommandLineArgs.timeoutinsec);


                   }); // END of command line args use

        } // main END

    } // class END
    
} // namepsace END
