using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;

namespace pserv4.modules
{
    public class ModuleDataObject : DataObject
    {
        public string ProcessID { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string ModuleMemorySize { get; private set; }
        public string FileDescription { get; private set; }
        public string FileVersion { get; private set; }
        public string Product { get; private set; }
        public string ProductVersion { get; private set; }



        public ModuleDataObject(Process p, ProcessModule m, bool isDisabled)
            :   base(string.Format("{0}.{1}", p.Id, m.FileName))
        {
            ProcessID = p.Id.ToString();
            Name = System.IO.Path.GetFileName(m.FileName);

            Path = System.IO.Path.GetDirectoryName(m.FileName);
            ModuleMemorySize = Localisation.BytesToSize(m.ModuleMemorySize); 
            FileDescription = m.FileVersionInfo.FileDescription;
            FileVersion = m.FileVersionInfo.FileVersion;
            Product = m.FileVersionInfo.ProductName;
            ProductVersion = m.FileVersionInfo.ProductVersion;
            IsDisabled = isDisabled;
        }
    }

}
