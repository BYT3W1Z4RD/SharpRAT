using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace Plugin
{
    public class Plugin : MarshalByRefObject
    {
        public string Execute()
        {
            string response = "Rebooting remote client computer...";
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.Arguments = "/c \"shutdown /r /f /t 0\"";
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            Process myProcess = new Process();
            myProcess.StartInfo = psi;
            myProcess.Start();
            myProcess.WaitForExit();
            return response;
        }
    }
}
