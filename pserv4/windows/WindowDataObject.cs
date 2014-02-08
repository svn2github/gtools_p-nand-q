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

        public WindowDataObject(int hwnd)
        {
            Handle = hwnd;
            bool WindowTimedOut = false;
           
            Title = NativeWindowFunctions.GetTitle(hwnd, out WindowTimedOut);
            Class = NativeWindowFunctions.GetClassName(hwnd);
            WS_STYLE style = NativeWindowFunctions.GetWindowStyle(hwnd);
            Style = style.ToString();
            ExStyle = NativeWindowFunctions.GetWindowExStyle(hwnd).ToString();
            ID = NativeWindowFunctions.GetWindowID(hwnd).ToString();

            RECT r = new RECT();
            NativeWindowFunctions.GetWindowRect(Handle, ref r);

            Size = string.Format("({0}, {1})", r.Width, r.Height);
            Position = string.Format("({0}, {1})", r.Top, r.Left);

            int pID, tID;
            NativeWindowFunctions.GetWindowThreadProcessId(hwnd, out pID, out tID);
            ProcessID = pID.ToString();
            ThreadID = tID.ToString();
            
            if ((r.Width == r.Height) && (r.Width == 0))
            {
                IsDisabled = true;
            }
            if ((style & WS_STYLE.VISIBLE) == 0)
            {
                IsDisabled = true;
            }
            if (!WindowTimedOut)
            {
                IsRunning = true;
            }
        }
    }

}
