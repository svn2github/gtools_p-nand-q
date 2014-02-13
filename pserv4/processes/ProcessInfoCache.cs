using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;
using pserv4.Properties;
using System.Management;
using System.Management.Instrumentation;
using log4net;
using System.Reflection;

namespace pserv4.processes
{
    public class ProcessInfoCache
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public class Data
        {
            // static data 
            public readonly string CommandLine;
            public readonly string Key;
            public readonly string ProcessId;
            public readonly string Name;

            private string _(ManagementObject queryObj, string key)
            {
                try
                {
                    var result = queryObj.Properties[key];
                    if( result != null )
                    {
                        if( result.Value != null )
                        {
                            return result.Value.ToString();
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Error(string.Format("Unable to query property '{0}'", key), e);
                }
                return "";
            }

            public Data(ManagementObject queryObj)
            {
                CommandLine = _(queryObj, "CommandLine");
                ProcessId = _(queryObj, "ProcessId");
                Name = _(queryObj, "Name");

                Key = string.Format("{0}.{1}", ProcessId, Name).ToLower();
                if (Key.EndsWith(".exe"))
                {
                    Key = Key.Substring(0, Key.Length - 4);
                }
            }
        }

        private static Dictionary<string, Data> KnownProcessInformation = null;

        private static string CreateKeyFromProcess(Process p)
        {
            string key = string.Format("{0}.{1}", p.Id, p.ProcessName).ToLower();
            if( key.EndsWith(".exe") )
            {
                key = key.Substring(0, key.Length - 4);
            }
            return key;
        }

        private static Data ReadDataForProcess(Process p)
        {
            string key = CreateKeyFromProcess(p);
            Data result;
            if (KnownProcessInformation.TryGetValue(key, out result))
            {
                return result;
            }
            return null;
        }

        private static void CreateInitialInformation(Process p)
        {
            KnownProcessInformation = new Dictionary<string, Data>();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT ProcessId,Name,CommandLine FROM Win32_Process"))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    Data data = new Data(queryObj);
                    KnownProcessInformation[data.Key] = data;
                }
            }                        
        }

        public static Data Get(Process p)
        {
            if (KnownProcessInformation == null)
                CreateInitialInformation(p);

            return ReadDataForProcess(p);
        }

    }
}
