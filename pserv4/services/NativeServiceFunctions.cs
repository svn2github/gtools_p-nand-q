using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace pserv4.services
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class QUERY_SERVICE_CONFIG
    {
        public SC_SERVICE_TYPE ServiceType;
        public SC_START_TYPE StartType;
        public SC_ERROR_CONTROL ErrorControl;
        public string BinaryPathName;
        public string LoadOrderGroup;
        public int TagId;
        public string Dependencies;
        public string ServiceStartName;
        public string DisplayName;
    }

    [Flags]
    public enum SC_CONTROL_CODE : uint
    {
        /// <summary>
        /// Notifies a paused service that it should resume. The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
        /// </summary>
        SERVICE_CONTROL_CONTINUE = 0x00000003,

        /// <summary>
        /// Notifies a service that it should report its current status information to the service control manager. The hService handle must have the SERVICE_INTERROGATE access right. 
        /// Note that this control is not generally useful as the SCM is aware of the current state of the service.
        /// </summary>
        SERVICE_CONTROL_INTERROGATE = 0x00000004,

        /// <summary>
        /// Notifies a network service that there is a new component for binding. The hService handle must have the SERVICE_PAUSE_CONTINUE access right. However, this control code has been deprecated; use Plug and Play functionality instead. 
        /// </summary>
        SERVICE_CONTROL_NETBINDADD = 0x00000007,

        /// <summary>
        /// Notifies a network service that one of its bindings has been disabled. The hService handle must have the SERVICE_PAUSE_CONTINUE access right. However, this control code has been deprecated; use Plug and Play functionality instead. 
        /// </summary>
        SERVICE_CONTROL_NETBINDDISABLE = 0x0000000A,

        /// <summary>
        /// Notifies a network service that a disabled binding has been enabled. The hService handle must have the SERVICE_PAUSE_CONTINUE access right. However, this control code has been deprecated; use Plug and Play functionality instead. 
        /// </summary>
        SERVICE_CONTROL_NETBINDENABLE = 0x00000009,

        /// <summary>
        /// Notifies a network service that a component for binding has been removed. The hService handle must have the SERVICE_PAUSE_CONTINUE access right. However, this control code has been deprecated; use Plug and Play functionality instead. 
        /// </summary>
        SERVICE_CONTROL_NETBINDREMOVE = 0x00000008,

        /// <summary>
        /// Notifies a service that its startup parameters have changed. The hService handle must have the SERVICE_PAUSE_CONTINUE access right. 
        /// </summary>
        SERVICE_CONTROL_PARAMCHANGE = 0x00000006,

        /// <summary>
        /// Notifies a service that it should pause. The hService handle must have the SERVICE_PAUSE_CONTINUE access right.
        /// </summary>
        SERVICE_CONTROL_PAUSE = 0x00000002,

        SERVICE_CONTROL_STOP = 0x00000001,
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class SERVICE_DESCRIPTION
    {
        public string Description;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class SERVICE_STATUS
    {
        public SC_SERVICE_TYPE ServiceType;
        public SC_RUNTIME_STATUS CurrentState;
        public SC_CONTROLS_ACCEPTED ControlsAccepted;
        public int Win32ExitCode;
        public int ServiceSpecificExitCode;
        public int CheckPoint;
        public int WaitHint;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class SERVICE_STATUS_PROCESS : SERVICE_STATUS
    {
        public int ProcessID;
        public SC_SERVICE_FLAGS ServiceFlags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class ENUM_SERVICE_STATUS_PROCESS
    {
        public string ServiceName;
        public string DisplayName;
        public SC_SERVICE_TYPE ServiceType;
        public SC_RUNTIME_STATUS CurrentState;
        public SC_CONTROLS_ACCEPTED ControlsAccepted;
        public int Win32ExitCode;
        public int ServiceSpecificExitCode;
        public int CheckPoint;
        public int WaitHint;
        public int ProcessID;
        public int ServiceFlags;
    }

    [Flags]
    public enum SC_STATUS_TYPE : uint
    {
        SC_STATUS_PROCESS_INFO      = 0
    }

    [Flags]
    public enum SC_SERVICE_CONFIG : uint
    {
        /// <summary>
        /// The lpInfo parameter is a pointer to a SERVICE_DELAYED_AUTO_START_INFO structure.
        /// </summary>
        SERVICE_CONFIG_DELAYED_AUTO_START_INFO = 3,

        /// <summary>
        /// The lpInfo parameter is a pointer to a SERVICE_DESCRIPTION structure.
        /// </summary>
        SERVICE_CONFIG_DESCRIPTION = 1,

        /// <summary>
        /// The lpInfo parameter is a pointer to a SERVICE_FAILURE_ACTIONS structure.
        /// </summary>
        SERVICE_CONFIG_FAILURE_ACTIONS = 2,

        /// <summary>
        /// The lpInfo parameter is a pointer to a SERVICE_FAILURE_ACTIONS_FLAG structure.
        /// </summary>
        SERVICE_CONFIG_FAILURE_ACTIONS_FLAG = 4,

        /// <summary>
        /// The lpInfo parameter is a pointer to a SERVICE_PREFERRED_NODE_INFO structure.
        /// </summary>
        SERVICE_CONFIG_PREFERRED_NODE = 9,

        /// <summary>
        /// The lpInfo parameter is a pointer to a SERVICE_PRESHUTDOWN_INFO structure.
        /// </summary>
        SERVICE_CONFIG_PRESHUTDOWN_INFO = 7,

        /// <summary>
        /// The lpInfo parameter is a pointer to a SERVICE_REQUIRED_PRIVILEGES_INFO structure.
        /// </summary>
        SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO = 6,

        /// <summary>
        /// The lpInfo parameter is a pointer to a SERVICE_SID_INFO structure.
        /// </summary>
        SERVICE_CONFIG_SERVICE_SID_INFO = 5,

        /// <summary>
        /// The lpInfo parameter is a pointer to a SERVICE_TRIGGER_INFO  structure.
        /// </summary>
        SERVICE_CONFIG_TRIGGER_INFO = 8
    }

    [Flags]
    public enum SC_SERVICE_FLAGS : uint
    {
        /// <summary>
        /// The service is running in a process that is not a system process, or it is not running. 
        /// If the service is running in a process that is not a system process, dwProcessId is nonzero. If the service is not running, dwProcessId is zero.
        /// </summary>
        DEFAULT = 0x00000000,

        /// <summary>
        /// The service runs in a system process that must always be running.
        /// </summary>
        SERVICE_RUNS_IN_SYSTEM_PROCESS = 0x00000001
    }

    [Flags]
    public enum SC_CONTROLS_ACCEPTED : uint
    {
        /// <summary>
        /// The service can be paused and continued.
        /// </summary>
        SERVICE_ACCEPT_PAUSE_CONTINUE = 0x00000002,

        /// <summary>
        /// The service can be stopped
        /// </summary>
        SERVICE_ACCEPT_STOP = 0x00000001,
    }

    [Flags]
    public enum ACCESS_MASK : uint
    {
        DELETE = 0x00010000,
        READ_CONTROL = 0x00020000,
        WRITE_DAC = 0x00040000,
        WRITE_OWNER = 0x00080000,
        SYNCHRONIZE = 0x00100000,

        STANDARD_RIGHTS_REQUIRED = 0x000f0000,

        STANDARD_RIGHTS_READ = 0x00020000,
        STANDARD_RIGHTS_WRITE = 0x00020000,
        STANDARD_RIGHTS_EXECUTE = 0x00020000,
        STANDARD_RIGHTS_ALL = 0x001f0000,
        SPECIFIC_RIGHTS_ALL = 0x0000ffff,
        ACCESS_SYSTEM_SECURITY = 0x01000000,
        MAXIMUM_ALLOWED = 0x02000000,

        GENERIC_READ = 0x80000000,
        GENERIC_WRITE = 0x40000000,
        GENERIC_EXECUTE = 0x20000000,
        GENERIC_ALL = 0x10000000,

        DESKTOP_READOBJECTS = 0x00000001,
        DESKTOP_CREATEWINDOW = 0x00000002,
        DESKTOP_CREATEMENU = 0x00000004,
        DESKTOP_HOOKCONTROL = 0x00000008,
        DESKTOP_JOURNALRECORD = 0x00000010,
        DESKTOP_JOURNALPLAYBACK = 0x00000020,
        DESKTOP_ENUMERATE = 0x00000040,
        DESKTOP_WRITEOBJECTS = 0x00000080,
        DESKTOP_SWITCHDESKTOP = 0x00000100,

        WINSTA_ENUMDESKTOPS = 0x00000001,
        WINSTA_READATTRIBUTES = 0x00000002,
        WINSTA_ACCESSCLIPBOARD = 0x00000004,
        WINSTA_CREATEDESKTOP = 0x00000008,
        WINSTA_WRITEATTRIBUTES = 0x00000010,
        WINSTA_ACCESSGLOBALATOMS = 0x00000020,
        WINSTA_EXITWINDOWS = 0x00000040,
        WINSTA_ENUMERATE = 0x00000100,
        WINSTA_READSCREEN = 0x00000200,

        WINSTA_ALL_ACCESS = 0x0000037f,

        SERVICE_QUERY_CONFIG           = 0x0001,
        SERVICE_CHANGE_CONFIG          = 0x0002,
        SERVICE_QUERY_STATUS           = 0x0004,
        SERVICE_ENUMERATE_DEPENDENTS   = 0x0008,
        SERVICE_START                  = 0x0010,
        SERVICE_STOP                   = 0x0020,
        SERVICE_PAUSE_CONTINUE         = 0x0040,
        SERVICE_INTERROGATE            = 0x0080,
        SERVICE_USER_DEFINED_CONTROL   = 0x0100,

        SERVICE_ALL_ACCESS             = (STANDARD_RIGHTS_REQUIRED     | 
                                        SERVICE_QUERY_CONFIG         | 
                                        SERVICE_CHANGE_CONFIG        | 
                                        SERVICE_QUERY_STATUS         | 
                                        SERVICE_ENUMERATE_DEPENDENTS | 
                                        SERVICE_START                | 
                                        SERVICE_STOP                 | 
                                        SERVICE_PAUSE_CONTINUE       | 
                                        SERVICE_INTERROGATE          | 
                                        SERVICE_USER_DEFINED_CONTROL)

    }

    [Flags]
    public enum SCM_ACCESS : uint
    {
        /// <summary>
        /// Required to connect to the service control manager.
        /// </summary>
        SC_MANAGER_CONNECT = 0x00001,

        /// <summary>
        /// Required to call the CreateService function to create a service
        /// object and add it to the database.
        /// </summary>
        SC_MANAGER_CREATE_SERVICE = 0x00002,

        /// <summary>
        /// Required to call the EnumServicesStatusEx function to list the
        /// services that are in the database.
        /// </summary>
        SC_MANAGER_ENUMERATE_SERVICE = 0x00004,

        /// <summary>
        /// Required to call the LockServiceDatabase function to acquire a
        /// lock on the database.
        /// </summary>
        SC_MANAGER_LOCK = 0x00008,

        /// <summary>
        /// Required to call the QueryServiceLockStatus function to retrieve
        /// the lock status information for the database.
        /// </summary>
        SC_MANAGER_QUERY_LOCK_STATUS = 0x00010,

        /// <summary>
        /// Required to call the NotifyBootConfigStatus function.
        /// </summary>
        SC_MANAGER_MODIFY_BOOT_CONFIG = 0x00020,

        /// <summary>
        /// Includes STANDARD_RIGHTS_REQUIRED, in addition to all access
        /// rights in this table.
        /// </summary>
        SC_MANAGER_ALL_ACCESS = ACCESS_MASK.STANDARD_RIGHTS_REQUIRED |
            SC_MANAGER_CONNECT |
            SC_MANAGER_CREATE_SERVICE |
            SC_MANAGER_ENUMERATE_SERVICE |
            SC_MANAGER_LOCK |
            SC_MANAGER_QUERY_LOCK_STATUS |
            SC_MANAGER_MODIFY_BOOT_CONFIG,

        GENERIC_READ = ACCESS_MASK.STANDARD_RIGHTS_READ |
            SC_MANAGER_ENUMERATE_SERVICE |
            SC_MANAGER_QUERY_LOCK_STATUS,

        GENERIC_WRITE = ACCESS_MASK.STANDARD_RIGHTS_WRITE |
            SC_MANAGER_CREATE_SERVICE |
            SC_MANAGER_MODIFY_BOOT_CONFIG,

        GENERIC_EXECUTE = ACCESS_MASK.STANDARD_RIGHTS_EXECUTE |
            SC_MANAGER_CONNECT | SC_MANAGER_LOCK,

        GENERIC_ALL = SC_MANAGER_ALL_ACCESS,
    }

    [Flags]
    public enum SC_ENUM_TYPE : uint
    {
        SC_ENUM_PROCESS_INFO = 0
    }

    [Flags]
    public enum SC_START_TYPE : uint
    {
        /// <summary>
        /// A service started automatically by the service control manager during system startup
        /// </summary>
        SERVICE_AUTO_START = 0x00000002,

        /// <summary>
        /// A device driver started by the system loader. This value is valid only for driver services.
        /// </summary>
        SERVICE_BOOT_START = 0x00000000,

        /// <summary>
        /// A service started by the service control manager when a process calls the StartService function.
        /// </summary>
        SERVICE_DEMAND_START = 0x00000003,

        /// <summary>
        /// A service that cannot be started. Attempts to start the service result in the error code ERROR_SERVICE_DISABLED.
        /// </summary>
        SERVICE_DISABLED = 0x00000004,

        /// <summary>
        /// A device driver started by the IoInitSystem function. This value is valid only for driver services
        /// </summary>
        SERVICE_SYSTEM_START = 0x00000001,

        SERVICE_NO_CHANGE = 0xffffffff
    }

    [Flags]
    public enum SC_ERROR_CONTROL : uint
    {
        /// <summary>
        /// The startup program logs the error in the event log, if possible. If the last-known good configuration is being started, the startup operation fails. Otherwise, the system is restarted with the last-known good configuration.
        /// </summary>
        SERVICE_ERROR_CRITICAL = 0x00000003,

        /// <summary>
        /// The startup program ignores the error and continues the startup operation.
        /// </summary>
        SERVICE_ERROR_IGNORE = 0x00000000,

        /// <summary>
        /// The startup program logs the error in the event log and continues the startup operation.
        /// </summary>
        SERVICE_ERROR_NORMAL = 0x00000001,

        /// <summary>
        /// The startup program logs the error in the event log. If the last-known good configuration is being started, the startup operation continues. Otherwise, the system is restarted with the last-known-good configuration.
        /// </summary>
        SERVICE_ERROR_SEVERE = 0x00000002,


        SERVICE_NO_CHANGE = 0xffffffff
    }

    
    public enum SC_RUNTIME_STATUS : uint
    {
        /// <summary>
        /// The service is about to continue.
        /// </summary>
        SERVICE_CONTINUE_PENDING = 0x00000005,

        /// <summary>
        ///  The service is pausing.
        /// </summary>
        SERVICE_PAUSE_PENDING = 0x00000006,

        /// <summary>
        ///  The service is paused.
        /// </summary>
        SERVICE_PAUSED = 0x00000007,

        /// <summary>
        /// The service is running.
        /// </summary>
        SERVICE_RUNNING = 0x00000004,

        /// <summary>
        /// The service is starting.
        /// </summary>
        SERVICE_START_PENDING = 0x00000002,

        /// <summary>
        /// The service is stopping.
        /// </summary>
        SERVICE_STOP_PENDING = 0x00000003,

        /// <summary>
        /// The service has stopped
        /// </summary>
        SERVICE_STOPPED = 0x00000001
    }


    [Flags]
    public enum SC_SERVICE_TYPE : uint
    {
        SERVICE_DRIVER = 0x0000000B,
        SERVICE_WIN32 = 0x00000030,

        /// <summary>
        /// The service is a file system driver.
        /// </summary>
        SERVICE_FILE_SYSTEM_DRIVER = 0x00000002,

        /// <summary>
        /// The service is a device driver.
        /// </summary>
        SERVICE_KERNEL_DRIVER = 0x00000001,

        /// <summary>
        /// The service runs in its own process.
        /// </summary>
        SERVICE_WIN32_OWN_PROCESS = 0x00000010,

        /// <summary>
        /// The service shares a process with other services.
        /// </summary>
        SERVICE_WIN32_SHARE_PROCESS = 0x00000020,

        /// <summary>
        /// The service can interact with the desktop. 
        /// </summary>
        SERVICE_INTERACTIVE_PROCESS = 0x00000100,

        SERVICE_NO_CHANGE = 0xffffffff
    }

    [Flags]
    public enum SC_QUERY_SERVICE_STATUS : uint
    {
        SERVICE_ACTIVE = 0x00000001,
        SERVICE_INACTIVE = 0x00000002,
        SERVICE_STATE_ALL = 0x00000003,
    }

    [Flags]
    public enum SC_SERVICE_CONFIG_INFO_LEVEL : uint
    {
        /// <summary>
        /// The lpBuffer parameter is a pointer to a SERVICE_DESCRIPTION structure.
        /// </summary>
        SERVICE_CONFIG_DESCRIPTION = 1,
    }

    public class NativeServiceFunctions
    {
        public const string SERVICES_ACTIVE_DATABASE = "ServicesActive";

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(
            string machineName, 
            string databaseName, 
            uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(
            IntPtr hSCManager, 
            string lpServiceName, 
            uint dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(
            IntPtr hSCObject);

        [DllImport("advapi32.dll", EntryPoint = "EnumServicesStatusExW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool EnumServicesStatusEx(
            IntPtr hSCManager,
            SC_ENUM_TYPE InfoLevel,
            SC_SERVICE_TYPE ServiceType,
            SC_QUERY_SERVICE_STATUS ServiceState,
            IntPtr lpServices,
            int cbBufSize,
            ref int pcbBytesNeeded,
            ref int lpServicesReturned,
            ref int lpResumeHandle,
            string pszGroupName);
        
        [DllImport("advapi32.dll", EntryPoint = "QueryServiceConfigW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool QueryServiceConfig(
            IntPtr hService,
            IntPtr lpServiceConfig,
            int cbBufSize,
            ref int pcbBytesNeeded );


        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern unsafe bool QueryServiceStatusEx(
            IntPtr serviceHandle,
            SC_STATUS_TYPE statusType,
            IntPtr buffer,
            int bufferSize,
            out int bytesNeeded);
         

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern unsafe bool QueryServiceConfig2(
            IntPtr serviceHandle,
            SC_SERVICE_CONFIG_INFO_LEVEL infoLevel,
            IntPtr buffer,
            int bufferSize,
            out int bytesNeeded);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern unsafe bool ChangeServiceConfig(
            IntPtr serviceHandle,
            SC_SERVICE_TYPE ServiceType = SC_SERVICE_TYPE.SERVICE_NO_CHANGE,
            SC_START_TYPE StartType = SC_START_TYPE.SERVICE_NO_CHANGE,
            SC_ERROR_CONTROL ErrorControl = SC_ERROR_CONTROL.SERVICE_NO_CHANGE,
            string BinaryPathName = null,
            string LoadOrderGroup = null,
            string TagId = null,
            string Dependencies = null,
            string ServiceStartName = null,
            string Password = null,
            string DisplayName = null);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern unsafe bool ChangeServiceConfig2(
            IntPtr serviceHandle,
            SC_SERVICE_CONFIG infoLevel,
            IntPtr memory);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern unsafe bool StartService(
            IntPtr serviceHandle,
            int numArgs,
            IntPtr buffer);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern unsafe bool ControlService(
            IntPtr serviceHandle,
            SC_CONTROL_CODE control, 
            IntPtr buffer);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern unsafe bool DeleteService(
            IntPtr serviceHandle);

        public const int ERROR_MORE_DATA = 234;

    }
}
