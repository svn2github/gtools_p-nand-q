using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pserv4
{
    public abstract class DataObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDisabled { get; protected set; }
        public bool IsRunning { get; protected set; }
        public string ToolTip { get; protected set; }
        public string ToolTipCaption { get; protected set; }
        public string InternalID { get; private set; }

        public DataObject(string internalID)
        {
            InternalID = internalID;
        }

        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
