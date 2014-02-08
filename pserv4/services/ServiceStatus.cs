using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace pserv4.services
{
    public class ServiceStatus : IDisposable
    {
        private readonly NativeService Service;
        public SERVICE_STATUS_PROCESS Status = new SERVICE_STATUS_PROCESS();
        private IntPtr Memory;

        public ServiceStatus(NativeService service)
        {
            Service = service;
            Memory = Marshal.AllocHGlobal(Marshal.SizeOf(Status));
        }

        

        public bool Start()
        {
            if (NativeServiceFunctions.StartService(Service.Handle, 0, Memory))
                return true;

            Trace.WriteLine("ERROR, unable to startup service {0}", Service.Description);
            return false;
        }

        public bool Control(SC_CONTROL_CODE code)
        {
            if (NativeServiceFunctions.ControlService(Service.Handle, code, Memory))
            {
                Status = (SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(
                    Memory,
                    typeof(SERVICE_STATUS_PROCESS));
                Trace.TraceInformation("Currentstatus = {0}", Status.CurrentState);
                return true;
            }
            Trace.TraceInformation("ERROR, unable to set control {0} for service {1}",
                Marshal.GetLastWin32Error(), Service.Description);

            return false;
        }

        public bool Refresh()
        {
            int bytesNeeded;

            if (NativeServiceFunctions.QueryServiceStatusEx(
                Service.Handle,
                SC_STATUS_TYPE.SC_STATUS_PROCESS_INFO,
                Memory,
                Marshal.SizeOf(Status),
                out bytesNeeded))
            {
                Status = (SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(
                    Memory,
                    typeof(SERVICE_STATUS_PROCESS));
                Trace.TraceInformation("CurrentStatus as returned by QSSE = {0}", Status.CurrentState);
                return true;
            }
            Trace.WriteLine("ERROR, QueryServiceStatusEx() failed, aborting...");
            return false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Marshal.FreeHGlobal(Memory);
        }

        #endregion
    }
}
