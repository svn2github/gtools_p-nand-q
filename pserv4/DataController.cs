using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace pserv4
{
    public abstract class DataController
    {
        public abstract IEnumerable<ObjectColumn> Columns { get; }

        public ListView MainListView;

        /// <summary>
        /// Given the input list, refresh it with the current object list 
        /// </summary>
        /// <param name="objects"></param>
        public abstract void Refresh(ObservableCollection<DataObject> objects);
        
        /// <summary>
        /// Return the context menu used for all items on this list
        /// </summary>
        public virtual ContextMenu ContextMenu
        {
            get
            {
                return null;
            }
        }

        public virtual void OnContextMenuOpening(System.Collections.IList selectedItems, ContextMenu menu)
        {
            // default implementation: do nothing
        }


    }
}
