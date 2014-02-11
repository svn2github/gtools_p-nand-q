using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;

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

        protected bool SetStringProperty(string bindingName, object newValue)
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

        protected bool SetNonZeroStringProperty(string bindingName, object newValue)
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

    }
}
