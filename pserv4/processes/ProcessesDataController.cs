using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace pserv4.processes
{
    public class ProcessesDataController : DataController
    {
        private static List<DataObjectColumn> ActualColumns;

        public ProcessesDataController()
            :   base(
                    "Processes", 
                    "Process",
                    "",
                    "",
                    "",
                    "",
                    "")
        {
        }

        public override IEnumerable<DataObjectColumn> Columns
        {
            get
            {
                if (ActualColumns == null)
                {
                    ActualColumns = new List<DataObjectColumn>();
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_ID, "InternalID"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_Name, "Name"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_Path, "Path"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_User, "User"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_FileDescription, "FileDescription"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_FileVersion, "FileVersion"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_Product, "Product"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_ProductVersion, "ProductVersion"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_ThreadCount, "ThreadCount"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_HandleCount, "HandleCount"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_MainWindowHandle, "MainWindowHandle"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_MainWindowTitle, "MainWindowTitle"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_Responding, "Responding"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_StartTime, "StartTime"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_TotalProcessorTime, "TotalProcessorTime"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_TotalRunTime, "TotalRunTime"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PrivilegedProcessorTime, "PrivilegedProcessorTime"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_ProcessPriorityClass, "ProcessPriorityClass"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_SessionId, "SessionId"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_StartInfoArguments, "StartInfoArguments"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_NonpagedSystemMemorySize64, "NonpagedSystemMemorySize64"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PagedMemorySize64, "PagedMemorySize64"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PeakPagedMemorySize64, "PeakPagedMemorySize64"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PagedSystemMemorySize64, "PagedSystemMemorySize64"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PeakVirtualMemorySize64, "PeakVirtualMemorySize64"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PeakWorkingSet64, "PeakWorkingSet64"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PrivateMemorySize64, "PrivateMemorySize64"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_VirtualMemorySize64, "VirtualMemorySize64"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_WorkingSet64, "WorkingSet64"));
                }
                return ActualColumns;
            }
        }

        public override void Refresh(ObservableCollection<DataObject> objects)
        {
            using (var manager = new RefreshManager<ProcessDataObject>(objects))
            {
                foreach (Process p in Process.GetProcesses())
                {
                    ProcessDataObject pdo = null;

                    if (manager.Contains(p.Id.ToString(), out pdo))
                    {
                        pdo.Refresh(p);
                    }
                    else
                    {
                        objects.Add(new ProcessDataObject(p));
                    }
                }
            }
        }
    }
}
