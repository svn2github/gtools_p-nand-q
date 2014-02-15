using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using log4net;

namespace dllusage
{
    public class GetModulesByPath : DataController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GetModulesByPath(string name = null)
            :   base( (name == null) ? resource.IDS_ModulesByPath : name)
        {
        }

        public override void Refresh(Dictionary<string, TreeViewItemModel> itemsDictionary)
        {
            var duplicates = new Dictionary<string, Dictionary<string, TreeViewItemModel>>();

            // reset all duplicates
            foreach(TreeViewItemModel model in itemsDictionary.Values)
            {
                model.IsDuplicate = false;
            }

            foreach(Process p in Process.GetProcesses())
            {
                if ((p.Id < 10) || RefreshManager.KnownSystemServices.ContainsKey(p.ProcessName))
                {
                    continue;
                }

                string processName = p.GetSafeProcessName();
                string processKey = processName.ToLower();

                ProcessModuleCollection pmc = null;
                try
                {
                     pmc = p.Modules;
                }
                catch
                {
                    continue;                
                }
                if (pmc != null)
                {
                    string formattedProcessName = string.Format("{0} [{1}]", processName, p.Id);
                    foreach (ProcessModule m in pmc)
                    {
                        string moduleName = m.GetSafeModuleName();
                        string moduleKey = moduleName.ToLower();
                        TreeViewItemModel pi = null;
                        if (!itemsDictionary.TryGetValue(moduleKey, out pi))
                        {
                            pi = new TreeViewItemModel(GetDisplayNameFromModuleName(moduleName), moduleKey, m);
                            itemsDictionary[moduleKey] = pi;
                        }
                        pi.AddItem(processName, processKey, p);

                        // now, enable duplicate detection
                        string moduleFileName = Path.GetFileName(moduleKey);
                        Dictionary<string, TreeViewItemModel> existingModules = null;
                        if (duplicates.TryGetValue(moduleFileName, out existingModules))
                        {
                            existingModules[moduleKey] = pi;

                            if (existingModules.Keys.Count > 1)
                            {
                                foreach (TreeViewItemModel model in existingModules.Values)
                                {
                                    model.IsDuplicate = true;
                                }
                            }
                        }
                        else
                        {
                            // register single dictionary only
                            var newModule = new Dictionary<string, TreeViewItemModel>();
                            newModule[moduleKey] = pi;
                            duplicates[moduleFileName] = newModule;
                        }
                    }
                }
            }
        }

        protected virtual string GetDisplayNameFromModuleName(string moduleName)
        {
            return moduleName;
        }
    }
}
