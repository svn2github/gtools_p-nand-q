using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace pserv4
{
    public interface IObjectController
    {
        IEnumerable<ObjectColumn> Columns { get; }

        /// <summary>
        /// Given the input list, refresh it with the current object list 
        /// </summary>
        /// <param name="objects"></param>
        void Refresh(ObservableCollection<IObject> objects);
    }
}
