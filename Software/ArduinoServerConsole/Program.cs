using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ArduinoServerConsole
{
    enum TcpState
    {
        Null,
        Connected,
        Disconnected
    }

    class Program
    {
        private static int PORT = 43594;
#if DEBUG
        private static IPAddress HOST = IPAddress.Parse("127.0.0.1"); // Localhost
#else
        private static IPAddress HOST = IPAddress.Parse("0.0.0.0"); // Live
#endif
        private static bool Running = true;

        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(HOST, PORT);

            server.Start();

            DisplayMessage("Server started");
            
            while (Running)
            {
                var client = new ArduinoClient(server.AcceptTcpClient());

                ThreadPool.QueueUserWorkItem(client.HandleConnection);
            
            }

            DisplayMessage("Server stopped");
            DisplayMessage("Press any key to exit");
            Console.ReadKey();
        }


        public static void DisplayMessage(string msg)
        {
            Console.WriteLine($"[INFO] {msg}");
        }
    }
}
