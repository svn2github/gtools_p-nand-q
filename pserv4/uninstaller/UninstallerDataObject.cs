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
        public string ModifyPath { get; private set; }
        public readonly bool BasedInLocalMachine;
        
        public void Refresh(RegistryKey rootKey, string keyPath, string keyName)
        {
            using (RegistryKey hkKey = rootKey.OpenSubKey(keyPath, false))
            {
                string applicationName = hkKey.GetValue("DisplayName") as string;
                if (string.IsNullOrEmpty(applicationName))
                {
                    applicationName = hkKey.GetValue("QuietDisplayName") as string;
                }
                if (string.IsNullOrEmpty(applicationName))
                {
                    applicationName = keyName;
                }
                if(SetStringProperty("ApplicationName", applicationName))
                {
                    ToolTipCaption = applicationName;
                    NotifyPropertyChanged("ToolTipCaption");
                }

                SetStringProperty("InstallLocation", hkKey.GetValue("InstallLocation") as string);
                SetStringProperty("Version", hkKey.GetValue("DisplayVersion") as string);
                SetStringProperty("Publisher", hkKey.GetValue("Publisher") as string);
                SetStringProperty("HelpLink", hkKey.GetValue("HelpLink") as string);
                SetStringProperty("ModifyPath", hkKey.GetValue("ModifyPath") as string);
                SetStringProperty("AboutLink", hkKey.GetValue("URLInfoAbout") as string);
                if(SetStringProperty("Action", hkKey.GetValue("UninstallString") as string))
                {
                    ToolTip = Action;
                    NotifyPropertyChanged("ToolTip");
                }

                bool isDisabled = false;
                bool isRunning = false;
                if (string.IsNullOrEmpty(Action))
                {
                    isDisabled = true;
                }
                else if (!string.IsNullOrEmpty(InstallLocation))
                {
                    isRunning = true;
                }
                SetRunning(isRunning);
                SetDisabled(isDisabled);
            }
        }

        public bool RemoveFromRegistry()
        {
            return false;
        }

        public UninstallerDataObject(RegistryKey rootKey, string keyPath, string keyName)
            :   base(keyName)
        {
            BasedInLocalMachine = (rootKey == Registry.LocalMachine);
            Refresh(rootKey, keyPath, keyName);
            ConstructionIsFinished = true;
        }

    }

}
