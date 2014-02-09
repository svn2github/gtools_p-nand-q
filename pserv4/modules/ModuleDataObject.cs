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

        public void Refresh(Process p, ProcessModule m, bool isDisabled)
        {
            SetStringProperty("ProcessID", p.Id );
            SetStringProperty("Name", System.IO.Path.GetFileName(m.FileName));
            SetStringProperty("Path", System.IO.Path.GetDirectoryName(m.FileName));
            SetStringProperty("ModuleMemorySize", Localisation.BytesToSize(m.ModuleMemorySize));
            SetStringProperty("FileDescription", m.FileVersionInfo.FileDescription);
            SetStringProperty("FileVersion", m.FileVersionInfo.FileVersion);
            SetStringProperty("Product", m.FileVersionInfo.ProductName);
            SetStringProperty("ProductVersion", m.FileVersionInfo.ProductVersion);

            SetRunning(!Path.ToLower().Contains(Environment.GetEnvironmentVariable("windir").ToLower()));
            SetDisabled(isDisabled);
        }


        public ModuleDataObject(Process p, ProcessModule m, bool isDisabled)
            :   base(string.Format("{0}.{1}", p.Id, m.FileName))
        {
            Refresh(p, m, isDisabled);
            ConstructionIsFinished = true;
        }
    }

}
