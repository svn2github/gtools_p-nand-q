using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace pserv4.modules
{
    public class ModulesDataController : DataController
    {
        private static List<DataObjectColumn> ActualColumns;

        public ModulesDataController()
            :   base("Modules", "Module")
        {
        }

        public override IEnumerable<DataObjectColumn> Columns
        {
            get
            {
                if (ActualColumns == null)
                {
                    ActualColumns = new List<DataObjectColumn>();
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.MODULE_C_ProcessID, "ProcessID"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.MODULE_C_Name, "Name"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.MODULE_C_Path, "Path"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.MODULE_C_ModuleMemorySize, "ModuleMemorySize"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.MODULE_C_FileDescription, "FileDescription"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.MODULE_C_FileVersion, "FileVersion"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.MODULE_C_Product, "Product"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.MODULE_C_ProductVersion, "ProductVersion"));
                }
                return ActualColumns;
            }
        }

        public override void Refresh(ObservableCollection<DataObject> objects)
        {
            Dictionary<string, ModuleDataObject> existingObjects = new Dictionary<string, ModuleDataObject>();

            foreach (DataObject o in objects)
            {
                ModuleDataObject sdo = o as ModuleDataObject;
                if (sdo != null)
                {
                    existingObjects[sdo.InternalID] = sdo;
                }
            }

            foreach (Process p in Process.GetProcesses())
            {
                bool isDisabled = false;
                if( p.Id < 10 )
                {
                    isDisabled = true;
                }
                else
                {
                    string username = pserv4.processes.NativeProcessFunctions.GetUserInfo(p);
                    if (username.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase) )
                    {
                        isDisabled = true;
                    }
                }
                
                try
                {
                    
                    foreach (ProcessModule m in p.Modules)
                    {
                        string ID = string.Format("{0}.{1}", p.Id, m.FileName);
                                        
                        ModuleDataObject mdo;
                        if (existingObjects.TryGetValue(ID, out mdo))
                        {
                            // todo: refresh existing instance from updated data
                        }
                        else
                        {
                            objects.Add(new ModuleDataObject(p, m, isDisabled));
                        }
                    }
                }
                catch(Exception)
                {

                }
            }
        }
    }
}
