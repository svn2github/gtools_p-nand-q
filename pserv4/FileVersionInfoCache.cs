using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace pserv4
{
    public static class FileVersionInfoCache
    {
        public class CacheInfo
        {
            public readonly string FileDescription;
            public readonly string FileVersion;
            public readonly string ProductName;
            public readonly string ProductVersion;

            public CacheInfo(ProcessModule module)
            {
                FileDescription = module.FileVersionInfo.FileDescription;
                FileVersion = module.FileVersionInfo.FileVersion;
                ProductName = module.FileVersionInfo.ProductName;
                ProductVersion = module.FileVersionInfo.ProductVersion;
            }
        }

        private static Dictionary<string, CacheInfo> CachedInfos = new Dictionary<string,CacheInfo>();
        
        public static CacheInfo Get(string key, ProcessModule module)
        {
            CacheInfo result;
            if( CachedInfos.TryGetValue(key, out result))
            {
                return result;
            }
            result = new CacheInfo(module);
            CachedInfos[key] = result;
            return result;
        }

                            
    }
}
