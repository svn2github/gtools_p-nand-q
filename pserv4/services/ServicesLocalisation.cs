using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pserv4.Properties;

namespace pserv4.services
{
    public static class ServicesLocalisation
    {
        public static string Localized(SC_START_TYPE st)
        {
            switch (st)
            {
                case SC_START_TYPE.SERVICE_AUTO_START:
                    return pserv4.Properties.Resources.SERVICE_AUTO_START;

                case SC_START_TYPE.SERVICE_BOOT_START:
                    return pserv4.Properties.Resources.SERVICE_BOOT_START;

                case SC_START_TYPE.SERVICE_DEMAND_START:
                    return pserv4.Properties.Resources.SERVICE_DEMAND_START;

                case SC_START_TYPE.SERVICE_DISABLED:
                    return pserv4.Properties.Resources.SERVICE_DISABLED;

                case SC_START_TYPE.SERVICE_SYSTEM_START:
                    return pserv4.Properties.Resources.SERVICE_SYSTEM_START;

                default:
                    return st.ToString();
            }
        }

        public static SC_START_TYPE ReverseLocalizedStartType(string input)
        {
            if( Resources.SERVICE_AUTO_START.Equals(input) )
                return SC_START_TYPE.SERVICE_AUTO_START;

            if( Resources.SERVICE_BOOT_START.Equals(input) )
                return SC_START_TYPE.SERVICE_BOOT_START;
            
            if( Resources.SERVICE_DEMAND_START.Equals(input) )
                return SC_START_TYPE.SERVICE_DEMAND_START;
            
            if( Resources.SERVICE_DISABLED.Equals(input) )
                return SC_START_TYPE.SERVICE_DISABLED;
            
            if( Resources.SERVICE_SYSTEM_START.Equals(input) )
                return SC_START_TYPE.SERVICE_SYSTEM_START;

            return SC_START_TYPE.SERVICE_NO_CHANGE;
        }

        public static string Localized(SC_ERROR_CONTROL ec)
        {
            switch (ec)
            {
                case SC_ERROR_CONTROL.SERVICE_ERROR_NORMAL:
                    return pserv4.Properties.Resources.SERVICE_ERROR_NORMAL;

                case SC_ERROR_CONTROL.SERVICE_ERROR_SEVERE:
                    return pserv4.Properties.Resources.SERVICE_ERROR_SEVERE;

                case SC_ERROR_CONTROL.SERVICE_ERROR_IGNORE:
                    return pserv4.Properties.Resources.SERVICE_ERROR_IGNORE;

                case SC_ERROR_CONTROL.SERVICE_ERROR_CRITICAL:
                    return pserv4.Properties.Resources.SERVICE_ERROR_CRITICAL;

                default:
                    return ec.ToString();
            }
        }


        private static Dictionary<SC_SERVICE_TYPE, string> SC_SERVICE_TYPES;

        public static string Localized(SC_SERVICE_TYPE st)
        {
            if( SC_SERVICE_TYPES == null )
            {
                SC_SERVICE_TYPES = new Dictionary<SC_SERVICE_TYPE,string>();
                SC_SERVICE_TYPES[SC_SERVICE_TYPE.SERVICE_KERNEL_DRIVER] = pserv4.Properties.Resources.SERVICE_KERNEL_DRIVER;
                SC_SERVICE_TYPES[SC_SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS] = pserv4.Properties.Resources.SERVICE_WIN32_OWN_PROCESS;
                SC_SERVICE_TYPES[SC_SERVICE_TYPE.SERVICE_WIN32_SHARE_PROCESS] = pserv4.Properties.Resources.SERVICE_WIN32_SHARE_PROCESS;
                SC_SERVICE_TYPES[SC_SERVICE_TYPE.SERVICE_FILE_SYSTEM_DRIVER] = pserv4.Properties.Resources.SERVICE_FILE_SYSTEM_DRIVER;
                SC_SERVICE_TYPES[SC_SERVICE_TYPE.SERVICE_KERNEL_DRIVER] = pserv4.Properties.Resources.SERVICE_KERNEL_DRIVER;
            }
            StringBuilder result = new StringBuilder();
            bool first = true;
            foreach(SC_SERVICE_TYPE k in SC_SERVICE_TYPES.Keys )
            {
                if ((st & k) != 0)
                {
                    st &= ~(k);
                    if (first)
                        first = false;
                    else
                        result.Append('|');

                    result.Append(SC_SERVICE_TYPES[k]);
                    if (st == 0)
                        break;
                }
            }
            return result.ToString();
        }

        private static Dictionary<SC_CONTROLS_ACCEPTED, string> LOOKUP_SC_CONTROLS_ACCEPTED;

        public static string Localized(SC_CONTROLS_ACCEPTED ca)
        {
            if( LOOKUP_SC_CONTROLS_ACCEPTED == null )
            {
                LOOKUP_SC_CONTROLS_ACCEPTED = new Dictionary<SC_CONTROLS_ACCEPTED,string>();
                LOOKUP_SC_CONTROLS_ACCEPTED[SC_CONTROLS_ACCEPTED.SERVICE_ACCEPT_PAUSE_CONTINUE] = pserv4.Properties.Resources.SERVICE_ACCEPT_PAUSE_CONTINUE;
                LOOKUP_SC_CONTROLS_ACCEPTED[SC_CONTROLS_ACCEPTED.SERVICE_ACCEPT_STOP] = pserv4.Properties.Resources.SERVICE_ACCEPT_STOP;
            }
            StringBuilder result = new StringBuilder();
            bool first = true;
            foreach(SC_CONTROLS_ACCEPTED k in LOOKUP_SC_CONTROLS_ACCEPTED.Keys )
            {
                if ((ca & k) != 0)
                {
                    ca &= ~(k);
                    if (first)
                        first = false;
                    else
                        result.Append('|');

                    result.Append(LOOKUP_SC_CONTROLS_ACCEPTED[k]);
                    if (ca == 0)
                        break;
                }
            }
            return result.ToString();
        }

        

        public static string Localized(SC_RUNTIME_STATUS rs)
        {
            switch (rs)
            {
                case SC_RUNTIME_STATUS.SERVICE_STOPPED:
                    return pserv4.Properties.Resources.SERVICE_STOPPED;

                case SC_RUNTIME_STATUS.SERVICE_STOP_PENDING:
                    return pserv4.Properties.Resources.SERVICE_STOP_PENDING;

                case SC_RUNTIME_STATUS.SERVICE_START_PENDING:
                    return pserv4.Properties.Resources.SERVICE_START_PENDING;

                case SC_RUNTIME_STATUS.SERVICE_RUNNING:
                    return pserv4.Properties.Resources.SERVICE_RUNNING;

                case SC_RUNTIME_STATUS.SERVICE_PAUSED:
                    return pserv4.Properties.Resources.SERVICE_PAUSED;

                case SC_RUNTIME_STATUS.SERVICE_PAUSE_PENDING:
                    return pserv4.Properties.Resources.SERVICE_PAUSE_PENDING;

                case SC_RUNTIME_STATUS.SERVICE_CONTINUE_PENDING:
                    return pserv4.Properties.Resources.SERVICE_CONTINUE_PENDING;

                default:
                    return rs.ToString();
            }
        }
    
    }
}
