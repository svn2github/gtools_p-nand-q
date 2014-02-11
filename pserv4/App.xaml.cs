using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

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
                    else if (key.Equals("PROGRAMS"))
                        initialView = MainViewType.Programs;
                    else if (key.Equals("WINDOWS"))
                        initialView = MainViewType.Windows;
                    else if (key.Equals("DUMPXML"))
                        dumpXml = true;
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
                if( string.IsNullOrEmpty(dumpXmlFilename))
                {

                }
                else
                {

                }
            }
            else
            {
                wnd.Show();
            }
        }

    }
}
