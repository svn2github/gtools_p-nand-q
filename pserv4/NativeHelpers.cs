using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security.Principal;
using System.IO;
using System.ComponentModel;
using log4net;
using System.Reflection;

namespace pserv4
{
    public static class NativeHelpers
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool ReportFailure(string context, params object[] args)
        {
            int lastError = Marshal.GetLastWin32Error();
            Log.ErrorFormat("ERROR {0}: {1} failed with {2}",
                lastError,
                string.Format(context, args),
                new Win32Exception(lastError).Message);
            return false;
        }
    }
}
