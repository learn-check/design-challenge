using System;
using System.Linq;
using System.Windows;
using System.IO.Ports;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using System.Threading.Tasks;
using ArduinoPanel.data;

namespace ArduinoPanel
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Default settings for the arduino
        /// </summary>
        private readonly SerialPort Arduino = new SerialPort() {
            BaudRate = 9600
        };

        private readonly ApiHandler Api = new ApiHandler();
        private readonly Timer UpdateTimer = new Timer();
        private readonly List<CustomerInfo> customerInfos = new List<CustomerInfo>();

#if DEBUG
        private readonly string BASE_URL = @"https://localhost:44352/api/";
#else
        private readonly string BASE_URL = "";
#endif

        /// <summary>
        /// Stores all the messeages/errors
        /// </summary>
        private readonly List<string> MessagesList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            Closing += (e, s) => OnShutDown();

            UpdateTimer.Interval = 2500; // 2.5 seconds

            UpdateTimer.AutoReset = true;

            UpdateTimer.Enabled = true;

            UpdateTimer.Elapsed += (e, s) => FetchAPI();
#if DEBUG
            PortInput.Text = "COM3"; // being lazy is fun
#endif
            ArduinoConnect.Click += (e, s) => TryConnectArduino();
            
            ArduinoDisconnect.Click += (e, s) => TryDisconnectArduino();

            ArduinoStartTrain.Click += (e, s) => StartTrain();

            ArduinoStopTrain.Click += (e, s) => StopTrain();

            Arduino.DataReceived += DataReceived;

            Reservations.AutoGeneratingColumn += (e, s) =>
            {
                s.Column.Width = 75;
            };

            Reservations.ItemsSource = customerInfos;
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

        private void StartTrain()
        { } // TODO

        private void StopTrain()
        { } // TODO 

        private void FetchAPI()
        {
            Dispatcher.Invoke(new Action( async () => {

                StatusLabel.Content = "Bijwerken.....";

                var reservations = await Api.GetAllReservations();

                await Task.Delay(1000);

                Reservations.ItemsSource = null;

                Reservations.ItemsSource = reservations;

                StatusLabel.Content = "Reserveringen";
            }));
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
        private void TryConnectArduino()
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
                // meh don't care about exception on shutdown
            }
        }
    }
}
