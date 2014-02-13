using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Collections.ObjectModel;
using pserv4.Properties;
using System.Diagnostics;

namespace pserv4.uninstaller
{
    public class UninstallerDataController : DataController
    {
        private static List<DataObjectColumn> ActualColumns;

        public UninstallerDataController()
            : base(
                "Uninstaller", 
                "Program",
                Resources.UNINSTALLER_CTRL_START_Description,
                Resources.UNINSTALLER_CTRL_STOP_Description,
                Resources.UNINSTALLER_CTRL_RESTART_Description,
                Resources.UNINSTALLER_CTRL_PAUSE_Description,
                Resources.UNINSTALLER_CTRL_CONTINUE_Description)
        {
            HasProperties = true;
        }


        private bool IsInstallDirectoryDefined = false;

        private delegate bool UninstallerCallback(UninstallerDataObject udo);

        private void DispatchCallback(UninstallerCallback callback)
        {
            foreach (UninstallerDataObject udo in MainListView.SelectedItems)
            {
                callback(udo);
            }
        }

        public override void OnControlStart(object sender, RoutedEventArgs e)
        {
            DispatchCallback((udo) => { return udo.PerformUninstallAction(); });
        }

        public override void OnControlStop(object sender, RoutedEventArgs e)
        {
            DispatchCallback((udo) => { return udo.PerformModifyAction(); });
        }

        public override void OnControlRestart(object sender, RoutedEventArgs e)
        {
            DispatchCallback((udo) => { return udo.ShowRegistryEditor(); });
        }

        public override void OnControlPause(object sender, RoutedEventArgs e)
        {
            DispatchCallback((udo) => { return udo.ShowHelpLink(); });
        }

        public override void OnControlContinue(object sender, RoutedEventArgs e)
        {
            DispatchCallback((udo) => { return udo.ShowAboutLink(); });
        }

        public void OnBringUpExplorer(object sender, RoutedEventArgs e)
        {
            DispatchCallback((udo) => { return udo.BringUpExplorerInInstallLocation(); });
        }

        public void OnBringUpTerminal(object sender, RoutedEventArgs e)
        {
            DispatchCallback((udo) => { return udo.BringUpTerminalInInstallLocation(); });
        }


        public override IEnumerable<DataObjectColumn> Columns
        {
            get
            {
                if (ActualColumns == null)
                {
                    ActualColumns = new List<DataObjectColumn>();
                    ActualColumns.Add(new DataObjectColumn(Resources.UNINSTALLER_C_ApplicationName, "ApplicationName"));
                    ActualColumns.Add(new DataObjectColumn(Resources.UNINSTALLER_C_InstallLocation, "InstallLocation"));
                    ActualColumns.Add(new DataObjectColumn(Resources.UNINSTALLER_C_Key, "InternalID"));
                    ActualColumns.Add(new DataObjectColumn(Resources.UNINSTALLER_C_Version, "Version"));
                    ActualColumns.Add(new DataObjectColumn(Resources.UNINSTALLER_C_Publisher, "Publisher"));
                    ActualColumns.Add(new DataObjectColumn(Resources.UNINSTALLER_C_HelpLink, "HelpLink"));
                    ActualColumns.Add(new DataObjectColumn(Resources.UNINSTALLER_C_AboutLink, "AboutLink"));
                    ActualColumns.Add(new DataObjectColumn(Resources.UNINSTALLER_C_ModifyPath, "ModifyPath"));
                    ActualColumns.Add(new DataObjectColumn(Resources.UNINSTALLER_C_Action, "Action"));
                }
                return ActualColumns;
            }
        }

        public override UserControl CreateDetailsPage(DataObject o)
        {
            return new UninstallerProperties(o as UninstallerDataObject);
        }

        public override ContextMenu ContextMenu
        {
            get
            {
                ContextMenu menu = base.ContextMenu;

                menu.Items.Add(new Separator());
                AppendMenuItem(menu, Resources.UNINSTALLER_BRING_UP_REGEDIT, "report_go", OnControlRestart);
                AppendMenuItem(menu, Resources.UNINSTALLER_BRING_UP_EXPLORER, "folder_find", OnBringUpExplorer);
                AppendMenuItem(menu, Resources.UNINSTALLER_START_CMD, "application_xp_terminal", OnBringUpTerminal);
                menu.Items.Add(new Separator());
                AppendMenuItem(menu, Resources.UNINSTALLER_FORCE_UNINSTALL, "delete", OnRemoveFromRegistry);
                return AppendDefaultItems(menu);
            }
        }

