using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

namespace pserv4.services
{
    public class NativeSCManager : IDisposable
    {
        public IntPtr Handle;

        public NativeSCManager(string machineName = null)
        {
            if (string.IsNullOrEmpty(machineName))
                machineName = Environment.MachineName;

            Handle = NativeServiceFunctions.OpenSCManager(
                machineName,
                NativeServiceFunctions.SERVICES_ACTIVE_DATABASE,
                (uint)(ACCESS_MASK.STANDARD_RIGHTS_READ | ACCESS_MASK.GENERIC_READ));
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!NativeServiceFunctions.CloseServiceHandle(Handle))
            {
                Trace.WriteLine("Warning, unable to close ServiceControlManager.Handle");
            }
        }

        #endregion

        /// <summary>
        /// Returns the status of all currently existing services on this SCM
        /// </summary>
        /// <param name="ServiceType">Type of serviec </param>
        /// <returns>Enumerable list of service statuses</returns>
        public IEnumerable<ENUM_SERVICE_STATUS_PROCESS> Refresh(SC_SERVICE_TYPE ServiceType)
        {
            // Quote from MSDN: Windows Server 2003 and Windows XP:  
            // The maximum size of this array is 64K bytes. This limit
            // was increased as of Windows Server 2003 SP1 and Windows XP SP2.

            const int BytesAllocated = 63 * 1024;

            IntPtr lpServices = Marshal.AllocHGlobal((int)BytesAllocated);
            int cbBytesNeeded = 0;
            int ServicesReturned = 0;
            int ResumeHandle = 0;
            bool repeat = true;
            while (repeat)
            {
                if (NativeServiceFunctions.EnumServicesStatusEx(Handle,
                        SC_ENUM_TYPE.SC_ENUM_PROCESS_INFO,
                        ServiceType,
                        SC_QUERY_SERVICE_STATUS.SERVICE_STATE_ALL,
                        lpServices,
                        BytesAllocated,
                        ref cbBytesNeeded,
                        ref ServicesReturned,
                        ref ResumeHandle,
                        null))
                {
                    //Trace.TraceInformation("Got {0} services in last chunk", ServicesReturned);
                    repeat = false;
                }
                else
                {
                    int LastError = Marshal.GetLastWin32Error();
                    if (LastError == NativeServiceFunctions.ERROR_MORE_DATA)
                    {
                        //Trace.TraceInformation("Got {0} services in this chunk", ServicesReturned);
                    }
                    else
                    {
                        //Trace.TraceInformation("ERROR {0}, unable to query list", LastError);
                        break;
                    }
                }

                int iPtr = lpServices.ToInt32();
                for (int i = 0; i < ServicesReturned; i++)
                {
                    ENUM_SERVICE_STATUS_PROCESS essp = (ENUM_SERVICE_STATUS_PROCESS)
                        Marshal.PtrToStructure(
                            new IntPtr(iPtr),
                            typeof(ENUM_SERVICE_STATUS_PROCESS));

                    yield return essp;
                    iPtr += Marshal.SizeOf(essp);
                }
            }
        }
    }
}
