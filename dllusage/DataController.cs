using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;

namespace dllusage
{
    public abstract class DataController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public readonly string DisplayName;

        public DataController(string displayName)
        {
            DisplayName = displayName;
        }

        public abstract void Refresh(Dictionary<string, TreeViewItemModel> itemsDictionary);
    }
}
