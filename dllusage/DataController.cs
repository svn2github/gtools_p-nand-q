using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dllusage
{
    public abstract class DataController
    {
        public readonly string DisplayName;

        public DataController(string displayName)
        {
            DisplayName = displayName;
        }

        public abstract void Refresh(Dictionary<string, TreeViewItemModel> itemsDictionary);
    }
}
