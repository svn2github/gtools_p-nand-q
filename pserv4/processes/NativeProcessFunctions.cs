using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security.Principal;
using System.IO;
using log4net;
using System.Reflection;

using LUID = System.Int64;
using HANDLE = System.IntPtr;

namespace pserv4.processes
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
    
    
    public enum TOKEN_INFORMATION_CLASS
    {
        TokenUser = 1,
        TokenGroups,
        TokenPrivileges,
        TokenOwner,
        TokenPrimaryGroup,
        TokenDefaultDacl,
        TokenSource,
        TokenType,
        TokenImpersonationLevel,
        TokenStatistics,
        TokenRestrictedSids,
        TokenSessionId
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

    public static class NativeProcessFunctions
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int TOKEN_QUERY = 0X00000008;

        private const int ERROR_NO_MORE_ITEMS = 259;
        
        [StructLayout(LayoutKind.Sequential)]
        struct TOKEN_USER
        {
            public _SID_AND_ATTRIBUTES User;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct _SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public int Attributes;
        }

        [DllImport("advapi32")]
        private static extern bool OpenProcessToken(
            HANDLE ProcessHandle, // handle to process
            int DesiredAccess, // desired access to process
            ref IntPtr TokenHandle // handle to open access token
        );

        [DllImport("kernel32")]
        private static extern HANDLE GetCurrentProcess();

        [DllImport("advapi32", CharSet=CharSet.Auto)]
        private static extern bool GetTokenInformation(
            HANDLE hToken,
            TOKEN_INFORMATION_CLASS tokenInfoClass,
            IntPtr TokenInformation,
            int tokeInfoLength,
            ref int reqLength
        );

        [DllImport("kernel32")]
        private static extern bool CloseHandle(HANDLE handle);

        [DllImport("advapi32", CharSet=CharSet.Auto)]
        private static extern bool LookupAccountSid
        (
            [In,MarshalAs(UnmanagedType.LPTStr)] string lpSystemName, // name of local or remote computer
            IntPtr pSid, // security identifier
            StringBuilder Account, // account name buffer
            ref int cbName, // size of account name buffer
            StringBuilder DomainName, // domain name
            ref int cbDomainName, // size of domain name buffer
            ref int peUse // SID type
            // ref _SID_NAME_USE peUse // SID type
        );

        [DllImport("advapi32", CharSet=CharSet.Auto)]
        private static extern bool ConvertSidToStringSid(
            IntPtr pSID,
            [In,Out,MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid);

        public static string GetUserInfo(Process p)
        {
            if (p.Id < 10)
                return "SYSTEM";
            string result = "";
            try
            {
                int Access = TOKEN_QUERY;
                HANDLE procToken = IntPtr.Zero;
                if (OpenProcessToken(p.Handle, Access, ref procToken))
                {
                    result = PerformDump(procToken);
                    CloseHandle(procToken);
                }
            }
            catch (Exception e)
            {
                Log.Error("GetUserInfo", e);
            }
            return result;
        }

        private static string PerformDump(HANDLE token)
        {
            StringBuilder sb = new StringBuilder();
            TOKEN_USER tokUser;
            const int bufLength = 256;
            IntPtr tu = Marshal.AllocHGlobal( bufLength );
            int cb = bufLength;
            GetTokenInformation( token, TOKEN_INFORMATION_CLASS.TokenUser, tu, cb, ref cb );
            tokUser = (TOKEN_USER) Marshal.PtrToStructure(tu, typeof(TOKEN_USER) );
            string result = DumpAccountSid(tokUser.User.Sid);
            Marshal.FreeHGlobal( tu );
            return result;
        }
        
        private static string DumpAccountSid(IntPtr SID)
        {
            int cchAccount = 0;
            int cchDomain = 0;
            int snu = 0 ;
            string result = "";

            // Caller allocated buffer
            StringBuilder Account= null;
            StringBuilder Domain = null;
            bool ret = LookupAccountSid(null, SID, Account, ref cchAccount, Domain, ref cchDomain, ref snu);
            if (ret == true)
                if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS)
                    return "";
            try
            {
                Account = new StringBuilder( cchAccount );
                Domain = new StringBuilder( cchDomain );
                ret = LookupAccountSid(null, SID, Account, ref cchAccount, Domain, ref cchDomain, ref snu);
                if (ret)
                {
                    result = Account.ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Error("DumpAccountSid", ex);
            }
            finally
            {
            }
            return result;
        }

        public static string GetSafeProcessName(Process p)
        {
            string result = p.ProcessName;
            try
            {
                if (p.Id >= 10)
                    result = p.MainModule.FileName;
            }
            catch(Exception e)
            {
                Log.Error("GetSafeProcessName", e);
            }
            if (result.StartsWith("\\??\\"))
                result = result.Substring(4);
            return result;
        }

    }

}
