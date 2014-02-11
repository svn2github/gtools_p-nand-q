using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace pserv4
{
    public abstract class DataObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool ConstructionIsFinished;

        public bool IsDisabled { get; protected set; }
        public bool IsRunning { get; protected set; }
        public string ToolTip { get; protected set; }
        public string ToolTipCaption { get; protected set; }
        public string InternalID { get; private set; }

        public DataObject(string internalID)
        {
            ConstructionIsFinished = false;
            InternalID = internalID;
        }

        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        protected bool SetRunning(bool isRunning)
        {
            if (!ConstructionIsFinished || (isRunning != IsRunning))
            {
                IsRunning = isRunning;
                NotifyPropertyChanged("IsRunning");
                return true;
            }
            return false;
        }

        protected bool SetDisabled(bool isDisabled)
        {
            if (!ConstructionIsFinished || isDisabled != IsDisabled)
            {
                IsDisabled = isDisabled;
                NotifyPropertyChanged("IsDisabled");
            }
            return false;
        }

        public bool SetStringProperty(string bindingName, object newValue)
        {
            string stringValue = (newValue == null) ? "" : newValue.ToString();

            Type t = GetType();

            string actualValue = t.GetProperty(bindingName).GetValue(this, null) as string;
            if (!ConstructionIsFinished || !stringValue.Equals(actualValue))
            {
                t.GetProperty(bindingName).SetValue(this, stringValue, null);
                NotifyPropertyChanged(bindingName);
                return true;
            }
            return false;
        }

        public bool SetNonZeroStringProperty(string bindingName, object newValue)
        {
            string stringValue = (newValue == null) ? "" : newValue.ToString();
            if (stringValue.Equals("0"))
                stringValue = "";

            Type t = GetType();

            string actualValue = t.GetProperty(bindingName).GetValue(this, null) as string;
            if (!ConstructionIsFinished || !stringValue.Equals(actualValue))
            {
                t.GetProperty(bindingName).SetValue(this, stringValue, null);
                NotifyPropertyChanged(bindingName);
                return true;
            }
            return false;
        }

        public bool BringUpExplorer(string directory)
        {
            Trace.TraceInformation("{0}.BringUpExplorer({1}) called", this, directory);
            try
            {
                if (string.IsNullOrEmpty(directory))
                {
                    Trace.TraceWarning("Warning, directory is empty - assume function failed");
                    return false;
                }

                string cmd = string.Format("/root,{0}", directory);
                Trace.TraceInformation("CMD: {0}", cmd);

                using (Process p = Process.Start("explorer.exe", cmd))
                {
                    if (p == null)
                    {
                        Trace.TraceWarning("Warning, Process.Start() returned null, assuming function failed");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("Exception {0}: unable to bring up Explorer in directory {1}", e, directory);
                Trace.TraceWarning(e.StackTrace);
                return false;
            }
        }

        public bool ShowRegistryEditor(string key)
        {
            using (RegistryKey hkKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit", true))
            {
                hkKey.SetValue("Lastkey", key);
            }
            try
            {
                Process.Start("regedit.exe");
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("Exception {0}: unable to bring up registry editor in {1}", e, key);
                Trace.TraceWarning(e.StackTrace);
                return false;
            }
        }

        public bool BringUpTerminal(string directory)
        {
            Trace.TraceInformation("{0}.BringUpTerminal({1}) called", this, directory);
            try
            {
                if (string.IsNullOrEmpty(directory))
                {
                    Trace.TraceWarning("Warning, directory is empty - assume function failed");
                    return false;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "cmd.exe"));
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.Arguments = string.Format("/K \"cd /d {0}\"", directory);

                using (Process p = Process.Start(startInfo))
                {
                    if (p == null)
                    {
                        Trace.TraceWarning("Warning, Process.Start() returned null, assuming function failed");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("Exception {0}: unable to bring up cmd.exe in directory {1}", e, directory);
                Trace.TraceWarning(e.StackTrace);
                return false;
            }
        }

    }
}
