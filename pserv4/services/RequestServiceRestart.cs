using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using log4net;
using System.Reflection;

namespace pserv4.services
{
    public class RequestServiceRestart : ServiceStateRequest
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool HasBeenAskedToStart;
        private ServiceStatus SS;

        #region ServiceStateRequest Members

        public ACCESS_MASK GetServiceAccessMask()
        {
            return ACCESS_MASK.SERVICE_STOP|ACCESS_MASK.SERVICE_START;
        }

        public bool Request(ServiceStatus ss)
        {
            SS = ss;
            if( ss.Status.CurrentState == SC_RUNTIME_STATUS.SERVICE_STOPPED )
            {
                Log.Info("Restart asks for service to start...");
                if (!SS.Start())
                    return false;

                HasBeenAskedToStart = true;
                return true;
            }
            else
            {
                HasBeenAskedToStart = false;
                return ss.Control(SC_CONTROL_CODE.SERVICE_CONTROL_STOP);
            }
        }

        public bool HasSuccess(SC_RUNTIME_STATUS state)
        {
            Log.InfoFormat("HasSuccess: {0}", state);
            if (SS == null)
                return false;

            if (HasBeenAskedToStart)
                return state == SC_RUNTIME_STATUS.SERVICE_RUNNING;
                

            if (state != SC_RUNTIME_STATUS.SERVICE_STOPPED)
                return false;

            Log.Info("Restart asks for service to start...");
            if (!SS.Start())
                return false;
            
            HasBeenAskedToStart = true;
            return false;
        }

        public bool HasFailed(SC_RUNTIME_STATUS state)
        {
            return false;
        }

        #endregion
    }
}
