using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pserv4.services
{
    public class ServiceDataObject : IObject
    {
        public string DisplayName { get; private set; }
        public string ServiceName { get; private set; }
        public string ServiceType { get; private set; }
        public string PID { get; private set; }
        public string StartType { get; private set; }
        public string BinaryPathName { get; private set; }
        public string LoadOrderGroup { get; private set; }
        public string ErrorControl { get; private set; }
        public string TagId { get; private set; }
        public string Description { get; private set; }
        public string User { get; private set; }

        public string ControlsAcceptedString
        {
            get
            {
                return ServicesLocalisation.Localized(ControlsAccepted);
            }
        }

        public string Win32ExitCode { get; private set; }
        public string ServiceSpecificExitCode { get; private set; }
        public string CheckPoint { get; private set; }
        public string WaitHint { get; private set; }
        public string ServiceFlags { get; private set; }

        public string CurrentStateString
        { 
            get
            {
                return ServicesLocalisation.Localized(CurrentState);
            }
        }

        public bool IsDisabled { get; private set; }
        public bool IsRunning { get; private set; }

        private SC_RUNTIME_STATUS CurrentState;
        private SC_CONTROLS_ACCEPTED ControlsAccepted;

        public ServiceDataObject(NativeService service, ENUM_SERVICE_STATUS_PROCESS essp)
        {
            DisplayName = essp.DisplayName;
            ServiceName = essp.ServiceName;
            CurrentState = essp.CurrentState;
            ControlsAccepted = essp.ControlsAccepted;

            Win32ExitCode = essp.Win32ExitCode.ToString();
            ServiceSpecificExitCode = essp.ServiceSpecificExitCode.ToString();
            CheckPoint = essp.CheckPoint.ToString();
            WaitHint = essp.WaitHint.ToString();
            ServiceFlags = essp.ServiceFlags.ToString();

            ServiceType = ServicesLocalisation.Localized(essp.ServiceType);
            if (essp.ProcessID != 0)
                PID = essp.ProcessID.ToString();
            QUERY_SERVICE_CONFIG config = service.ServiceConfig;

            if (essp.CurrentState == SC_RUNTIME_STATUS.SERVICE_RUNNING)
            {
                IsRunning = true;
            }

            if (config != null)
            {
                if (config.StartType == SC_START_TYPE.SERVICE_DISABLED)
                {
                    IsDisabled = true;
                }
                StartType = ServicesLocalisation.Localized(config.StartType);
                BinaryPathName = config.BinaryPathName;
                LoadOrderGroup = config.LoadOrderGroup;
                ErrorControl = ServicesLocalisation.Localized(config.ErrorControl);
                TagId = config.TagId.ToString();
                User = config.ServiceStartName;

            }
            Description = service.Description;
            


        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

}
