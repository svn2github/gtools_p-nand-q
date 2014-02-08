using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pserv4.services
{
    public class RequestServiceStartup : ServiceStateRequest
    {
        #region ServiceStateRequest Members

        public ACCESS_MASK GetServiceAccessMask()
        {
            return ACCESS_MASK.SERVICE_START;
        }

        public bool Request(ServiceStatus ss)
        {
            return ss.Start();
        }

        public bool HasSuccess(SC_RUNTIME_STATUS state)
        {
            return state == SC_RUNTIME_STATUS.SERVICE_RUNNING;
        }

        public bool HasFailed(SC_RUNTIME_STATUS state)
        {
            return state == SC_RUNTIME_STATUS.SERVICE_STOPPED;
        }

        #endregion
    }
}
