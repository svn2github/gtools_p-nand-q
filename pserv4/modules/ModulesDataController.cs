using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace pserv4.modules
{
    public class ModulesDataController : IObjectController
    {
        private static List<ObjectColumn> ActualColumns;

        public ModulesDataController()
        {
        }

        public IEnumerable<ObjectColumn> Columns
        {
            get
            {
                if (ActualColumns == null)
                {
                    ActualColumns = new List<ObjectColumn>();
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.MODULE_C_ProcessID, "ProcessID"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.MODULE_C_Name, "Name"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.MODULE_C_Path, "Path"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.MODULE_C_ModuleMemorySize, "ModuleMemorySize"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.MODULE_C_FileDescription, "FileDescription"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.MODULE_C_FileVersion, "FileVersion"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.MODULE_C_Product, "Product"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.MODULE_C_ProductVersion, "ProductVersion"));
                }
                return ActualColumns;
            }
        }

        public void Refresh(ObservableCollection<IObject> objects)
        {
            Dictionary<string, ModuleDataObject> existingObjects = new Dictionary<string, ModuleDataObject>();

            foreach (IObject o in objects)
            {
                ModuleDataObject sdo = o as ModuleDataObject;
                if (sdo != null)
                {
                    existingObjects[sdo.ID] = sdo;
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
