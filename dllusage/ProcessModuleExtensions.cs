using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace dllusage
{
    public static class ProcessModuleExtensions
    {
        public static string GetSafeModuleName(this ProcessModule m)
        {
            try
            {
                return m.FileName;
            }
            catch
            {

            }
            return m.ToString();
        }
    }
}
