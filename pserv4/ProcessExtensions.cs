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
using log4net;
using System.Reflection;
using GSharpTools;

namespace pserv4
{
    public static class ProcessExtensions
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
                Log.Error(string.Format("KillProcessById({0}) failed", id), e);
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
                            Log.Warn("Warning, Process.Start() returned null, assuming function failed");
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("unable to bring up debugger: {0}",cmdLine), e);
                    return false;
                }
            }
        }
    }
}
