using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace pserv4.services
{
    public class PerformServiceStateRequest : BackgroundAction
    {
        private readonly ServiceStateRequest SSR;
        public readonly List<ServiceDataObject> Services = new List<ServiceDataObject>();

        public PerformServiceStateRequest(ServiceStateRequest ssr)
        {
            SSR = ssr;
        }

        public override void DoWork()
        {
            ACCESS_MASK ServiceAccessMask = SSR.GetServiceAccessMask() | ACCESS_MASK.STANDARD_RIGHTS_READ | ACCESS_MASK.SERVICE_QUERY_STATUS;

            using (NativeSCManager scm = new NativeSCManager())
            {
                int serviceIndex = 0;
                foreach (ServiceDataObject so in Services)
                {
                    ++serviceIndex;

                    try
                    {
                        SetOutputText(string.Format("Service {0}/{1}: {2} is initially in state {3}",
                                        serviceIndex,
                                        Services.Count,
                                        so.DisplayName,
                                        ServicesLocalisation.Localized(so.CurrentState)));

                        using (NativeService ns = new NativeService(scm, so.InternalID, ServiceAccessMask))
                        {
                            bool requestedStatusChange = false;

                            Trace.TraceInformation("BEGIN backgroundWorker1_Process for {0}", ns.Description);
                            using (ServiceStatus ss = new ServiceStatus(ns))
                            {
                                for (int i = 0; i < 100; ++i)
                                {
                                    if (IsCancelled)
                                        break;

                                    if (!ss.Refresh())
                                        break;

                                    SetOutputText(string.Format("Service {0}/{1}: {2} is now in state {3}",
                                        serviceIndex,
                                        Services.Count,
                                        so.DisplayName, 
                                        ServicesLocalisation.Localized(ss.Status.CurrentState)));
                                    
                                    if (SSR.HasSuccess(ss.Status.CurrentState))
                                    {
                                        Trace.WriteLine("Reached target status, done...");
                                        break; // TODO: reached 100% of this service' status reqs. 
                                    }

                                    // if we haven't asked the service to change its status yet, do so now. 
                                    if (!requestedStatusChange)
                                    {
                                        requestedStatusChange = true;
                                        Trace.TraceInformation("Ask {0} to issue its status request on {1}", SSR, ss);
                                        if (!SSR.Request(ss))
                                            break;
                                    }
                                    else if (SSR.HasFailed(ss.Status.CurrentState))
                                    {
                                        Trace.TraceInformation("ERROR, target state is one of the failed ones :(");
                                        break;
                                    }
                                    Thread.Sleep(500);
                                }
                                so.UpdateFrom(ss.Status);
                                Trace.TraceInformation("END backgroundWorker1_Process");
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    if (IsCancelled)
                        break;
                }
            }
        }
    }
}
