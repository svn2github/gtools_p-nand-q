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
    public class ModuleDataObject : IObject
    {
        public string ID { get; private set; }
        public string ProcessID { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string ModuleMemorySize { get; private set; }
        public string FileDescription { get; private set; }
        public string FileVersion { get; private set; }
        public string Product { get; private set; }
        public string ProductVersion { get; private set; }

        public bool IsDisabled { get; private set; }
        public bool IsRunning { get; private set; }


        public ModuleDataObject(Process p, ProcessModule m, bool isDisabled)
        {
            ProcessID = p.Id.ToString();
            Name = System.IO.Path.GetFileName(m.FileName);
            
            ID = string.Format("{0}.{1}", p.Id, m.FileName);

            Path = System.IO.Path.GetDirectoryName(m.FileName);
            ModuleMemorySize = Localisation.BytesToSize(m.ModuleMemorySize); 
            FileDescription = m.FileVersionInfo.FileDescription;
            FileVersion = m.FileVersionInfo.FileVersion;
            Product = m.FileVersionInfo.ProductName;
            ProductVersion = m.FileVersionInfo.ProductVersion;
            IsDisabled = isDisabled;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

}
