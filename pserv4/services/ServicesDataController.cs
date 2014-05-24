using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Collections;
using pserv4.Properties;
using GSharpTools;
using GSharpTools.WPF;
using log4net;
using System.Reflection;

namespace pserv4.services
{
    public class ServicesDataController : DataController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SC_SERVICE_TYPE ServicesType;

        public string MachineName { get; private set; }

        private bool AnythingPaused;
        private bool AnythingRunning;
        private bool AnythingStopped;

        private static string _(string s, string a)
        {
            return (s == null) ? a : s;
        }

        public ServicesDataController(
                SC_SERVICE_TYPE servicesType = SC_SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS | SC_SERVICE_TYPE.SERVICE_WIN32_SHARE_PROCESS,
                string controllerName = "Services", 
                string itemName = "Service",
                string controlStartDescription = null,
                string controlStopDescription =  null,
                string controlRestartDescription =  null,
                string controlPauseDescription =  null,
                string controlContinueDescription = null
            )
            : base(
                    controllerName, 
                    itemName,
                    _(controlStartDescription, Resources.SERVICES_CTRL_START_Description),
                    _(controlStopDescription, Resources.SERVICES_CTRL_STOP_Description),
                    _(controlRestartDescription,Resources.SERVICES_CTRL_RESTART_Description),
                    _(controlPauseDescription,Resources.SERVICES_CTRL_PAUSE_Description),
                    _(controlContinueDescription,Resources.SERVICES_CTRL_CONTINUE_Description))
        {
            Caption = string.Format(Resources.IDS_SERVICES_CAPTION_LOCAL_MACHINE, ControllerName, Environment.MachineName);
            ServicesType = servicesType;
            HasProperties = true;
            MachineName = Environment.MachineName;
        }

        public override IEnumerable<DataObjectColumn> Columns
        {
            get
            {
                if (ActualColumns == null)
                {
                    CreateColumns(
                        new DataObjectColumn(Resources.SERVICE_C_DisplayName, "DisplayName"),
                        new DataObjectColumn(Resources.SERVICE_C_ServiceName, "InternalID"),
                        new DataObjectColumn(Resources.SERVICE_C_ProcessID, "PID"),
                        new DataObjectColumn(Resources.SERVICE_C_CurrentState, "CurrentStateString"),
                        new DataObjectColumn(Resources.SERVICE_C_User, "User"),
                        new DataObjectColumn(Resources.SERVICE_C_ServiceType, "ServiceTypeString"),
                        new DataObjectColumn(Resources.SERVICE_C_StartType, "StartTypeString"),
                        new DataObjectColumn(Resources.SERVICE_C_BinaryPathName, "BinaryPathName"),
                        new DataObjectColumn(Resources.SERVICE_C_LoadOrderGroup, "LoadOrderGroup"),
                        new DataObjectColumn(Resources.SERVICE_C_ErrorControl, "ErrorControl"),
                        new DataObjectColumn(Resources.SERVICE_C_TagId, "TagId"),
                        new DataObjectColumn(Resources.SERVICE_C_Description, "Description"),
                        new DataObjectColumn(Resources.SERVICE_C_Win32ExitCode, "Win32ExitCode"),
                        new DataObjectColumn(Resources.SERVICE_C_ServiceSpecificExitCode, "ServiceSpecificExitCode"),
                        new DataObjectColumn(Resources.SERVICE_C_CheckPoint, "CheckPoint"),
                        new DataObjectColumn(Resources.SERVICE_C_WaitHint, "WaitHint"),
                        new DataObjectColumn(Resources.SERVICE_C_ServiceFlags, "ServiceFlags"),
                        new DataObjectColumn(Resources.SERVICE_C_ControlsAccepted, "ControlsAcceptedString"),
                        new DataObjectColumn(Resources.SERVICE_C_InstallLocation, "InstallLocation"));
                }
                return ActualColumns;
            }
        }

        
        public override ContextMenu ContextMenu
        {
            get
            {
                ContextMenu menu = base.ContextMenu;

                menu.Items.Add(new Separator());

                AppendMenuItem(menu, Resources.SERVICES_SET_START_AUTOMATIC, "application_go", OnSetStartupAutomatic);
                AppendMenuItem(menu, Resources.SERVICES_SET_START_MANUAL, "application", OnSetStartupManual);
                AppendMenuItem(menu, Resources.SERVICES_SET_START_DISABLED, "application_key", OnSetStartupDisabled);
                menu.Items.Add(new Separator());
                AppendMenuItem(menu, Resources.SERVICES_BRING_UP_REGEDIT, "report_go", OnBringUpRegistryEditor);
                AppendMenuItem(menu, Resources.SERVICES_BRING_UP_EXPLORER, "folder_find", OnBringUpExplorer);
                AppendMenuItem(menu, Resources.SERVICES_START_CMD, "application_xp_terminal", OnBringUpTerminal);
                menu.Items.Add(new Separator());
                AppendMenuItem(menu, Resources.IDS_SERVICES_CONNECT_LOCAL_MACHINE, "computer", OnConnectToLocalMachine);
                AppendMenuItem(menu, Resources.IDS_SERVICES_CONNECT_REMOTE_MACHINE, "computer_add", OnConnectToRemoteMachine);
                menu.Items.Add(new Separator());

                AppendMenuItem(menu, Resources.SERVICES_UNINSTALL, "application_form_delete", OnUnstall);
                AppendMenuItem(menu, Resources.SERVICES_DELETE_REGISTRY, "delete", OnDeleteRegistry);
                return AppendDefaultItems(menu);
            }
        }

