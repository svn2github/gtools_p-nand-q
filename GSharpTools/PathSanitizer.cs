using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using log4net;

namespace GSharpTools
{
    public static class PathSanitizer
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [DllImport("shell32.dll")]
        private static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, CSIDL nFolder, bool fCreate);

        private static string GetDirectory(CSIDL folder)
        {
            StringBuilder path = new StringBuilder(260);
            SHGetSpecialFolderPath(IntPtr.Zero, path, folder, false);
            return path.ToString();
        }

        public static string Get32BitSystemDirectory()
        {
            return GetDirectory(CSIDL.SYSTEMX86);
        }

        public static string GetWindowsDirectory()
        {
            return GetDirectory(CSIDL.WINDOWS);
        }

        private static Dictionary<string, string> Cache = new Dictionary<string, string>();

        public static string GetDirectory(string userPath)
        {
            if (userPath == null)
                return "";

            string result;
            if( Cache.TryGetValue(userPath, out result) )
            {
                return result;
            }

            result = GetExecutable(userPath);
            int i = result.LastIndexOf('\\');
            if (i >= 0)
            {
                result = result.Substring(0, i);
            }

            if ((result.Length <= 3) && File.Exists(userPath))
            {
                result = userPath;
                i = result.LastIndexOf('\\');
                if (i >= 0)
                {
                    result = result.Substring(0, i);
                }
            }
            if(result.StartsWith("\\??\\"))
            {
                result = result.Substring(4);
            }
            if( result.StartsWith("\\Systemroot\\system32", StringComparison.OrdinalIgnoreCase))
            {
                result = Environment.SystemDirectory + result.Substring(20);
            }
            else if (result.StartsWith("system32\\", StringComparison.OrdinalIgnoreCase))
            {
                result = Environment.SystemDirectory + result.Substring(8);
            }
            else if (result.StartsWith("syswow64\\", StringComparison.OrdinalIgnoreCase))
            {
                result = Get32BitSystemDirectory() + result.Substring(8);
            }
            Cache[userPath] = result;
            return result;
        }

        public static string GetArguments(string userPath)
        {
            string result = userPath;
            if (userPath.StartsWith("\""))
            {
                result = userPath.Substring(1);
                int k = result.IndexOf('"');
                if (k >= 0)
                {
                    result = result.Substring(k + 1);
                }
            }
            else
            {
                int k = result.IndexOf(' ');
                if (k >= 0)
                {
                    result = result.Substring(k + 1);
                }
            }
            result = result.Trim();
            return result;
        }

        public static string GetExecutable(string userPath, bool ensureQuotes = false)
        {
            string result = userPath;
            if (result.StartsWith("\""))
            {
                result = result.Substring(1);
                int k = result.IndexOf('"');
                if (k >= 0)
                {
                    result = result.Substring(0, k);
                }
            }
            else
            {
                int k = result.IndexOf(' ');
                if (k >= 0)
                {
                    result = result.Substring(0, k);
                }
            }
            if (ensureQuotes)
            {
                if( userPath.IndexOf(' ') >= 0 )
                {
                    return string.Format("\"{0}\"", userPath);
                }
            }
            return result;
        }
    }
}
