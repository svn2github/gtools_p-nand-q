using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using log4net;
using System.Reflection;
using GSharpTools;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace pserv4
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal static pserv4.Properties.Settings Settings;
        internal static PersistentSettings PersistentSettings;

        private void Application_Exit(object sender, ExitEventArgs args)
        {
            PersistentSettings.Save();
            Log.Info("- shutting down pserv -");
        }

        private void Application_Startup(object sender, StartupEventArgs sea)
        {
            Log.Info("- starting up pserv -");

            Settings = new pserv4.Properties.Settings();
            PersistentSettings = new PersistentSettings(Settings, "p-nand-q.com\\pserv4");
            PersistentSettings.Load();

            MainViewType initialView = MainViewType.Modules;
            try
            {
                if( !string.IsNullOrEmpty(Settings.LastViewType))
                {
                    initialView = (MainViewType)Enum.Parse(typeof(MainViewType), Settings.LastViewType, true);
                }
            }
            catch(Exception)
            {
            }

            services.ServiceStateRequest ssr = null;
            bool dumpXml = false;
            bool useClipboard = false;
            string dumpXmlFilename = null;
            List<string> servicenames = new List<string>();
            int n = 0;
            foreach(string arg in sea.Args)
            {
                Log.InfoFormat("- arg[{0}]: {1}", n++, arg);
                if(arg.StartsWith("/"))
                {
                    string key = arg.Substring(1).ToUpper();
                    if (key.Equals("DEVICES"))
                    {
                        initialView = MainViewType.Devices;
                        Log.InfoFormat("=> set initialView to {0}", initialView);
                    }
                    else if (key.Equals("MODULES"))
                    {
                        initialView = MainViewType.Modules;
                        Log.InfoFormat("=> set initialView to {0}", initialView);
                    }
                    else if (key.Equals("PROCESSES"))
                    {
                        initialView = MainViewType.Processes;
                        Log.InfoFormat("=> set initialView to {0}", initialView);
                    }
                    else if (key.Equals("UNINSTALLER"))
                    {
                        initialView = MainViewType.Uninstaller;
                        Log.InfoFormat("=> set initialView to {0}", initialView);
                    }
                    else if (key.Equals("WINDOWS"))
                    {
                        initialView = MainViewType.Windows;
                        Log.InfoFormat("=> set initialView to {0}", initialView);
                    }
                    else if (key.Equals("DUMPXML"))
                    {
                        dumpXml = true;
                        Log.Info("=> set dumpXml to true");
                    }
                    else if (key.Equals("CLIPBOARD"))
                    {
                        dumpXml = true;
                        useClipboard = true;
                        Log.Info("=> set dumpXml to true");
                        Log.Info("=> set useClipboard to true");
                    }
                    else if (key.Equals("START"))
                    {
                        ssr = new services.RequestServiceStart();
                        Log.InfoFormat("=> set ssr to {0}", ssr);
                    }
                    else if (key.Equals("STOP"))
                    {
                        ssr = new services.RequestServiceStop();
                        Log.InfoFormat("=> set ssr to {0}", ssr);
                    }
                    else if (key.Equals("RESTART"))
                    {
                        ssr = new services.RequestServiceRestart();
                        Log.InfoFormat("=> set ssr to {0}", ssr);
                    }
                    else if (key.Equals("PAUSE"))
                    {
                        ssr = new services.RequestServicePause();
                        Log.InfoFormat("=> set ssr to {0}", ssr);
                    }
                    else if (key.Equals("CONTINUE"))
                    {
                        ssr = new services.RequestServiceContinue();
                        Log.InfoFormat("=> set ssr to {0}", ssr);
                    }
                }
                else if( dumpXml && string.IsNullOrEmpty(dumpXmlFilename))
                {
                    dumpXmlFilename = arg;
                    Log.InfoFormat("=> set dumpXmlFilename to {0}", dumpXmlFilename);
                }
                else if( ssr != null)
                {
                    Log.InfoFormat("=> add service named '{0}'", arg);
                    servicenames.Add(arg);
                }
            }

            MainWindow wnd = new MainWindow(initialView);
            if (dumpXml)
            {
                wnd.SwitchController(initialView, false);
                if (useClipboard)
                {
                    wnd.CopyToClipboard(null, null);
                }
                else if (string.IsNullOrEmpty(dumpXmlFilename))
                {
                    wnd.SaveAsXML(null, null);
                }
                else
                {
                    wnd.SaveAsXML(dumpXmlFilename, null);
                }
                Shutdown();
            }
            else
            {
                if( ssr != null )
                {
                    wnd.SetInitialAction(ssr, servicenames);
                }
                wnd.Show();
            }
        }

    }
}
