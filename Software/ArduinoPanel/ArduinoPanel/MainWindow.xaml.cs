using System;
using System.Linq;
using System.Windows;
using System.IO.Ports;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net;
using System.Text;

namespace ArduinoPanel
{
    enum TcpState 
    {
        Null, // base
        Connected,
        Disconnected
    }
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Default settings for the arduino
        /// </summary>
        private readonly SerialPort Arduino = new SerialPort() {
            BaudRate = 9600
        };

        private TcpState TcpState = TcpState.Null;
        /// <summary>
        /// Stores all the messeages/errors
        /// </summary>
        private readonly List<string> MessagesList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            Closing += (e, s) => OnShutDown();

#if DEBUG
            PortInput.Text = "COM3"; // why not
#endif
            ArduinoConnect.Click += (e, s) => TryConnect();
            
            ArduinoDisconnect.Click += (e, s) => TryDisconnectArduino();

            ServerConnect.Click += (e, s) => HandleServerConnection();

            Arduino.DataReceived += DataReceived;
        }

        /// <summary>
        /// Gets called when data is received from the arduino
        /// </summary>
        /// <param name="port">Meh</param>
        /// <param name="_event">Meh 2</param>
        private void DataReceived(object port, SerialDataReceivedEventArgs _event)
        {
            var serial = (SerialPort) port;

            var data = serial.ReadExisting();

            if (!string.IsNullOrEmpty(data) && data.Last() == '\n' && data != "-1")
            {
                Dispatcher.Invoke(new Action(() => {
                    DisplayMessage($"[ARDUINO]: {data}");
                    Messages.ScrollIntoView(Messages.Items.Count - 1);
                }));
            }
        }

        /// <summary>
        /// Updates the listbox
        /// </summary>
        /// <param name="addMsg">message to be displayed</param>
        private void DisplayMessage(string addMsg)
        {
            MessagesList.Add(addMsg);
            Messages.ItemsSource = null;
            Messages.ItemsSource = MessagesList;
        }

        /// <summary>
        /// Tries to close the connection on given com port
        /// </summary>
        private void TryDisconnectArduino()
        {
            if (!Arduino.IsOpen) return;

            try
            {
                Arduino.Close();
                DisplayMessage("[ARDUINO] Verbinding is gesloten");
            }
            catch (Exception e)
            {
                DisplayMessage($"[ERROR] {e.Message}");
            }
        }

        /// <summary>
        /// Tries to make a connection over the given com port
        /// </summary>
        private void TryConnect()
        {
            if (string.IsNullOrEmpty(PortInput.Text))
            {
                App.Error("Portnaam mag niet leeg zijn!");
                return;
            }

            try
            {
                if (Arduino.IsOpen)
                    Arduino.Close();

                Arduino.PortName = PortInput.Text.ToUpper().Trim();

                Arduino.Open();
                DisplayMessage("[ARDUINO] Verbinding is gemaakt");
            }
            catch (Exception e)
            {
                DisplayMessage($"[ERROR] {e.Message}");
                Arduino.Close();
            }
        }

        private async void TryConnectToServer()
        {
            await Task.Run(new Action( async () => {
                
                Dispatcher.Invoke(new Action(() => {
                    // change tcp state
                    // change button text
                    ServerConnect.Content = "Verbreek verbinding met server";
                    TcpState = TcpState.Connected;
                }));

                var Client = new TcpClient
                {
                    SendTimeout = 250
                };

                int NetworkBufferSize = 256;
                bool StopClient = false;
                byte[] buffer = new byte[NetworkBufferSize];
                int bytesRead = 0;

                int port = 43594;
#if DEBUG
                string host = "localhost"; // Dev
#else
                string host = "207.180.202.119"; // Live
#endif
                // Do some magic here
                try
                {
                    await Client.ConnectAsync(host, port);
                    NetworkStream networkStream = Client.GetStream();

                    while (!StopClient)
                    {
                        bytesRead = networkStream.Read(buffer, 0, buffer.Length);

                        if (bytesRead > 0)
                        {
                            Dispatcher.Invoke(new Action(() => {
                                DisplayMessage($"[SERVER] {Encoding.UTF8.GetString(buffer)}");
                            }));
                        }
                        // else just keep repeating till the end of times
                    }

                }
                catch (Exception ex)
                {
                    // welp
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

                Dispatcher.Invoke(new Action(() => {
                    ServerConnect.Content = "Maak verbinding met server";
                    TcpState = TcpState.Disconnected;
                }));
            }));
        }

        private void TryDisconnectFromServer()
        {
            // change tcp state
            // change button text
        }

        private void HandleServerConnection()
        {
            switch (TcpState)
            {
                case TcpState.Connected:
                    TryDisconnectFromServer();
                    break; // disconnect

                case TcpState.Disconnected:
                case TcpState.Null:
                    TryConnectToServer();
                    break; // create a new connection
                
                default: break;
            }
        }

        /// <summary>
        /// Releases the serial port on application exit
        /// </summary>
        private void OnShutDown()
        {
            try
            {
                if (Arduino.IsOpen)
                {
                    Arduino.Close();
                    Arduino.Dispose();
                }
            }
            catch
            {
                // meh don't about exception care on shutdown
            }
        }
    }
}
