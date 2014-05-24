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

namespace pserv4.processes
{
    public class ProcessesDataController : DataController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
                    CreateColumns(
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_ID, "InternalID"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_Name, "Name"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_MainExecutable, "MainExecutable"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_CommandLine, "CommandLine"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_User, "User"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_FileDescription, "FileDescription"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_FileVersion, "FileVersion"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_Product, "Product"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_ProductVersion, "ProductVersion"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_ThreadCount, "ThreadCount"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_HandleCount, "HandleCount"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_MainWindowHandle, "MainWindowHandle"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_MainWindowTitle, "MainWindowTitle"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_Responding, "Responding"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_StartTime, "StartTime"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_TotalProcessorTime, "TotalProcessorTime"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_TotalRunTime, "TotalRunTime"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PrivilegedProcessorTime, "PrivilegedProcessorTime"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_ProcessPriorityClass, "ProcessPriorityClass"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_SessionId, "SessionId"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_NonpagedSystemMemorySize64, "NonpagedSystemMemorySize64"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PagedMemorySize64, "PagedMemorySize64"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PeakPagedMemorySize64, "PeakPagedMemorySize64"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PagedSystemMemorySize64, "PagedSystemMemorySize64"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PeakVirtualMemorySize64, "PeakVirtualMemorySize64"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PeakWorkingSet64, "PeakWorkingSet64"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_PrivateMemorySize64, "PrivateMemorySize64"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_VirtualMemorySize64, "VirtualMemorySize64"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_WorkingSet64, "WorkingSet64"),
                        new DataObjectColumn(pserv4.Properties.Resources.PROCESS_C_InstallLocation, "InstallLocation"));
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
                    try
                    {
                        if (manager.Contains(p.Id.ToString(), out pdo))
                        {
                            pdo.Refresh(p);
                        }
                        else
                        {
                            objects.Add(new ProcessDataObject(p));
                        }
                    }
                    catch(Exception e)
                    {
                        Log.Error(string.Format("Exception caught while analysing process {0}", p), e);
                    }
                }
            }
        }

        private delegate bool ProcessCallback(ProcessDataObject udo);

        private void DispatchCallback(ProcessCallback callback)
        {
            foreach (ProcessDataObject pdo in MainListView.SelectedItems)
            {
                callback(pdo);
            }
        }

        public void OnBringUpExplorer(object sender, RoutedEventArgs e)
        {
            DispatchCallback((pdo) => { return pdo.BringUpExplorerInInstallLocation(); });
        }

        public void OnBringUpTerminal(object sender, RoutedEventArgs e)
        {
            DispatchCallback((pdo) => { return pdo.BringUpTerminalInInstallLocation(); });
        }

        public void OnKillProcess(object sender, RoutedEventArgs e)
        {
            DispatchCallback((pdo) => { return ProcessExtensions.KillProcessById(pdo.ID); });
        }

        public void OnDebugProcess(object sender, RoutedEventArgs e)
        {
            DispatchCallback((pdo) => { return ProcessExtensions.DebugProcessById(pdo.ID); });
        }

        public override ContextMenu ContextMenu
        {
            get
            {
                ContextMenu menu = new ContextMenu(); // base.ContextMenu;

                AppendMenuItem(menu, Resources.PROCESS_BRING_UP_EXPLORER, "folder_find", OnBringUpExplorer);
                AppendMenuItem(menu, Resources.PROCESS_START_CMD, "application_xp_terminal", OnBringUpTerminal);
                menu.Items.Add(new Separator());
                AppendMenuItem(menu, Resources.PROCESS_DEBUG_PROCESS, "application_lightning", OnDebugProcess);
                menu.Items.Add(new Separator());
                AppendMenuItem(menu, Resources.PROCESS_KILL_PROCESS, "application_form_delete", OnKillProcess);
                return AppendDefaultItems(menu);
            }
        }
    }
}
