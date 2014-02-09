using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

namespace pserv4.windows
{
    [Flags]
    public enum WS_EX_STYLE : uint
    {
        DLGMODALFRAME = 0x00000001,
        NOPARENTNOTIFY = 0x00000004,
        TOPMOST = 0x00000008,
        ACCEPTFILES = 0x00000010,
        TRANSPARENT = 0x00000020,
        MDICHILD = 0x00000040,
        TOOLWINDOW = 0x00000080,
        WINDOWEDGE = 0x00000100,
        CLIENTEDGE = 0x00000200,
        CONTEXTHELP = 0x00000400,
        RIGHT = 0x00001000,
        LEFT = 0x00000000,
        RTLREADING = 0x00002000,
        LTRREADING = 0x00000000,
        LEFTSCROLLBAR = 0x00004000,
        RIGHTSCROLLBAR = 0x00000000,
        CONTROLPARENT = 0x00010000,
        STATICEDGE = 0x00020000,
        APPWINDOW = 0x00040000,
        LAYERED = 0x00080000,
        NOINHERITLAYOUT = 0x00100000,
        LAYOUTRTL = 0x00400000,
        COMPOSITED = 0x02000000,
        NOACTIVATE = 0x08000000,
    }
    
    [Flags]
    public enum WS_STYLE : uint
    {
        POPUP = 0x80000000,
        CHILD = 0x40000000,
        MINIMIZE = 0x20000000,
        VISIBLE = 0x10000000,
        DISABLED = 0x08000000,
        CLIPSIBLINGS = 0x04000000,
        CLIPCHILDREN = 0x02000000,
        MAXIMIZE = 0x01000000,
        BORDER = 0x00800000,
        DLGFRAME = 0x00400000,
        VSCROLL = 0x00200000,
        HSCROLL = 0x00100000,
        SYSMENU = 0x00080000,
        THICKFRAME = 0x00040000,
        MINIMIZEBOX = 0x00020000,
        MAXIMIZEBOX = 0x00010000,
    }
    
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left_, int top_, int right_, int bottom_)
        {
            Left = left_;
            Top = top_;
            Right = right_;
            Bottom = bottom_;
        }

        public int Height { get { return Bottom - Top; } }
        public int Width { get { return Right - Left; } }
    }

    public static class NativeWindowFunctions
    {
        private const int WM_GETTEXT = 0x000D; 

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessageTimeout(int hwnd, int msg, int wParam, StringBuilder sb, int fuFlags, int uTimeout, out UIntPtr result);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetWindowThreadProcessId(int hwnd, out UIntPtr lpdwProcessId);
  
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(int hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetWindowLong(int hWnd, int nID);

        private const int GWL_STYLE  = -16;
        private const int GWL_EXSTYLE = -20;
        private const int GWL_ID = -12;

        private const int SMTO_ABORTIFHUNG = 0x0002;
        
        private delegate bool ENUM_WINDOWS_PROC(int hwnd, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int EnumWindows(ENUM_WINDOWS_PROC callPtr, int lParam);
        
        private const int GWL_WNDPROC = -4;
        private const int GWL_HINSTANCE = -6;
        private const int GWL_HWNDPARENT = -8;
        private const int GWL_USERDATA = -21;

        public const int SW_RESTORE = 9;
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 6;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(int hWnd, int state);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(int hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(int hWnd, ref RECT lpRect);
        
        private static List<int> Windows;
        
        private static bool EnumWindowsProc(int hwnd, int lParam)
        {
            Windows.Add(hwnd);
            return true;
        }
        
        public static List<int> EnumWindows()
        {
            Windows = new List<int>();
            EnumWindows(EnumWindowsProc, 0);
            return Windows;
        }
        
        public static void GetWindowThreadProcessId(int hwnd, out int ProcessID, out int ThreadID)
        {
            UIntPtr _ProcessID = new UIntPtr(0);
            ThreadID = (int) GetWindowThreadProcessId(hwnd, out _ProcessID);
            ProcessID = (int) _ProcessID.ToUInt32();
        }
        
        public static string GetTitle(int hwnd, out bool windowTimedOut)
        {
            StringBuilder sb = new StringBuilder();
            sb.EnsureCapacity(10240);
            UIntPtr lRes = new UIntPtr(1860);
            if (SendMessageTimeout(hwnd, WM_GETTEXT, 10240, sb, SMTO_ABORTIFHUNG, 1000, out lRes) == 0)
            {
                Trace.TraceError("SendMessageTimeout() failed with {0}", Marshal.GetLastWin32Error());
                windowTimedOut = true;
                return "???";
            }
            else
            {
                windowTimedOut = false;
                return sb.ToString();
            }
        }
        
        public static int GetWindowID(int hwnd)
        {
            return (int) GetWindowLong(hwnd, GWL_ID);
        }
        
        public static WS_EX_STYLE GetWindowExStyle(int hwnd)
        {
            return (WS_EX_STYLE)GetWindowLong(hwnd, GWL_EXSTYLE);
        }

        public static WS_STYLE GetWindowStyle(int hwnd)
        {
            return (WS_STYLE)GetWindowLong(hwnd, GWL_STYLE);
        }
        
        public static string GetClassName(int hwnd)
        {
            StringBuilder sb = new StringBuilder();
            sb.EnsureCapacity(10240);
            GetClassName(hwnd, sb, 10240);
            return sb.ToString();
        }
    }

}
