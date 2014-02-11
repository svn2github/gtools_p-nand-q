using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using pserv4.services;
using pserv4.Properties;

namespace pserv4.devices
{
    public class DevicesDataController : ServicesDataController
    {
        public DevicesDataController()
            :   base(
                    SC_SERVICE_TYPE.SERVICE_KERNEL_DRIVER|SC_SERVICE_TYPE.SERVICE_FILE_SYSTEM_DRIVER,
                    "Devices", 
                    "Device",
                    Resources.DEVICES_CTRL_START_Description,
                    Resources.DEVICES_CTRL_STOP_Description,
                    Resources.DEVICES_CTRL_RESTART_Description,
                    Resources.DEVICES_CTRL_PAUSE_Description,
                    Resources.DEVICES_CTRL_CONTINUE_Description)            
        {
        }
    }
}
