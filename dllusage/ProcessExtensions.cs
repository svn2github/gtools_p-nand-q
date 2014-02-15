using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace dllusage
{
    public static class ProcessExtensions
    {
        public static string GetSafeProcessName(this Process p)
        {
            string result = p.ProcessName;
            try
            {
                if (p.Id >= 10)
                    result = p.MainModule.FileName;
            }
            catch
            {

            }
            if (result.StartsWith("\\??\\"))
                result = result.Substring(4);
            return result;
        }
    }
}
