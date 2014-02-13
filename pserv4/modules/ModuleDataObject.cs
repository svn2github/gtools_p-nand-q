using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public readonly int ID;

        public void Refresh(Process p, ProcessModule m, bool isDisabled)
        {
            SetStringProperty("ProcessID", p.Id );
            if(SetStringProperty("Name", System.IO.Path.GetFileName(m.FileName)))
            {
                SetStringProperty("ToolTipCaption", Name);
            }

            SetStringProperty("Path", System.IO.Path.GetDirectoryName(m.FileName));
            SetStringProperty("ModuleMemorySize", Localisation.BytesToSize(m.ModuleMemorySize));

            FileVersionInfoCache.CacheInfo ci = FileVersionInfoCache.Get(m.FileName, m);
            if( ci != null )
            {
                if(SetStringProperty("FileDescription", ci.FileDescription))
                {
                    SetStringProperty("ToolTip", ci.FileDescription);
                }
                SetStringProperty("FileVersion", ci.FileVersion);
                SetStringProperty("Product", ci.ProductName);
                SetStringProperty("ProductVersion", ci.ProductVersion);
            }

            SetRunning(!Path.ToLower().Contains(Environment.GetEnvironmentVariable("windir").ToLower()));
            SetDisabled(isDisabled);
        }

        public bool BringUpExplorerInInstallLocation()
        {
            return BringUpExplorer(Path);
        }

        public bool BringUpTerminalInInstallLocation()
        {
            return BringUpTerminal(Path);
        }

        public ModuleDataObject(Process p, ProcessModule m, bool isDisabled)
            :   base(string.Format("{0}.{1}", p.Id, m.FileName))
        {
            ID = p.Id;
            Refresh(p, m, isDisabled);
            ConstructionIsFinished = true;
        }
    }

}
