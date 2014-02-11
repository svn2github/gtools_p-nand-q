using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace pserv4
{
    public interface IDataObjectDetails
    {
        void RevertChanges();
        void ApplyChanges(object context);
        bool HasAnyChanges();
        void BindTabItem(TabItem tabItem);
    }
}
