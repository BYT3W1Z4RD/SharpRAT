using System;
using System.Diagnostics;
using System.Linq;

public class ReconnectPlugin : MarshalByRefObject
{
    public static void Execute()
    {
        // Restart the current process
        Process.Start(new ProcessStartInfo
        {
            FileName = Environment.GetCommandLineArgs()[0],
            Arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1)),
            UseShellExecute = false,
            CreateNoWindow = true
        });

        // Exit the current process
        Environment.Exit(0);
    }
}