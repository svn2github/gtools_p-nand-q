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
using log4net;
using System.Reflection;
using GSharpTools;

namespace pserv4.uninstaller
{
    public class UninstallerDataObject : DataObject
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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

        public void ApplyChanges(
            string applicationName, 
            string installLocation,
            string version,
            string publisher,
            string helpLink,
            string aboutLink,
            string modifyPath,
            string action
            )
        {
            RegistryKey rootKey = BasedInLocalMachine ? Registry.LocalMachine : Registry.CurrentUser;

            using (RegistryKey regkey = rootKey.OpenSubKey(UninstallerDataController.UNINSTALLER_SECTION, true))
            {
                using (RegistryKey hkKey = regkey.OpenSubKey(InternalID, true))
                {
                    if (SetStringProperty("ApplicationName", applicationName))
                    {
                        hkKey.SetValue("DisplayName", applicationName);
                        ToolTipCaption = applicationName;
                        NotifyPropertyChanged("ToolTipCaption");
                    }

                    if(SetStringProperty("InstallLocation", installLocation))
                    {
                        hkKey.SetValue("InstallLocation", installLocation);
                    }
                    if(SetStringProperty("Version", version))
                    {
                        hkKey.SetValue("DisplayVersion", installLocation);
                    }
                    if(SetStringProperty("Publisher", publisher))
                    {
                        hkKey.SetValue("Publisher", installLocation);
                    }
                    if(SetStringProperty("HelpLink", helpLink))
                    {
                        hkKey.SetValue("HelpLink", installLocation);
                    }
                    if (SetStringProperty("ModifyPath", modifyPath))
                    {
                        hkKey.SetValue("ModifyPath", installLocation);
                    }
                    if(SetStringProperty("AboutLink", aboutLink))
                    {
                        hkKey.SetValue("URLInfoAbout", installLocation);
                    }
                    if (SetStringProperty("Action", action))
                    {
                        hkKey.SetValue("UninstallString", installLocation);
                        ToolTip = Action;
                        NotifyPropertyChanged("ToolTip");
                    }
                }
            }
        }

        public bool ShowAboutLink()
        {
            return ShowLink(AboutLink);
        }
        
        public bool BringUpExplorerInInstallLocation()
        {
            return ProcessInfoTools.ShowExplorer(InstallLocation);
        }

        public bool BringUpTerminalInInstallLocation()
        {
            return ProcessInfoTools.ShowTerminal(InstallLocation);
        }

        public bool ShowLink(string link)
        {
            Log.InfoFormat("{0}.ShowLink({1}) called", this, link);
            try
            {
                if (string.IsNullOrEmpty(link))
                {
                    Log.Warn("Warning, link is empty - assume function failed");
                    return false;
                }
                using (Process p = Process.Start(link))
                {
                    if (p == null)
                    {
                        Log.Warn("Warning, Process.Start() returned null, assuming function failed");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("unable to bring up browser for URL {0}", link), e);
                return false;
            }
        }

        public bool ShowHelpLink()
        {
            return ShowLink(HelpLink);
        }

        public bool PerformUninstallAction()
        {
            return PerformAction(Action);
        }

        public bool PerformModifyAction()
        {
            return PerformAction(ModifyPath);
        }

        private bool PerformAction(string action)
        {
            Log.InfoFormat("{0}.PerformAction({1}) called", this, action);
            try
            {
                if (string.IsNullOrEmpty(action))
                {
                    Log.Warn("Warning, action is empty - assume function failed");
                    return false;
                }

                string cmd = PathSanitizer.GetExecutable(action);
                string args = PathSanitizer.GetArguments(action);
                Log.InfoFormat("CMD: {0}, ARGS: {1}", cmd, args);

                using (Process p = Process.Start(cmd, args))
                {
                    if (p == null)
                    {
                        Log.Warn("Warning, Process.Start() returned null, assuming function failed");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("unable to run action {0}", action), e);
                return false;
            }
        }

        public string RegistryKey
        {
            get
            {
                return string.Format("{0}\\{1}\\{2}",
                    (BasedInLocalMachine ? "HKEY_LOCAL_MACHINE" : "HKEY_CURRENT_USER"),
                    UninstallerDataController.UNINSTALLER_SECTION,
                    InternalID);
            }
        }


        public bool ShowRegistryEditor()
        {
            return ProcessInfoTools.ShowRegistryEditor(RegistryKey);
        }

        public bool RemoveFromRegistry()
        {
            try
            {
                RegistryKey rootKey = BasedInLocalMachine ? Registry.LocalMachine : Registry.CurrentUser;

                Log.InfoFormat("RemoveFromRegistry: {0}", RegistryKey);

                using (RegistryKey regkey = rootKey.OpenSubKey(UninstallerDataController.UNINSTALLER_SECTION, true))
                {
                    if (regkey.OpenSubKey(InternalID) != null)
                    {
                        regkey.DeleteSubKeyTree(InternalID);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("unable to remove registry key {0}", RegistryKey), e);
                return false;
            }
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
