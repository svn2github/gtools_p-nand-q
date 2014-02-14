using System;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;

namespace su
{
    class su
    {
        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void Main(string[] args)
        {
            if( !IsAdministrator() )
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "cmd.exe"));
                startInfo.Verb = "runas";

                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.Arguments = string.Format("/K \"cd /d {0}\"", startInfo.WorkingDirectory);

                System.Diagnostics.Process.Start(startInfo);
            }
        }
    }
}
