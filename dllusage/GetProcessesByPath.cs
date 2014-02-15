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
    public class GetProcessesByPath : DataController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GetProcessesByPath(string name = null)
            :   base( (name == null) ? resource.IDS_ProcessesByPath : name)
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
                    string key = procname.ToLower();

                    TreeViewItemModel pi = null;
                    if (!itemsDictionary.TryGetValue(procname, out pi))
                    {
                        pi = new TreeViewItemModel(GetDisplayNameFromProcessName(procname), key, p);
                        itemsDictionary[procname] = pi;
                    }
                    foreach (ProcessModule m in pmc)
                    {
                        string moduleName = m.GetSafeModuleName(); // TODO: decorate with more info
                        string moduleKey = moduleName.ToLower();
                        pi.AddItem(moduleName, moduleKey, m);
                    }
                }
            }
        }

        protected virtual string GetDisplayNameFromProcessName(string procname)
        {
            return procname;
        }
    }
}
