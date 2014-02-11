using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pserv4
{
    public  class DataObjectColumn
    {
        public readonly string DisplayName;
        public readonly string BindingName;

        public DataObjectColumn(string displayName, string bindingName)
        {
            DisplayName = displayName;
            BindingName = bindingName;
        }
    }
}
