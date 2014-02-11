using System;
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
            AnythingPaused = true;
            AnythingRunning = true;
            AnythingStopped = true;
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
                AppendMenuItem(menu, Resources.UNINSTALLER_FORCE_UNINSTALL, "delete", RemoveFromRegistry);
                menu.Items.Add(new Separator());
                AppendMenuItem(menu, Resources.IDS_PROPERTIES, "database_gear", ShowProperties);
                return menu;
            }
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

        public void RemoveFromRegistry(object sender, RoutedEventArgs e)
        {
            List<UninstallerDataObject> deleteThese = new List<UninstallerDataObject>();
            foreach (UninstallerDataObject udo in MainListView.SelectedItems)
            {
                deleteThese.Add(udo);
            }

            foreach(UninstallerDataObject udo in deleteThese)
            {
                if (udo.RemoveFromRegistry())
                {
                    MainWindow.Items.Remove(udo);
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
                    if (manager.Contains(subKeyName, out mdo))
                    {
                        mdo.Refresh(rootKey, keyPath, subKeyName);
                    }
                    else
                    {
                        mdo = new UninstallerDataObject(rootKey, keyPath, subKeyName);
                        if( !string.IsNullOrEmpty(mdo.ApplicationName) )
                        {
                            manager.Objects.Add(mdo);
                        }
                    }
                }
            }
        }
    }
}
