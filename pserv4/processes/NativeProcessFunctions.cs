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
                Log.Error(string.Format("Exception caught while querying user info on process '{0}'", p), e);
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
                Log.Error(string.Format("Exception caught in DumpAccountSid '{0}'", SID), ex);
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
            catch(Exception ex)
            {
                Log.Error(string.Format("Exception caught in GetSafeProcessName '{0}'", p), ex);
            }
            if (result.StartsWith("\\??\\"))
                result = result.Substring(4);
            return result;
        }

    }

}
