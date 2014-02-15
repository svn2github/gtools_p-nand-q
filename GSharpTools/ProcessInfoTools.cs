using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using log4net;

namespace GSharpTools
{
    public static class ProcessInfoTools
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;

        [StructLayout(LayoutKind.Sequential)]
        private struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        public static bool ShowFileProperties(string filename)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = filename;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            return ShellExecuteEx(ref info);
        }

        public static bool ShowTerminal(string directory)
        {
            Log.InfoFormat("ShowTerminal({0}) called", directory);
            try
            {
                if (string.IsNullOrEmpty(directory))
                {
                    Log.Warn("Warning, directory is empty - assume function failed");
                    return false;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "cmd.exe"));
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.Arguments = string.Format("/K \"cd /d {0}\"", directory);

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
                Log.Error(string.Format("Unable to bring up CMD.EXE in {0}", directory), e);
                return false;
            }
        }

        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        private static extern long FindExecutableW(
            string lpFile, string lpDirectory, StringBuilder lpResult);

        public static bool FindExecutable(string name, out string executable)
        {
            StringBuilder objResultBuffer = new StringBuilder(10240);
            long lngResult = 0;

            lngResult = FindExecutableW(name, string.Empty, objResultBuffer);

            if (lngResult >= 32)
            {
                executable = objResultBuffer.ToString();
                //Trace.TraceInformation("Mapped '{0}' to '{1}'", name, executable);
                return true;
            }
            executable = null;
            //Trace.TraceError("Error: ({0})", lngResult);
            return false;
        }

        public static bool ShowExplorer(string directory)
        {
            Log.InfoFormat("BringUpExplorer({0}) called", directory);
            try
            {
                if (string.IsNullOrEmpty(directory))
                {
                    Log.Warn("Warning, directory is empty - assume function failed");
                    return false;
                }

                string cmd = string.Format("/root,{0}", directory);
                Log.InfoFormat("CMD: {0}", cmd);

                using (Process p = Process.Start("explorer.exe", cmd))
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
                Log.Error(string.Format("Unable to bring up Explorer in directory {0}", directory), e);
                return false;
            }
        }

        public static bool ShowRegistryEditor(string key)
        {
            using (RegistryKey hkKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit", true))
            {
                hkKey.SetValue("Lastkey", key);
            }
            try
            {
                Process.Start("regedit.exe");
                return true;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Unable to bring up Registry Editor in {0}", key), e);
                return false;
            }
        }


    }
}
