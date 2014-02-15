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
    public class RefreshManager 
    {
        public static Dictionary<string, bool> KnownSystemServices;

        public void Refresh(ObservableCollection<TreeViewItemModel> items, DataController controller)
        {
            if (KnownSystemServices == null)
            {
                KnownSystemServices = new Dictionary<string, bool>() {
                    { "smss", true },
                    { "svchost", true },
                    { "services", true },
                    { "csrss", true }
                };
            }

            /// buildup dictionary of existing items
            Dictionary<string, TreeViewItemModel> dict = new Dictionary<string, TreeViewItemModel>(StringComparer.CurrentCultureIgnoreCase);
            foreach(TreeViewItemModel model in items)
            {
                dict[model.Key] = model;
                model.IsValid = false;
                model.IsNew = false;
            }

            controller.Refresh(dict);

            foreach(TreeViewItemModel model in dict.Values)
            {
                if(model.IsNew)
                    items.Add(model);
            }

            // remove stale items
            foreach (TreeViewItemModel model in items)
            {
                if(!model.IsValid)
                    items.Remove(model);
            }
        }

        void RefreshByModule(ObservableCollection<TreeViewItemModel> items, MainViewType currentDisplayMode)
        {
            /*
       public
           Dictionary<string, ProcessInfo> dict = new Dictionary<string, ProcessInfo>(StringComparer.CurrentCultureIgnoreCase);
           foreach(Process p in Process.GetProcesses())
           {
               string procname = GetSafeProcessName(p);

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
                   string FormattedProcessName = string.Format("{0} [{1}]", procname, p.Id);
                   foreach (ProcessModule m in pmc)
                   {
                       string key = GetSafeModuleName(m);
                       ProcessInfo pi = null;
                       try
                       {
                           if (dict.ContainsKey(key))
                           {
                               pi = dict[key];
                           }
                       }
                       catch
                       {
                            
                       }
                       if (pi == null)
                       {
                           pi = new ProcessInfo(key, currentDisplayMode);
                           dict[key] = pi;
                       }
                       pi.Dependencies.Add(FormattedProcessName);
                   }
               }
           }
            
           Items.Clear();
           foreach (ProcessInfo pi in dict.Values)
               Items.Add(pi);*/
        }

        public void RefreshByProcess()
        {
    /*
            foreach(Process p in Process.GetProcesses())
            {
                ProcessInfo pi = new ProcessInfo(GetSafeProcessName(p), p.Id, CurrentDisplayMode);
                Items.Add(pi);
                ProcessModuleCollection pmc = null;
                try
                {
                     pmc = p.Modules;
                }
                catch
                {
                    continue;
                }
                foreach (ProcessModule m in pmc)
                {
                    pi.Dependencies.Add(GetSafeModuleName(m));
                }
            }*/
        }



    }
}
