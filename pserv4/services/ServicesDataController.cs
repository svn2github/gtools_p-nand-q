using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace pserv4.services
{
    public class ServicesDataController : IObjectController
    {
        private static List<ObjectColumn> ActualColumns;
        private readonly SC_SERVICE_TYPE ServicesType;

        public ServicesDataController(SC_SERVICE_TYPE servicesType = SC_SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS | SC_SERVICE_TYPE.SERVICE_WIN32_SHARE_PROCESS)
        {
            ServicesType = servicesType;
        }

        public IEnumerable<ObjectColumn> Columns
        {
            get
            {
                if (ActualColumns == null)
                {
                    ActualColumns = new List<ObjectColumn>();
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_DisplayName, "DisplayName"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_ServiceName, "ServiceName"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_ServiceType, "ServiceType"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_ProcessID, "PID"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_StartType, "StartType"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_BinaryPathName, "BinaryPathName"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_LoadOrderGroup, "LoadOrderGroup"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_ErrorControl, "ErrorControl"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_TagId, "TagId"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_Description, "Description"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_User, "User"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_CurrentState, "CurrentStateString"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_Win32ExitCode, "Win32ExitCode"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_ServiceSpecificExitCode, "ServiceSpecificExitCode"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_CheckPoint, "CheckPoint"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_WaitHint, "WaitHint"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_ServiceFlags, "ServiceFlags"));
                    ActualColumns.Add(new ObjectColumn(pserv4.Properties.Resources.SERVICE_C_ControlsAccepted, "ControlsAcceptedString"));
                }
                return ActualColumns;
            }
        }

        public void Refresh(ObservableCollection<IObject> objects)
        {
            Dictionary<string, ServiceDataObject> existingObjects = new Dictionary<string, ServiceDataObject>();

            foreach (IObject o in objects)
            {
                ServiceDataObject sdo = o as ServiceDataObject;
                if (sdo != null)
                {
                    existingObjects[sdo.ServiceName] = sdo;
                }
            }

            using (NativeSCManager scm = new NativeSCManager())
            {
                foreach (ENUM_SERVICE_STATUS_PROCESS essp in scm.Refresh(ServicesType))
                {
                    using (NativeService ns = new NativeService(scm, essp.ServiceName))
                    {
                        ServiceDataObject sdo = null;

                        if (existingObjects.TryGetValue(essp.ServiceName, out sdo))
                        {
                            // todo: refresh existing instance from updated data
                        }
                        else
                        {
                            objects.Add(new ServiceDataObject(ns, essp));
                        }
                    }

                }
            }
        }
    }
}
