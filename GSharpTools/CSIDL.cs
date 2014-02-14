using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSharpTools
{
    public enum CSIDL : int
    {
        /// <summary>
        /// Desktop
        /// </summary>
        DESKTOP = 0x0000,

        /// <summary>
        /// Internet Explorer (icon on desktop)
        /// </summary>
        INTERNET = 0x0001,

        /// <summary>
        /// Start Menu\Programs
        /// </summary>
        PROGRAMS = 0x0002,

        /// <summary>
        /// My Computer\Control Panel
        /// </summary>
        CONTROLS = 0x0003,

        /// <summary>
        /// My Computer\Printers
        /// </summary>
        PRINTERS = 0x0004,

        /// <summary>
        /// My Documents
        /// </summary>
        PERSONAL = 0x0005,

        /// <summary>
        /// USERNAME\Favorites
        /// </summary>
        FAVORITES = 0x0006,

        /// <summary>
        /// Start Menu\Programs\Startup
        /// </summary>
        STARTUP = 0x0007,

        /// <summary>
        /// USERNAME\Recent
        /// </summary>
        RECENT = 0x0008,

        /// <summary>
        /// USERNAME\SendTo
        /// </summary>
        SENDTO = 0x0009,

        /// <summary>
        /// DESKTOP\Recycle Bin
        /// </summary>
        BITBUCKET = 0x000a,

        /// <summary>
        /// USERNAME\Start Menu
        /// </summary>
        STARTMENU = 0x000b,

        /// <summary>
        /// "My Music" folder
        /// </summary>
        MYMUSIC = 0x000d,

        /// <summary>
        /// "My Videos" folder
        /// </summary>
        MYVIDEO = 0x000e,

        /// <summary>
        /// USERNAME\Desktop
        /// </summary>
        DESKTOPDIRECTORY = 0x0010,

        /// <summary>
        /// My Computer
        /// </summary>
        DRIVES = 0x0011,

        /// <summary>
        /// Network Neighborhood (My Network Places)
        /// </summary>
        NETWORK = 0x0012,

        /// <summary>
        /// USERNAME\nethood
        /// </summary>
        NETHOOD = 0x0013,

        /// <summary>
        /// windows\fonts
        /// </summary>
        FONTS = 0x0014,
        TEMPLATES = 0x0015,

        /// <summary>
        /// All Users\Start Menu
        /// </summary>
        COMMON_STARTMENU = 0x0016,

        /// <summary>
        /// All Users\Start Menu\Programs
        /// </summary>
        COMMON_PROGRAMS = 0X0017,

        /// <summary>
        /// All Users\Startup
        /// </summary>
        COMMON_STARTUP = 0x0018,

        /// <summary>
        /// All Users\Desktop
        /// </summary>
        COMMON_DESKTOPDIRECTORY = 0x0019,

        /// <summary>
        /// USERNAME\Application Data
        /// </summary>
        APPDATA = 0x001a,

        /// <summary>
        /// USERNAME\PrintHood
        /// </summary>
        PRINTHOOD = 0x001b,

        /// <summary>
        /// USERNAME\Local Settings\Applicaiton Data (non roaming)
        /// </summary>
        LOCAL_APPDATA = 0x001c,

        /// <summary>
        /// non localized startup
        /// </summary>
        ALTSTARTUP = 0x001d,

        /// <summary>
        /// non localized common startup
        /// </summary>
        COMMON_ALTSTARTUP = 0x001e,
        COMMON_FAVORITES = 0x001f,
        INTERNET_CACHE = 0x0020,
        COOKIES = 0x0021,
        HISTORY = 0x0022,

        /// <summary>
        /// All Users\Application Data
        /// </summary>
        COMMON_APPDATA = 0x0023,

        /// <summary>
        /// GetWindowsDirectory()
        /// </summary>
        WINDOWS = 0x0024,

        /// <summary>
        /// GetSystemDirectory()
        /// </summary>
        SYSTEM = 0x0025,

        /// <summary>
        /// C:\Program Files
        /// </summary>
        PROGRAM_FILES = 0x0026,

        /// <summary>
        /// C:\Program Files\My Pictures
        /// </summary>
        MYPICTURES = 0x0027,

        /// <summary>
        /// USERPROFILE
        /// </summary>
        PROFILE = 0x0028,

        /// <summary>
        /// x86 system directory on RISC
        /// </summary>
        SYSTEMX86 = 0x0029,

        /// <summary>
        /// x86 C:\Program Files on RISC
        /// </summary>
        PROGRAM_FILESX86 = 0x002a,

        /// <summary>
        /// C:\Program Files\Common
        /// </summary>
        PROGRAM_FILES_COMMON = 0x002b,

        /// <summary>
        /// x86 Program Files\Common on RISC
        /// </summary>
        PROGRAM_FILES_COMMONX86 = 0x002c,

        /// <summary>
        /// All Users\Templates
        /// </summary>
        COMMON_TEMPLATES = 0x002d,

        /// <summary>
        /// All Users\Documents
        /// </summary>
        COMMON_DOCUMENTS = 0x002e,

        /// <summary>
        /// All Users\Start Menu\Programs\Administrative Tools
        /// </summary>
        COMMON_ADMINTOOLS = 0x002f,

        /// <summary>
        /// USERNAME\Start Menu\Programs\Administrative Tools
        /// </summary>
        ADMINTOOLS = 0x0030,

        /// <summary>
        /// Network and Dial-up Connections
        /// </summary>
        CONNECTIONS = 0x0031,

        /// <summary>
        /// All Users\My Music
        /// </summary>
        COMMON_MUSIC = 0x0035,

        /// <summary>
        /// All Users\My Pictures
        /// </summary>
        COMMON_PICTURES = 0x0036,

        /// <summary>
        /// All Users\My Video
        /// </summary>
        COMMON_VIDEO = 0x0037,

        /// <summary>
        /// Resource Direcotry
        /// </summary>
        RESOURCES = 0x0038,

        /// <summary>
        /// Localized Resource Direcotry
        /// </summary>
        RESOURCES_LOCALIZED = 0x0039,

        /// <summary>
        /// Links to All Users OEM specific apps
        /// </summary>
        COMMON_OEM_LINKS = 0x003a,

        /// <summary>
        /// USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
        /// </summary>
        CDBURN_AREA = 0x003b,

        /// <summary>
        /// Computers Near Me (computered from Workgroup membership)
        /// </summary>
        COMPUTERSNEARME = 0x003d
    }
}