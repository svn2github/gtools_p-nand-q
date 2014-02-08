using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pserv4.services
{
    public class RequestServiceContinue : ServiceStateRequest
    {
        #region ServiceStateRequest Members

        public ACCESS_MASK GetServiceAccessMask()
        {
            return ACCESS_MASK.SERVICE_PAUSE_CONTINUE;
        }

        public bool Request(ServiceStatus ss)
        {
            return ss.Control(SC_CONTROL_CODE.SERVICE_CONTROL_CONTINUE);
        }

        public bool HasSuccess(SC_RUNTIME_STATUS state)
        {
            return state == SC_RUNTIME_STATUS.SERVICE_RUNNING;
        }

        public bool HasFailed(SC_RUNTIME_STATUS state)
        {
            return false;
        }

        #endregion
    }
}
