using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

namespace dllusage
{
    public class GetModulesByPath : DataController
    {
        public GetModulesByPath(string name = null)
            :   base( (name == null) ? "Modules by path" : name)
        {
        }

        public override void Refresh(Dictionary<string, TreeViewItemModel> itemsDictionary)
        {
            foreach(Process p in Process.GetProcesses())
            {
                if ((p.Id < 10) || RefreshManager.KnownSystemServices.ContainsKey(p.ProcessName))
                {
                    continue;
                }

                string procname = p.GetSafeProcessName();

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
                    string formattedProcessName = string.Format("{0} [{1}]", procname, p.Id);
                    foreach (ProcessModule m in pmc)
                    {
                        string key = m.GetSafeModuleName();
                        TreeViewItemModel pi = null;
                        if (!itemsDictionary.TryGetValue(key, out pi))
                        {
                            pi = new TreeViewItemModel(GetDisplayNameFromModuleName(key), m);
                            itemsDictionary[key] = pi;
                        }
                        pi.AddItem(formattedProcessName, p);
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
