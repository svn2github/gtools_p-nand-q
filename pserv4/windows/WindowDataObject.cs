using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pserv4.windows
{
    public class WindowDataObject : DataObject
    {
        public string HandleString
        { 
            get
            {
                return Handle.ToString();
            }
        }
        
        public string Title { get; private set; }
        public string Class { get; private set; }
        public string Style { get; private set; }
        public string ExStyle { get; private set; }
        public string ID { get; private set; }
        public string Size { get; private set; }
        public string Position { get; private set; }
        public string ProcessID { get; private set; }
        public string ThreadID { get; private set; }
        public string Process { get; private set; }
        
        public readonly int Handle;

        public bool Refresh(int hwnd)
        {
            bool WindowTimedOut = false;

            string title = NativeWindowFunctions.GetTitle(hwnd, out WindowTimedOut);
            if( string.IsNullOrEmpty(title) )
                return false;

            if(SetStringProperty("Title", title))
            {
                ToolTipCaption = title;
                NotifyPropertyChanged("ToolTipCaption");
            }
            
            if(SetStringProperty("Class", NativeWindowFunctions.GetClassName(hwnd)))
            {
                ToolTip = Class;
                NotifyPropertyChanged("ToolTip");
            }
            
            WS_STYLE style = NativeWindowFunctions.GetWindowStyle(hwnd);
            SetStringProperty("Style", style);
            SetStringProperty("ExStyle", NativeWindowFunctions.GetWindowExStyle(hwnd));
            SetStringProperty("ID", NativeWindowFunctions.GetWindowID(hwnd));
            
            RECT r = new RECT();
            NativeWindowFunctions.GetWindowRect(Handle, ref r);

            SetStringProperty("Size", string.Format("({0}, {1})", r.Width, r.Height));
            SetStringProperty("Position", string.Format("({0}, {1})", r.Top, r.Left));

            int pID, tID;
            NativeWindowFunctions.GetWindowThreadProcessId(hwnd, out pID, out tID);
            SetStringProperty("ProcessID", pID);
            SetStringProperty("ThreadID", tID);

            bool isDisabled = false;
            bool isRunning = false;

            if ((r.Width == r.Height) && (r.Width == 0))
            {
                isDisabled = true;
            }
            if ((style & WS_STYLE.VISIBLE) == 0)
            {
                isDisabled = true;
            }
            if (!WindowTimedOut)
            {
                isRunning = true;
            }
            SetRunning(isRunning);
            SetDisabled(isDisabled);
            return true;
        }

        public WindowDataObject(int hwnd)
            : base(hwnd.ToString("X4"))
        {
            Handle = hwnd;
            Refresh(hwnd);
            ConstructionIsFinished = true;
        }
    }

}
