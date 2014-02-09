using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;

using LUID = System.Int64;
using HANDLE = System.IntPtr;


namespace pserv4.processes
{
    public class ProcessDataObject : DataObject
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
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
        public string StartInfoArguments { get; private set; }
        public string NonpagedSystemMemorySize64 { get; private set; }
        public string PagedMemorySize64 { get; private set; }
        public string PagedSystemMemorySize64 { get; private set; }
        public string PeakPagedMemorySize64 { get; private set; }
        public string PeakVirtualMemorySize64 { get; private set; }
        public string PeakWorkingSet64 { get; private set; }
        public string PrivateMemorySize64 { get; private set; }
        public string VirtualMemorySize64 { get; private set; }
        public string WorkingSet64 { get; private set; }

        public readonly int ID;

        public void Refresh(Process p)
        {
            SetStringProperty("Path", NativeProcessFunctions.GetSafeProcessName(p));
            SetStringProperty("Name", p.ProcessName);
            SetStringProperty("User", NativeProcessFunctions.GetUserInfo(p));

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
                    SetStringProperty("FileDescription", p.MainModule.FileVersionInfo.FileDescription);
                    SetStringProperty("FileVersion", p.MainModule.FileVersionInfo.FileVersion);
                    SetStringProperty("Product", p.MainModule.FileVersionInfo.ProductName);
                    SetStringProperty("ProductVersion", p.MainModule.FileVersionInfo.ProductVersion);
                    SetStringProperty("MainWindowHandle", p.MainWindowHandle);
                    SetStringProperty("MainWindowTitle", p.MainWindowTitle);
                    SetStringProperty("Responding", p.Responding);
                    SetStringProperty("StartTime", p.StartTime.TimeOfDay);
                    SetStringProperty("TotalRunTime", DateTime.Now - p.StartTime);
                    SetStringProperty("TotalProcessorTime", p.TotalProcessorTime);
                    SetStringProperty("PrivilegedProcessorTime", p.PrivilegedProcessorTime);
                    SetStringProperty("ThreadCount", p.Threads.Count);
                    SetStringProperty("HandleCount", p.HandleCount);
                    SetStringProperty("ProcessPriorityClass", p.PriorityClass);
                    SetStringProperty("SessionId", p.SessionId);
                    SetStringProperty("StartInfoArguments", p.StartInfo.Arguments);
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
            catch (Exception)
            {
            }

            if (User.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
            {
                isDisabled = true;
            }

            SetRunning(isRunning);
            SetDisabled(isDisabled);
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
