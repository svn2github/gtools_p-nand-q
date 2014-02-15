using System;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;
using GSharpTools;
using System.Collections.Generic;
using System.Text;

namespace su
{
    class su
    {
        /// <summary>
        /// Helper class for parsing input arguments
        /// </summary>
        private InputArgs Args;

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// This program finds an executable on the PATH. It can also find other stuff on the path, but 
        /// mostly it finds the executable.s
        /// </summary>
        /// <param name="args"></param>
        private void Run(string[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding(Encoding.Default.CodePage);

            Args = new InputArgs("su", string.Format(resource.IDS_TITLE, AppVersion.Get()) + "\r\n" + resource.IDS_COPYRIGHT);


            Args.Add(InputArgType.Parameter, "cmd", "cmd.exe", Presence.Optional, "bvlabl");

            if (Args.Process(args))
            {
                if (!IsAdministrator())
                {
                    string command = Args.GetString("cmd");
                    if( command.Equals("cmd.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "cmd.exe"));
                        startInfo.Verb = "runas";
                        startInfo.WorkingDirectory = Environment.CurrentDirectory;
                        startInfo.Arguments = string.Format("/K \"cd /d {0}\"", startInfo.WorkingDirectory);

                        System.Diagnostics.Process.Start(startInfo);
                    }
                    else
                    {
                        string exe;
                        if( ProcessInfoTools.FindExecutable(command, out exe) )
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo(exe);
                            startInfo.Verb = "runas";
                            startInfo.WorkingDirectory = Environment.CurrentDirectory;

                            System.Diagnostics.Process.Start(startInfo);
                        }
                        else
                        {
                            Console.WriteLine("ERROR, unable to locate '{0}'", command);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("You are already administrator...");
                }
            }
        }

        static void Main(string[] args)
        {
            new su().Run(args);
        }
    }
}
