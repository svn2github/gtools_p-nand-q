using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using pserv4.Properties;

namespace pserv4
{
    public static class ProcessExtensions
    {
        public static bool KillProcessById(int id)
        {
            try
            {
                foreach (Process myProc in Process.GetProcesses())
                {
                    if (myProc.Id == id)
                    {
                        myProc.Kill();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceInformation(e.ToString());
            }
            return false;
        }

        public static bool DebugProcessById(int id)
        {
            using (RegistryKey hkKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AeDebug", false))
            {
                string debugger = hkKey.GetValue("Debugger") as string;
                if (string.IsNullOrEmpty(debugger))
                    return false;

                // problem: %ld is not a recognized format sequence in C#, so:

                int k = debugger.IndexOf("%ld");
                if (k >= 0)
                {
                    debugger = debugger.Substring(0, k) + id.ToString() + debugger.Substring(k + 3);
                }
                k = debugger.IndexOf("%ld");
                if (k >= 0)
                {
                    debugger = debugger.Substring(0, k) + "0" + debugger.Substring(k + 3);
                }


                string cmdLine = debugger;

                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(PathSanitizer.GetExecutable(cmdLine));
                    startInfo.WorkingDirectory = Environment.CurrentDirectory;
                    startInfo.Arguments = PathSanitizer.GetArguments(cmdLine);

                    using (Process p = Process.Start(startInfo))
                    {
                        if (p == null)
                        {
                            Trace.TraceWarning("Warning, Process.Start() returned null, assuming function failed");
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Trace.TraceError("Exception {0}: unable to bring up debugger", e);
                    Trace.TraceWarning(e.StackTrace);
                    return false;
                }
            }
        }
    }
}
