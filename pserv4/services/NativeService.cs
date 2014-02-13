using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using log4net;
using System.Reflection;

namespace pserv4.services
{
    /// <summary>
    /// This is a wrapper class that provides object-oriented access to a Windows service. 
    /// The object is disposable, so that the underlying handle can be safely closed.
    /// </summary>
    public class NativeService : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Native handle of the service. 
        /// <note>This is public, so that you can use one of the NativeServiceFunctions directly</note>
        /// </summary>
        public IntPtr Handle { get; private set; }

        public readonly string ServiceName;

        public readonly bool IsValid;

        public override string ToString()
        {
            return string.Format("NativeService({0}: {1})", Handle.ToInt32(), ServiceName);
        }

        /// <summary>
        /// The constructor opens access to the service
        /// </summary>
        /// <param name="scm">The SCM instance that contains the service</param>
        /// <param name="ServiceName">Name of the service</param>
        /// <param name="am">Access rights required.</param>
        public NativeService(NativeSCManager scm, string serviceName, ACCESS_MASK am = ACCESS_MASK.STANDARD_RIGHTS_READ | ACCESS_MASK.GENERIC_READ)
        {
            ServiceName = serviceName;

            Handle = NativeServiceFunctions.OpenService(
                scm.Handle,
                serviceName,
                (uint)am);

            IsValid = (Handle.ToInt32() != 0);
            if (!IsValid)
            {
                NativeHelpers.ReportFailure("OpenService({0}, {1})", serviceName, am);
            }
        }

        /// <summary>
        /// Returns a copy of the current service configuration
        /// </summary>
        public QUERY_SERVICE_CONFIG ServiceConfig
        {
            get
            {
                if (!IsValid)
                {
                    Log.WarnFormat("ServiceConfig not available for '{0}'", ServiceName);
                }
                else
                {
                    const int cbBufSize = 8 * 1024;
                    int cbBytesNeeded = 0;

                    IntPtr lpMemory = Marshal.AllocHGlobal((int)cbBufSize);

                    if (NativeServiceFunctions.QueryServiceConfig(Handle, lpMemory, cbBufSize, ref cbBytesNeeded))
                    {
                        return (QUERY_SERVICE_CONFIG)
                            Marshal.PtrToStructure(
                                new IntPtr(lpMemory.ToInt32()),
                                typeof(QUERY_SERVICE_CONFIG));
                    }
                    NativeHelpers.ReportFailure("QueryServiceConfig({0})", ServiceName);
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the text of the current service description
        /// </summary>
        public string Description
        {
            get
            {
                if (!IsValid)
                {
                    Log.WarnFormat("Description not available for '{0}'", ServiceName);
                }
                else
                {
                    const int cbBufSize = 8 * 1024;
                    int cbBytesNeeded = 0;

                    IntPtr lpMemory = Marshal.AllocHGlobal((int)cbBufSize);

                    if (NativeServiceFunctions.QueryServiceConfig2(
                            Handle,
                            SC_SERVICE_CONFIG_INFO_LEVEL.SERVICE_CONFIG_DESCRIPTION,
                            lpMemory,
                            cbBufSize,
                            out cbBytesNeeded))
                    {
                        SERVICE_DESCRIPTION sd = (SERVICE_DESCRIPTION)
                            Marshal.PtrToStructure(
                                new IntPtr(lpMemory.ToInt32()),
                                typeof(SERVICE_DESCRIPTION));
                        return sd.Description;
                    }
                    NativeHelpers.ReportFailure("QueryServiceConfig2({0})", ServiceName);
                }
                return null;
            }
            set
            {
                NativeServiceFunctions.ChangeServiceDescription(Handle, value);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if( IsValid )
            {
                if (!NativeServiceFunctions.CloseServiceHandle(Handle))
                {
                    Log.WarnFormat("Warning, unable to close NativeService.Handle");
                }
                Handle = new IntPtr(0);
            }
        }

        #endregion
    }
}
