using System;
using System.Diagnostics;

namespace Plugin
{
    public class Plugin : MarshalByRefObject
    {
        public static void Execute()
        {
            Process currentProcess = Process.GetCurrentProcess();
            currentProcess.Kill();
        }
    }
}
