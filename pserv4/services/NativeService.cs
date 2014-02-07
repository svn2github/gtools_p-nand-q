using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;

namespace pserv4.services
{
    /// <summary>
    /// This is a wrapper class that provides object-oriented access to a Windows service. 
    /// The object is disposable, so that the underlying handle can be safely closed.
    /// </summary>
    public class NativeService : IDisposable
    {
        /// <summary>
        /// Native handle of the service. 
        /// <note>This is public, so that you can use one of the NativeServiceFunctions directly</note>
        /// </summary>
        public readonly IntPtr Handle;

        /// <summary>
        /// The constructor opens access to the service
        /// </summary>
        /// <param name="scm">The SCM instance that contains the service</param>
        /// <param name="ServiceName">Name of the service</param>
        /// <param name="am">Access rights required.</param>
        public NativeService(NativeSCManager scm, string ServiceName, ACCESS_MASK am = ACCESS_MASK.STANDARD_RIGHTS_READ | ACCESS_MASK.GENERIC_READ)
        {
            Handle = NativeServiceFunctions.OpenService(
                scm.Handle,
                ServiceName,
                (uint)am);

            if( Handle.ToInt32() == 0 )
            {
                int lastError = Marshal.GetLastWin32Error();
                Trace.TraceError("ERROR {0}: OpenService failed with {1}",
                    lastError,
                    new Win32Exception(lastError).Message);
            }
        }

        /// <summary>
        /// Returns a copy of the current service configuration
        /// </summary>
        public QUERY_SERVICE_CONFIG ServiceConfig
        {
            get
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
                int lastError = Marshal.GetLastWin32Error();
                Trace.TraceError("ERROR {0}: QueryServiceConfig failed with {1}", 
                    lastError,
                    new Win32Exception(lastError).Message);

                Trace.TraceInformation("Handle: {0}", Handle);
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
                return null;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!NativeServiceFunctions.CloseServiceHandle(Handle))
            {
                Trace.WriteLine("Warning, unable to close NativeService.Handle");
            }
        }

        #endregion
    }
}
