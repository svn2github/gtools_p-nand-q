using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using log4net;

namespace dllusage
{
    public class GetProcessesByName : GetProcessesByPath
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public GetProcessesByName()
            :   base(resource.IDS_ProcessesByName)
        {
        }

        protected override string GetDisplayNameFromProcessName(string processName)
        {
            string filename = Path.GetFileName(processName);
            string directory = Path.GetDirectoryName(processName);

            return string.Format("{0} in {1}", filename, directory);
        }
    }
}
