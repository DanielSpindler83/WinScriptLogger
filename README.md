# WinScriptLogger
Log standard &amp; error output for scripts run on Windows.

Currently working for batch scripts.  
Soon to add support for PowerShell.

Wrapper around scripts, so that they can be run through automation and log the full standard and error output to a log file.
The idea is to see the output as though the script was run from a terminal.

WinScriptLogger 1.0.0.0    
USAGE:  
Run a script and log the full output to a file:  
  WinScriptLogger.exe --logfiledir C:\logs --script C:\temp\myscript.bat --timeoutinsec 1800  
  
  --script          Required. Full path of script to run.
  
  --logfiledir      Default log file output is same directory as script. Here this can be changed to a different path.
  
  --timeoutinsec    (Default: 0) Default is no timeout. Timeout in seconds. Once elapsed, script execution considered hung and then terminated.
  
  --help            Display this help screen.
  
  --version         Display version information.

