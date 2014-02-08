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
using Microsoft.Win32;

namespace pserv4.uninstaller
{
    public class UninstallerDataObject : DataObject
    {
        public string ApplicationName { get; private set; }
        public string InstallLocation { get; private set; }
        public string Version { get; private set; }
        public string Publisher { get; private set; }
        public string HelpLink { get; private set; }
        public string AboutLink { get; private set; }
        public string Action { get; private set; }
        
        public UninstallerDataObject(RegistryKey rootKey, string keyPath, string keyName)
            :   base(keyName)
        {
            using (RegistryKey hkKey = rootKey.OpenSubKey(keyPath, false))
            {
                ApplicationName = hkKey.GetValue("DisplayName") as string;
                if (string.IsNullOrEmpty(ApplicationName))
                {
                    ApplicationName = hkKey.GetValue("QuietDisplayName") as string;
                }
                if (string.IsNullOrEmpty(ApplicationName))
                {
                    ApplicationName = keyName;
                }
                
                InstallLocation = hkKey.GetValue("InstallLocation") as string;
                Version = hkKey.GetValue("DisplayVersion") as string;
                Publisher = hkKey.GetValue("Publisher") as string;
                HelpLink = hkKey.GetValue("HelpLink") as string;
                AboutLink = hkKey.GetValue("URLInfoAbout") as string;
                Action = hkKey.GetValue("UninstallString") as string;                
                
                if (string.IsNullOrEmpty(Action))
                {
                    IsDisabled = true;
                }
                else if (!string.IsNullOrEmpty(InstallLocation))
                {
                    IsRunning = true;
                }
            }
        }

    }

}
