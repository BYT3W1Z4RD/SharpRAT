using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Plugin
{
    public class Plugin : MarshalByRefObject
    {
        public string Execute()
        {
            // Get the current process ID
            int currentProcessId = Process.GetCurrentProcess().Id;

            // Use taskkill command to force the process to terminate
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.Arguments = $"/c taskkill /f /pid {currentProcessId} && start \"\" \"{Process.GetCurrentProcess().MainModule.FileName}\"";
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            Process killProcess = new Process();
            killProcess.StartInfo = psi;
            killProcess.Start();
            killProcess.WaitForExit();

            // Exit the current process
            Environment.Exit(0);

            return "Restarting remote client...";
        }
    }
}
