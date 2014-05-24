using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Collections.ObjectModel;
using pserv4.Properties;
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
using log4net;
using System.Reflection;

namespace pserv4.modules
{
    public class ModulesDataController : DataController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ModulesDataController()
            :   base(
                    "Modules", 
                    "Module",
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
                        new DataObjectColumn(Resources.MODULE_C_ProcessID, "ProcessID"),
                        new DataObjectColumn(Resources.MODULE_C_Name, "Name"),
                        new DataObjectColumn(Resources.MODULE_C_Path, "Path"),
                        new DataObjectColumn(Resources.MODULE_C_ModuleMemorySize, "ModuleMemorySize"),
                        new DataObjectColumn(Resources.MODULE_C_FileDescription, "FileDescription"),
                        new DataObjectColumn(Resources.MODULE_C_FileVersion, "FileVersion"),
                        new DataObjectColumn(Resources.MODULE_C_Product, "Product"),
                        new DataObjectColumn(Resources.MODULE_C_ProductVersion, "ProductVersion"));
                }
                return ActualColumns;
            }
        }

        private delegate bool ModuleCallback(ModuleDataObject mdo);

        private void DispatchCallback(ModuleCallback callback)
        {
            foreach (ModuleDataObject mdo in MainListView.SelectedItems)
            {
                callback(mdo);
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

        public override void Refresh(ObservableCollection<DataObject> objects)
        {
            using (var manager = new RefreshManager<ModuleDataObject>(objects))
            {
                foreach (Process p in Process.GetProcesses())
                {
                    bool isDisabled = false;
                    if (p.Id < 10)
                    {
                        continue;
                    }
                    else if (p.ProcessName.Equals("smss") ||
                        p.ProcessName.Equals("svchost") ||
                        p.ProcessName.Equals("services") ||
                        p.ProcessName.Equals("csrss"))
                    {
                        continue;
                    }
                    else
                    {
                        string username = pserv4.processes.NativeProcessFunctions.GetUserInfo(p);
                        if (username.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
                        {
                            isDisabled = true;
                        }
                    }
                    ProcessModuleCollection pmc = null;
                    try
                    {
                        pmc = p.Modules;
                    }
                    catch(Exception e)
                    {
                        Log.Error(string.Format("Exception caught while accessing modules of process {0}", p), e);
                    }
                    if( pmc != null )
                    {
                        foreach (ProcessModule m in pmc)
                        {
                            string ID = string.Format("{0}.{1}", p.Id, m.FileName);

                            ModuleDataObject mdo;
                            if (manager.Contains(ID, out mdo))
                            {
                                mdo.Refresh(p, m, isDisabled);
                            }
                            else
                            {
                                objects.Add(new ModuleDataObject(p, m, isDisabled));
                            }
                        }
                    }
                }
            }
        }
    }
}
