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
    public class ProcessDataObject : IObject
    {
        public string IDString
        { 
            get
            {
                return ID.ToString();
            }
        }
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


        public bool IsDisabled { get; private set; }
        public bool IsRunning { get; private set; }

        public readonly int ID;

        public ProcessDataObject(Process p)
        {
            ID = p.Id;
            Path = NativeProcessFunctions.GetSafeProcessName(p);
            Name = p.ProcessName;
            User = NativeProcessFunctions.GetUserInfo(p);
            if (Environment.UserName.Equals(User))
            {
                IsRunning = true;
            }

            try
            {
                if (p.Id >= 10)
                {
                    FileDescription = p.MainModule.FileVersionInfo.FileDescription;
                    
                    
                    FileVersion = p.MainModule.FileVersionInfo.FileVersion;
                    Product = p.MainModule.FileVersionInfo.ProductName;
                    ProductVersion = p.MainModule.FileVersionInfo.ProductVersion;
                    MainWindowHandle = p.MainWindowHandle.ToString();
                    MainWindowTitle = p.MainWindowTitle;
                    Responding = p.Responding.ToString();

                    StartTime = p.StartTime.TimeOfDay.ToString();
                    TotalRunTime = (DateTime.Now - p.StartTime).ToString();
                    TotalProcessorTime = p.TotalProcessorTime.ToString();

                    PrivilegedProcessorTime = p.PrivilegedProcessorTime.ToString();

                    ThreadCount = p.Threads.Count.ToString();
                    HandleCount = p.HandleCount.ToString();

                    ProcessPriorityClass = p.PriorityClass.ToString();
                    SessionId = p.SessionId.ToString();
                    StartInfoArguments = p.StartInfo.Arguments;


                    NonpagedSystemMemorySize64 = Localisation.BytesToSize(p.NonpagedSystemMemorySize64);
                    PagedMemorySize64 = Localisation.BytesToSize(p.PagedMemorySize64);
                    PagedSystemMemorySize64 = Localisation.BytesToSize(p.PagedSystemMemorySize64);
                    PeakPagedMemorySize64 = Localisation.BytesToSize(p.PeakPagedMemorySize64);
                    PeakVirtualMemorySize64 = Localisation.BytesToSize(p.PeakVirtualMemorySize64);
                    PeakWorkingSet64 = Localisation.BytesToSize(p.PeakWorkingSet64);
                    PrivateMemorySize64 = Localisation.BytesToSize(p.PrivateMemorySize64);
                    VirtualMemorySize64 = Localisation.BytesToSize(p.VirtualMemorySize64);
                    WorkingSet64 = Localisation.BytesToSize(p.WorkingSet64);
                }
                else
                {
                    IsDisabled = true;
                }
            }
            catch (Exception)
            {
            }

            if (User.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase) )
            {
                IsDisabled = true;
            }
                        
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

}
