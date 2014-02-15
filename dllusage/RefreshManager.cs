using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using log4net;

namespace dllusage
{
    public class RefreshManager 
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
    }
}
