using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace pserv4.services
{
    public class ServicesDataController : DataController
    {
        private static List<DataObjectColumn> ActualColumns;
        private readonly SC_SERVICE_TYPE ServicesType;

        public ServicesDataController(SC_SERVICE_TYPE servicesType = SC_SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS | SC_SERVICE_TYPE.SERVICE_WIN32_SHARE_PROCESS,
                string controllerName = "Services", string itemName = "Service")
            : base(controllerName, itemName)
        {
            ServicesType = servicesType;
        }

        public override IEnumerable<DataObjectColumn> Columns
        {
            get
            {
                if (ActualColumns == null)
                {
                    ActualColumns = new List<DataObjectColumn>();
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_DisplayName, "DisplayName"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_ServiceName, "InternalID"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_ServiceType, "ServiceType"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_ProcessID, "PID"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_StartType, "StartType"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_BinaryPathName, "BinaryPathName"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_LoadOrderGroup, "LoadOrderGroup"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_ErrorControl, "ErrorControl"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_TagId, "TagId"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_Description, "Description"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_User, "User"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_CurrentState, "CurrentStateString"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_Win32ExitCode, "Win32ExitCode"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_ServiceSpecificExitCode, "ServiceSpecificExitCode"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_CheckPoint, "CheckPoint"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_WaitHint, "WaitHint"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_ServiceFlags, "ServiceFlags"));
                    ActualColumns.Add(new DataObjectColumn(pserv4.Properties.Resources.SERVICE_C_ControlsAccepted, "ControlsAcceptedString"));
                }
                return ActualColumns;
            }
        }

        private void AppendMenuItem(ContextMenu menu, string header, string image, RoutedEventHandler callback)
        {
            MenuItem mi = new MenuItem();
            mi.Header = header;
            if( !string.IsNullOrEmpty(image))
            {
                Image i = new Image();
                string filename = string.Format(@"pack://application:,,,/images/{0}", image);
                Trace.TraceInformation("filename: {0}", filename);

                i.Source = new BitmapImage(new Uri(filename));
                mi.Icon = i;
            }
            mi.Click += callback;
            menu.Items.Add(mi);
        }

        public override ContextMenu ContextMenu
        {
            get
            {
                ContextMenu menu = new ContextMenu();

                AppendMenuItem(menu, pserv4.Properties.Resources.SERVICE_CONTROL_Start, "control_play_blue.png", OnStartService);
                AppendMenuItem(menu, pserv4.Properties.Resources.SERVICE_CONTROL_Stop, "control_stop_blue.png", OnStopService);
                AppendMenuItem(menu, pserv4.Properties.Resources.SERVICE_CONTROL_Restart, "control_repeat_blue.png", OnRestartService);
                AppendMenuItem(menu, pserv4.Properties.Resources.SERVICE_CONTROL_Pause, "control_pause_blue.png", OnPauseService);
                AppendMenuItem(menu, pserv4.Properties.Resources.SERVICE_CONTROL_Continue, "control_fastforward_blue.png", OnContinueService);

                return menu;
            }
        }

        private enum Possibility
        {
            Unknown,
            CanRun,
            CannotRun,
            CanBoth
        }

        public override void OnContextMenuOpening(System.Collections.IList selectedItems, ContextMenu menu)
        {
            Possibility canStartService = Possibility.Unknown;
            Possibility canStopService = Possibility.Unknown;
            Possibility canRestartService = Possibility.Unknown;
            Possibility canPauseService = Possibility.Unknown;
            Possibility canContinueService = Possibility.Unknown;

            foreach (ServiceDataObject o in selectedItems)
            {
                switch(o.CurrentState)
                {
                    case SC_RUNTIME_STATUS.SERVICE_PAUSED:
                        if (canPauseService == Possibility.Unknown)
                            canPauseService = Possibility.CannotRun;
                        else if (canPauseService != Possibility.CannotRun)
                            canPauseService = Possibility.CanBoth;

                        if (canContinueService == Possibility.Unknown)
                            canContinueService = Possibility.CanRun;
                        else if (canContinueService != Possibility.CanRun)
                            canContinueService = Possibility.CanBoth;
                        break;

                    case SC_RUNTIME_STATUS.SERVICE_RUNNING:
                        if (canStartService == Possibility.Unknown)
                            canStartService = Possibility.CannotRun;
                        else if (canStartService != Possibility.CannotRun)
                            canStartService = Possibility.CanBoth;

                        if (canContinueService == Possibility.Unknown)
                            canContinueService = Possibility.CannotRun;
                        else if (canContinueService != Possibility.CannotRun)
                            canContinueService = Possibility.CanBoth;
                        break;

                    case SC_RUNTIME_STATUS.SERVICE_STOPPED:
                        if (canStopService == Possibility.Unknown)
                            canStopService = Possibility.CannotRun;
                        else if (canStopService != Possibility.CannotRun)
                            canStopService = Possibility.CanBoth;

                        if (canRestartService == Possibility.Unknown)
                            canRestartService = Possibility.CannotRun;
                        else if (canRestartService != Possibility.CannotRun)
                            canRestartService = Possibility.CanBoth;

                        if (canContinueService == Possibility.Unknown)
                            canContinueService = Possibility.CannotRun;
                        else if (canContinueService != Possibility.CannotRun)
                            canContinueService = Possibility.CanBoth;

                        if (canPauseService == Possibility.Unknown)
                            canPauseService = Possibility.CannotRun;
                        else if (canPauseService != Possibility.CannotRun)
                            canPauseService = Possibility.CanBoth;
                            
                        break;
                }
            }


            SetMenuItemEnabled(menu, 0, canStartService);
            SetMenuItemEnabled(menu, 1, canStopService);
            SetMenuItemEnabled(menu, 2, canRestartService);
            SetMenuItemEnabled(menu, 3, canPauseService);
            SetMenuItemEnabled(menu, 4, canContinueService);

        }

        private void SetMenuItemEnabled(ContextMenu menu, int index, Possibility p)
        {
            MenuItem mi = menu.Items[index] as MenuItem;
            if(mi != null)
            {
                mi.IsEnabled = (p != Possibility.CannotRun);
            }
        }

        private void OnChangeServiceStatus(ServiceStateRequest ssr)
        {
            PerformServiceStateRequest pssr = new PerformServiceStateRequest(ssr);
            foreach(ServiceDataObject sdo in MainListView.SelectedItems)
            {
                pssr.Services.Add(sdo);
            }
            if( pssr.Services.Count > 0 )
            {
                new LongRunningFunctionWindow(pssr).ShowDialog();
            }
        }

        private void OnStartService(object sender, RoutedEventArgs e)
        {
            OnChangeServiceStatus(new RequestServiceStartup());
        }

        private void OnStopService(object sender, RoutedEventArgs e)
        {
            OnChangeServiceStatus(new RequestServiceShutdown());
        }

        private void OnRestartService(object sender, RoutedEventArgs e)
        {
            OnChangeServiceStatus(new RequestServiceRestart());
        }

        private void OnPauseService(object sender, RoutedEventArgs e)
        {
            OnChangeServiceStatus(new RequestServicePause());
        }

        private void OnContinueService(object sender, RoutedEventArgs e)
        {
            OnChangeServiceStatus(new RequestServiceContinue());
        }

        public override void Refresh(ObservableCollection<DataObject> objects)
        {
            Dictionary<string, ServiceDataObject> existingObjects = new Dictionary<string, ServiceDataObject>();

            foreach (DataObject o in objects)
            {
                ServiceDataObject sdo = o as ServiceDataObject;
                if (sdo != null)
                {
                    existingObjects[sdo.InternalID] = sdo;
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

