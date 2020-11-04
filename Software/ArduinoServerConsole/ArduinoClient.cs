using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoServerConsole
{
    class ArduinoClient
    {
        private readonly TcpClient Client;
        private readonly int NetworkBufferSize = 256;
        public TcpState Connectionstate { get; private set; } = TcpState.Null;

        private bool StopClient = false;

        public ArduinoClient(TcpClient client)
        {
            Client = client;

            if (client.Connected)
            {
                Connectionstate = TcpState.Connected;
                Client.SendTimeout = 250; // 1/4 of a second
            }
        }

        public async void HandleConnection(Object stateinfo)
        {
            NetworkStream networkStream = Client.GetStream();
            byte[] buffer = new byte[NetworkBufferSize];
            int bytesRead = 0;

            byte[] lol = Encoding.UTF8.GetBytes("Ik haat business echt....");

            while (!StopClient)
            {
                try
                {
                    networkStream.Write(lol, 0, lol.Length);
                    await Task.Delay(1000);
                    //bytesRead = networkStream.Read(buffer, 0, buffer.Length);

                    //if (bytesRead > 0)
                    //{
                    //    // recieved data from the client
                        
                    //}

                    //// else just keep repeating
                }
                catch (Exception ex)
                {
                    // if the ReceiveTimeout is reached an IOException will be raised...
                    // with an InnerException of type SocketException and ErrorCode 10060
                    // timeout exception
                    if (ex.InnerException is SocketException socketExept && socketExept.ErrorCode == 10060)
                    {
                        bytesRead = 0;
                    }
                    else // fatal error stop client
                    {
                        StopClient = true;
                    }
                }
            }
        }
    }
}
