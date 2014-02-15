using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;
using pserv4.Properties;
using Microsoft.Win32;
using log4net;
using System.Reflection;
using GSharpTools;

using LUID = System.Int64;
using HANDLE = System.IntPtr;


namespace pserv4.processes
{
    public class ProcessDataObject : DataObject
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string Name { get; private set; }
        public string MainExecutable { get; private set; }
        public string User { get; private set; }
        public string FileDescription { get; private set; }
        public string FileVersion { get; private set; }
        public string Product { get; private set; }
        public string ProductVersion { get; private set; }
        public string ThreadCount { get; private set; }
        public string HandleCount { get; private set; }
        public string MainWindowHandle { get; private set; }
        public string MainWindowTitle { get; private set; }
        public string Responding { get; private set; }
        public string StartTime { get; private set; }
        public string TotalRunTime { get; private set; }
        public string TotalProcessorTime { get; private set; }
        public string PrivilegedProcessorTime { get; private set; }
        public string ProcessPriorityClass { get; private set; }
        public string SessionId { get; private set; }
        public string NonpagedSystemMemorySize64 { get; private set; }
        public string PagedMemorySize64 { get; private set; }
        public string PagedSystemMemorySize64 { get; private set; }
        public string PeakPagedMemorySize64 { get; private set; }
        public string PeakVirtualMemorySize64 { get; private set; }
        public string PeakWorkingSet64 { get; private set; }
        public string PrivateMemorySize64 { get; private set; }
        public string VirtualMemorySize64 { get; private set; }
        public string WorkingSet64 { get; private set; }

        public string CommandLine { get; private set; }
        
        public readonly int ID;

        private static Dictionary<string, string> KnownSystemServices;

        public override string FileName
        {
            get
            {
                return MainExecutable;
            }
        }

        public void Refresh(Process p)
        {
            if( KnownSystemServices == null )
            {
                KnownSystemServices = new Dictionary<string, string>() {
                    { "smss", Resources.IDS_PROCESS_SMSS},
                    { "svchost", Resources.IDS_PROCESS_SVCHOST },
                    { "services", Resources.IDS_PROCESS_SERVICES },
                    { "csrss", Resources.IDS_PROCESS_CSRSS }
                };
            }

            bool isSystemProcess = false;
            if (KnownSystemServices.ContainsKey(p.ProcessName))
            {
                if (!ConstructionIsFinished)
                {
                    SetStringProperty("MainExecutable", KnownSystemServices[p.ProcessName]);
                    SetStringProperty("ToolTip", KnownSystemServices[p.ProcessName]);
                    SetStringProperty("User", "SYSTEM");
                }                
                isSystemProcess = true;
            }
            else if (SetStringProperty("MainExecutable", NativeProcessFunctions.GetSafeProcessName(p)))
            {
                NotifyPropertyChanged("InstallLocation");
                ToolTip = MainExecutable;
                NotifyPropertyChanged("ToolTip");
            }
            SetStringProperty("Name", p.ProcessName);

            if( !isSystemProcess)
            {
                SetStringProperty("User", NativeProcessFunctions.GetUserInfo(p));
            }
            

            bool isRunning = false;
            bool isDisabled = false;

            if (Environment.UserName.Equals(User))
            {
                isRunning = true;
            }

            try
            {
                if (p.Id >= 10)
                {
                    if (!ConstructionIsFinished)
                    {
                        if( !isSystemProcess)
                        {
                            FileVersionInfoCache.CacheInfo ci = FileVersionInfoCache.Get(MainExecutable, p.MainModule);
                            if (ci != null)
                            {
                                SetStringProperty("FileDescription", ci.FileDescription);
                                SetStringProperty("FileVersion", ci.FileVersion);
                                SetStringProperty("Product", ci.ProductName);
                                SetStringProperty("ProductVersion", ci.ProductVersion);
                            }
                        }
                        ProcessInfoCache.Data pi = ProcessInfoCache.Get(p);
                        if (pi != null)
                        {
                            SetStringProperty("CommandLine", pi.CommandLine);
                            SetStringProperty("FileDescription", pi.CommandLine);
                        }
                        SetStringProperty("StartTime", p.StartTime.TimeOfDay);
                    }
                    SetStringProperty("MainWindowHandle", p.MainWindowHandle);
                    SetStringProperty("MainWindowTitle", p.MainWindowTitle);
                    SetStringProperty("Responding", p.Responding);
                    
                    SetStringProperty("TotalRunTime", DateTime.Now - p.StartTime);
                    SetStringProperty("TotalProcessorTime", p.TotalProcessorTime);
                    SetStringProperty("PrivilegedProcessorTime", p.PrivilegedProcessorTime);
                    SetStringProperty("ThreadCount", p.Threads.Count);
                    SetStringProperty("HandleCount", p.HandleCount);
                    if (!isSystemProcess)
                    SetStringProperty("ProcessPriorityClass", p.PriorityClass);
                    SetStringProperty("SessionId", p.SessionId);
                    SetStringProperty("NonpagedSystemMemorySize64", Localisation.BytesToSize(p.NonpagedSystemMemorySize64));
                    SetStringProperty("PagedMemorySize64", Localisation.BytesToSize(p.PagedMemorySize64));
                    SetStringProperty("PagedSystemMemorySize64", Localisation.BytesToSize(p.PagedSystemMemorySize64));
                    SetStringProperty("PeakPagedMemorySize64", Localisation.BytesToSize(p.PeakPagedMemorySize64));
                    SetStringProperty("PeakVirtualMemorySize64", Localisation.BytesToSize(p.PeakVirtualMemorySize64));
                    SetStringProperty("PeakWorkingSet64", Localisation.BytesToSize(p.PeakWorkingSet64));
                    SetStringProperty("PrivateMemorySize64", Localisation.BytesToSize(p.PrivateMemorySize64));
                    SetStringProperty("VirtualMemorySize64", Localisation.BytesToSize(p.VirtualMemorySize64));
                    SetStringProperty("WorkingSet64", Localisation.BytesToSize(p.WorkingSet64));
                }
                else
                {
                    isDisabled = true;
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("problem decoding process {0}", MainExecutable), e);
            }

            if (User.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
            {
                isDisabled = true;
            }
            string toolTipCaption = FileDescription;
            if (string.IsNullOrEmpty(toolTipCaption))
                toolTipCaption = Name;
            if( !toolTipCaption.Equals(ToolTipCaption))
            {
                ToolTipCaption = toolTipCaption;
                NotifyPropertyChanged("ToolTipCaption");
            }


            SetRunning(isRunning);
            SetDisabled(isDisabled);
        }

        public string InstallLocation
        {
            get
            {
                return PathSanitizer.GetDirectory(MainExecutable);
            }
        }

        public bool BringUpExplorerInInstallLocation()
        {
            return ProcessInfoTools.ShowExplorer(InstallLocation);
        }

        public bool BringUpTerminalInInstallLocation()
        {
            return ProcessInfoTools.ShowTerminal(InstallLocation);
        }


        public ProcessDataObject(Process p)
            :   base(p.Id.ToString())
        {
            ID = p.Id;
            Refresh(p);
            ConstructionIsFinished = true;
        }
    }

}
