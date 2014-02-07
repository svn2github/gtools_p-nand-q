using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pserv4
{
    public  class ObjectColumn
    {
        public readonly string DisplayName;
        public readonly string BindingName;

        public ObjectColumn(string displayName, string bindingName)
        {
            DisplayName = displayName;
            BindingName = bindingName;
        }
    }
}
