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
    public class GetModulesByName : GetModulesByPath
    {
        public GetModulesByName()
            :   base("Modules by name")
        {
        }

        protected override string GetDisplayNameFromModuleName(string moduleName)
        {
            string filename = Path.GetFileName(moduleName);
            string directory = Path.GetDirectoryName(moduleName);

            return string.Format("{0} in {1}", filename, directory);
        }
    }
}