        public override void OnSelectionChanged(IList selectedItems)
        {
            IsControlStartEnabled = false; // action
            IsControlStopEnabled = false; // modify link
            IsControlRestartEnabled = true; // registry 
            IsControlPauseEnabled = false; // help link
            IsControlContinueEnabled = false; // about link
            IsInstallDirectoryDefined = false;

            foreach (UninstallerDataObject udo in selectedItems)
            {
                if (!string.IsNullOrEmpty(udo.Action))
                    IsControlStartEnabled = true;

                if (!string.IsNullOrEmpty(udo.ModifyPath))
                    IsControlStopEnabled = true;

                if (!string.IsNullOrEmpty(udo.HelpLink))
                    IsControlPauseEnabled = true;

                if (!string.IsNullOrEmpty(udo.AboutLink))
                    IsControlContinueEnabled = true;

                if (!string.IsNullOrEmpty(udo.InstallLocation))
                    IsInstallDirectoryDefined = true;
            }
        }

        public override void OnContextMenuOpening(IList selectedItems, ContextMenu menu)
        {
            SetMenuItemEnabled(menu, 0, IsControlStartEnabled);
            SetMenuItemEnabled(menu, 1, IsControlStopEnabled);
            SetMenuItemEnabled(menu, 2, IsControlRestartEnabled);
            SetMenuItemEnabled(menu, 3, IsControlPauseEnabled);
            SetMenuItemEnabled(menu, 4, IsControlContinueEnabled);

            SetMenuItemEnabled(menu, 7, IsInstallDirectoryDefined);
            SetMenuItemEnabled(menu, 8, IsInstallDirectoryDefined);
        }

        public const string UNINSTALLER_SECTION = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

        public override void Refresh(ObservableCollection<DataObject> objects)
        {
            using (var manager = new RefreshManager<UninstallerDataObject>(objects))
            {
                RefreshEntries(manager, Registry.LocalMachine, UNINSTALLER_SECTION);
                RefreshEntries(manager, Registry.CurrentUser, UNINSTALLER_SECTION);
            }
        }

        public override void ApplyChanges(List<IDataObjectDetails> changedItems)
        {
            using (new WaitCursor())
            {
                foreach (IDataObjectDetails dod in changedItems)
                {
                    dod.ApplyChanges(this);
                }
            }
        }

        public void OnRemoveFromRegistry(object sender, RoutedEventArgs e)
        {
            List<UninstallerDataObject> deleteThese = new List<UninstallerDataObject>();
            foreach (UninstallerDataObject udo in MainListView.SelectedItems)
            {
                deleteThese.Add(udo);
            }

            foreach(UninstallerDataObject udo in deleteThese)
            {
                MessageBoxResult result = MessageBox.Show(
                    string.Format(Resources.IDS_UNINSTALLER_SureToRemoveKey, udo.ApplicationName),
                    Resources.IDS_CONFIRMATION,
                    MessageBoxButton.YesNoCancel, 
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (udo.RemoveFromRegistry())
                    {
                        MainWindow.Items.Remove(udo);
                    }
                }
                else if( result == MessageBoxResult.Cancel )
                {
                    break;
                }
            }
        }

        private void RefreshEntries(RefreshManager<UninstallerDataObject> manager, RegistryKey rootKey, string keyName)
        {
            using(RegistryKey hkKey = rootKey.OpenSubKey(keyName,false))
            {
                foreach (string subKeyName in hkKey.GetSubKeyNames())
                {
                    string keyPath = string.Format("{0}\\{1}", keyName, subKeyName);
                    UninstallerDataObject mdo;
                    try
                    {
                        if (manager.Contains(subKeyName, out mdo))
                        {
                            mdo.Refresh(rootKey, keyPath, subKeyName);
                        }
                        else
                        {
                            mdo = new UninstallerDataObject(rootKey, keyPath, subKeyName);
                            if (!string.IsNullOrEmpty(mdo.ApplicationName))
                            {
                                manager.Objects.Add(mdo);
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Trace.TraceError("Exception {0}: unable to analyse {1}", e, keyPath);
                        Trace.TraceWarning(e.StackTrace);
                    }

                }
            }
        }
    }
}
