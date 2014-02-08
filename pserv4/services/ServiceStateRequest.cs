using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pserv4.services
{
    public interface ServiceStateRequest 
    {
        ACCESS_MASK GetServiceAccessMask();
        bool Request(ServiceStatus ss);
        bool HasSuccess(SC_RUNTIME_STATUS state);
        bool HasFailed(SC_RUNTIME_STATUS state);
    }

}
