using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pserv4
{
    public enum MainViewType : int
    {
        Invalid = -1,
        Services = 0,
        Devices,
        Windows,
        Programs,
        Processes,
        Modules
    }
}
