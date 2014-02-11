using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace pserv4
{
    public static class PathSanitizer
    {
        public static string GetDirectory(string userPath)
        {
            string result = GetExecutable(userPath);
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
