using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace pserv4
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();

            MainViewType initialView = MainViewType.Services;
            services.ServiceStateRequest ssr = null;
            bool dumpXml = false;
            bool useClipboard = false;
            string dumpXmlFilename = null;
            List<string> servicenames = new List<string>();

            foreach(string arg in args)
            {
                if(arg.StartsWith("/"))
                {
                    string key = arg.Substring(1).ToUpper();
                    if (key.Equals("DEVICES"))
                        initialView = MainViewType.Devices;
                    else if (key.Equals("MODULES"))
                        initialView = MainViewType.Modules;
                    else if (key.Equals("PROCESSES"))
                        initialView = MainViewType.Processes;
                    else if (key.Equals("UNINSTALLER"))
                        initialView = MainViewType.Uninstaller;
                    else if (key.Equals("WINDOWS"))
                        initialView = MainViewType.Windows;
                    else if (key.Equals("DUMPXML"))
                        dumpXml = true;
                    else if (key.Equals("CLIPBOARD"))
                    {
                        dumpXml = true;
                        useClipboard = true;
                    }
                    else if (key.Equals("START"))
                        ssr = new services.RequestServiceStart();
                    else if (key.Equals("STOP"))
                        ssr = new services.RequestServiceStop();
                    else if (key.Equals("RESTART"))
                        ssr = new services.RequestServiceRestart();
                    else if (key.Equals("PAUSE"))
                        ssr = new services.RequestServicePause();
                    else if (key.Equals("CONTINUE"))
                        ssr = new services.RequestServiceContinue();
                }
                else if( dumpXml && string.IsNullOrEmpty(dumpXmlFilename))
                {
                    dumpXmlFilename = arg;
                }
                else if( ssr != null)
                {
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
