using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using pserv4.services;

namespace pserv4.devices
{
    public class DevicesDataController : ServicesDataController
    {
        public DevicesDataController()
            :   base(SC_SERVICE_TYPE.SERVICE_KERNEL_DRIVER|SC_SERVICE_TYPE.SERVICE_FILE_SYSTEM_DRIVER,"Devices", "Device")
        {
        }
    }
}
