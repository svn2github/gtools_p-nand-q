﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
namespace pserv4
{
    public static class PathSanitizer
    {
        [DllImport("shell32.dll")]
        private static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);

        private static string Get32BitSystemDirectory()
        {
            StringBuilder path = new StringBuilder(260);
            SHGetSpecialFolderPath(IntPtr.Zero,path,0x0029,false);
            return path.ToString();
        }

        private static Dictionary<string, string> Cache = new Dictionary<string, string>();

        public static string GetDirectory(string userPath)
        {
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

        public static string GetExecutable(string userPath)
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
            return result;
        }
    }
}
