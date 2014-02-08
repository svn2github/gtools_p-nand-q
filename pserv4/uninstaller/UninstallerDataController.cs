using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace pserv4.uninstaller
{
    public class UninstallerDataController : DataController
    {
        private static List<DataObjectColumn> ActualColumns;

        public UninstallerDataController()
            : base("Programs", "Program")
        {
        }

        public override IEnumerable<DataObjectColumn> Columns
        {
            get
            {
                if (ActualColumns == null)
                {
                    ActualColumns = new List<DataObjectColumn>();
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_ApplicationName, "ApplicationName"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_InstallLocation, "InstallLocation"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_Key, "InternalID"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_Version, "Version"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_Publisher, "Publisher"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_HelpLink, "HelpLink"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_AboutLink, "AboutLink"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.UNINSTALLER_C_Action, "Action"));
                }
                return ActualColumns;
            }
        }

        public override void Refresh(ObservableCollection<DataObject> objects)
        {
            Dictionary<string, UninstallerDataObject> existingObjects = new Dictionary<string, UninstallerDataObject>();

            foreach (DataObject o in objects)
            {
                UninstallerDataObject sdo = o as UninstallerDataObject;
                if (sdo != null)
                {
                    existingObjects[sdo.InternalID] = sdo;
                }
            }
            RefreshEntries(existingObjects, objects, Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
            RefreshEntries(existingObjects, objects, Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
        }
        
        private void RefreshEntries(Dictionary<string, UninstallerDataObject> existingObjects, ObservableCollection<DataObject> objects, RegistryKey rootKey, string keyName)
        {
            using(RegistryKey hkKey = rootKey.OpenSubKey(keyName,false))
            {
                foreach (string subKeyName in hkKey.GetSubKeyNames())
                {
                    UninstallerDataObject mdo;
                    if (existingObjects.TryGetValue(subKeyName, out mdo))
                    {
                        // todo: refresh existing instance from updated data
                    }
                    else
                    {
                        mdo = new UninstallerDataObject(rootKey, string.Format("{0}\\{1}", keyName, subKeyName), subKeyName);
                        if( !string.IsNullOrEmpty(mdo.ApplicationName) )
                        {
                            objects.Add(mdo);
                        }
                    }
                }
            }
        }
    }
}