        private bool ApplyStartupChanges(SC_START_TYPE startupType)
        {
            using (new WaitCursor())
            {
                bool result = true;
                using (NativeSCManager scm = new NativeSCManager(MachineName))
                {
                    foreach (ServiceDataObject sdo in MainListView.SelectedItems)
                    {
                        if( !sdo.ApplyStartupChanges(scm, startupType) )
                        {
                            result = false;
                        }
                    }
                }
                return result;
            }
        }

        public override void ApplyChanges(List<IDataObjectDetails> changedItems)
        {
            using (new WaitCursor())
            {
                using (NativeSCManager scm = new NativeSCManager(MachineName))
                {
                    foreach (IDataObjectDetails dod in changedItems)
                    {
                        dod.ApplyChanges(scm);
                    }
                }
            }
        }

        public void ConnectToComputer(string name)
        {
            if(!MachineName.Equals(name))
            {
                using(new WaitCursor())
                {
                    MachineName = name;
                    if (name.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
                    {
                        Caption = string.Format(Resources.IDS_SERVICES_CAPTION_LOCAL_MACHINE, ControllerName, name);
                    }
                    else
                    {
                        Caption = string.Format(Resources.IDS_SERVICES_CAPTION_REMOTE_MACHINE, ControllerName, name);
                    }
                    MainWindow.Instance.UpdateTitle();
                    MainWindow.Items.Clear();
                    MainWindow.Instance.RefreshDisplay(null, null);
                }
            }
        }
        
        public void OnConnectToLocalMachine(object sender, RoutedEventArgs e)
        {
            ConnectToComputer(Environment.MachineName);
        }

        public void OnConnectToRemoteMachine(object sender, RoutedEventArgs e)
        {
            ConnectMachineDialog window = new ConnectMachineDialog(MachineName);
            bool? result = window.ShowDialog();
            if( result.HasValue && result.Value)
            {
                ConnectToComputer(window.SelectedMachine);
            }
        }

        public void OnSetStartupAutomatic(object sender, RoutedEventArgs e)
        {
            ApplyStartupChanges(SC_START_TYPE.SERVICE_AUTO_START);
        }

        public void OnSetStartupManual(object sender, RoutedEventArgs e)
        {
            ApplyStartupChanges(SC_START_TYPE.SERVICE_DEMAND_START);
        }

        public void OnSetStartupDisabled(object sender, RoutedEventArgs e)
        {
            ApplyStartupChanges(SC_START_TYPE.SERVICE_DISABLED);
        }


        public override void OnSelectionChanged(IList selectedItems)
        {
            AnythingPaused = false;
            AnythingRunning = false;
            AnythingStopped = false;

            foreach (ServiceDataObject o in selectedItems)
            {
                switch (o.CurrentState)
                {
                    case SC_RUNTIME_STATUS.SERVICE_PAUSED:
                        AnythingPaused = true;
                        break;

                    case SC_RUNTIME_STATUS.SERVICE_RUNNING:
                        AnythingRunning = true;
                        break;

                    case SC_RUNTIME_STATUS.SERVICE_STOPPED:
                        AnythingStopped = true;
                        break;
                }
            }

            IsControlStartEnabled  = AnythingStopped || AnythingPaused;
            IsControlStopEnabled  = AnythingRunning || AnythingPaused;
            IsControlRestartEnabled  = true;
            IsControlPauseEnabled  = AnythingRunning;
            IsControlContinueEnabled  = AnythingPaused;
        }

        public override void OnContextMenuOpening(IList selectedItems, ContextMenu menu)
        {
            SetMenuItemEnabled(menu, 0, IsControlStartEnabled);
            SetMenuItemEnabled(menu, 1, IsControlStopEnabled);
            SetMenuItemEnabled(menu, 2, IsControlRestartEnabled);
            SetMenuItemEnabled(menu, 3, IsControlPauseEnabled);
            SetMenuItemEnabled(menu, 4, IsControlContinueEnabled);
        }


        private ServiceDataObject GetServiceDataObjectByID(string id)
        {
            foreach (ServiceDataObject sdo in MainListView.Items)
            {
                if (id.Equals(sdo.InternalID, StringComparison.OrdinalIgnoreCase))
                {
                    return sdo;
                }
            }
            return null;
        }

        public override void ApplyTemplateInfo(ActionTemplateInfo ati, BackgroundAction action)
        {
            // default implementation: do nothing
            ServiceDataObject sdo = GetServiceDataObjectByID(ati.ID);
            if (sdo == null)
            {
                Log.ErrorFormat("Unable to find object for ID {0}", ati.ID);
            }
            else
            {
                string expectedStartType = ati["StartTypeString"];
                string actualStartType = sdo.StartTypeString;
                if (expectedStartType != actualStartType)
                {
                    Log.InfoFormat("=> StartType for {0} needs to change from {1} to {2}",
                        ati,
                        actualStartType,
                        expectedStartType);

                    using (NativeSCManager scm = new NativeSCManager(MachineName))
                    {
                        SC_START_TYPE startType = ServicesLocalisation.ReverseLocalizedStartType(expectedStartType);
                        if( startType != SC_START_TYPE.SERVICE_NO_CHANGE )
                        {
                            sdo.ApplyStartupChanges(scm, startType);
                        }
                    }
                }
                else
                {
                    Log.InfoFormat("=> StartType for {0} identical, no need to change", ati);
                }
            }
        }

        public void PerformExplicitRequest(ServiceStateRequest ssr, List<string> serviceNames, string title)
        {
            PerformServiceStateRequest pssr = new PerformServiceStateRequest(ssr);
            
            // OK, so this code is not very efficient and I should really be using a dictionary.
            // BUT. I expect one or at max two services to be passed, so who cares?
            foreach (ServiceDataObject sdo in MainListView.Items)
            {
                foreach(string serviceName in serviceNames)
                {
                    if(serviceName.Equals(sdo.InternalID, StringComparison.OrdinalIgnoreCase) )
                    {
                        pssr.Services.Add(sdo);
                        break;
                    }
                }
            }
            if (pssr.Services.Count > 0)
            {
                new LongRunningFunctionWindow(pssr, title).ShowDialog();
            }
        }

        private void OnChangeServiceStatus(ServiceStateRequest ssr, string title)
        {
            PerformServiceStateRequest pssr = new PerformServiceStateRequest(ssr);
            foreach(ServiceDataObject sdo in MainListView.SelectedItems)
            {
                pssr.Services.Add(sdo);
            }
            if( pssr.Services.Count > 0 )
            {
                new LongRunningFunctionWindow(pssr, title).ShowDialog();
            }
        }

        public override void OnControlStart(object sender, RoutedEventArgs e)
        {
            OnChangeServiceStatus(new RequestServiceStart(), Resources.IDS_STARTING_SERVICES);
        }

        public override void OnControlStop(object sender, RoutedEventArgs e)
        {
            OnChangeServiceStatus(new RequestServiceStop(), Resources.IDS_STOPPING_SERVICES);
        }

        public override void OnControlRestart(object sender, RoutedEventArgs e)
        {
            OnChangeServiceStatus(new RequestServiceRestart(), Resources.IDS_RESTARTING_SERVICES);
        }

        public override void OnControlPause(object sender, RoutedEventArgs e)
        {
            OnChangeServiceStatus(new RequestServicePause(), Resources.IDS_PAUSING_SERVICES);
        }

        public override void OnControlContinue(object sender, RoutedEventArgs e)
        {
            OnChangeServiceStatus(new RequestServiceContinue(), Resources.IDS_RESUMING_SERVICES);
        }

        public override UserControl CreateDetailsPage(DataObject o)
        {
            return new ServiceProperties(o as ServiceDataObject);
        }

        private delegate bool ServiceCallback(ServiceDataObject sdo);

        private void DispatchCallback(ServiceCallback callback)
        {
            foreach (ServiceDataObject sdo in MainListView.SelectedItems)
            {
                callback(sdo);
            }
        }

        public void OnBringUpRegistryEditor(object sender, RoutedEventArgs e)
        {
            DispatchCallback((sdo) => { return sdo.ShowRegistryEditor(); });
        }

        public void OnBringUpExplorer(object sender, RoutedEventArgs e)
        {
            DispatchCallback((sdo) => { return sdo.BringUpExplorerInInstallLocation(); });
        }

        public void OnBringUpTerminal(object sender, RoutedEventArgs e)
        {
            DispatchCallback((sdo) => { return sdo.BringUpTerminalInInstallLocation(); });
        }

        public void OnUnstall(object sender, RoutedEventArgs e)
        {
            List<ServiceDataObject> deleteThese = new List<ServiceDataObject>();
            foreach (ServiceDataObject sdo in MainListView.SelectedItems)
            {
                deleteThese.Add(sdo);
            }

            using (NativeSCManager scm = new NativeSCManager(MachineName))
            {
                foreach (ServiceDataObject sdo in deleteThese)
                {
                    MessageBoxResult result = MessageBox.Show(
                        string.Format(Resources.IDS_SERVICE_SureToUninstall, sdo.InternalID, sdo.DisplayName),
                        Resources.IDS_CONFIRMATION,
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (sdo.Uninstall(scm))
                        {
                            MainWindow.Items.Remove(sdo);
                        }
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        break;
                    }
                }
            }
        }

        public void OnDeleteRegistry(object sender, RoutedEventArgs e)
        {
            List<ServiceDataObject> deleteThese = new List<ServiceDataObject>();
            foreach (ServiceDataObject sdo in MainListView.SelectedItems)
            {
                deleteThese.Add(sdo);
            }

            using (NativeSCManager scm = new NativeSCManager(MachineName))
            {
                foreach (ServiceDataObject sdo in deleteThese)
                {
                    MessageBoxResult result = MessageBox.Show(
                        string.Format(Resources.IDS_SERVICE_SureToDeleteRegistry, sdo.InternalID, sdo.DisplayName),
                        Resources.IDS_CONFIRMATION,
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        sdo.RemoveRegistryKey();
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        break;
                    }
                }
            }
        }

        public override void Refresh(ObservableCollection<DataObject> objects)
        {
            using(var manager = new RefreshManager<ServiceDataObject>(objects))
            {
                using (NativeSCManager scm = new NativeSCManager(MachineName))
                {
                    foreach (ENUM_SERVICE_STATUS_PROCESS essp in scm.Refresh(ServicesType))
                    {
                        using (NativeService ns = new NativeService(scm, essp.ServiceName))
                        {
                            ServiceDataObject sdo = null;

                            if (manager.Contains(essp.ServiceName, out sdo))
                            {
                                sdo.UpdateFrom(essp);
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
}

