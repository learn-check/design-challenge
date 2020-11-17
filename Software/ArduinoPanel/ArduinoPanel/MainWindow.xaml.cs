using System;
using System.Linq;
using System.Windows;
using System.IO.Ports;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using System.Threading.Tasks;
using ArduinoPanel.data;
using System.Text;

namespace ArduinoPanel
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Default settings for the arduino, 9600 as de default baud rate
        /// </summary>
        private readonly SerialPort Arduino = new SerialPort { BaudRate = 9600 };

        /// <summary>
        /// Helper class for communicating with our api
        /// </summary>
        private readonly ApiHandler Api = new ApiHandler();

        /// <summary>
        /// Timer used to update our datagrid with the latest info from the api
        /// </summary>
        private readonly Timer UpdateTimer = new Timer();

        /// <summary>
        /// List of all the current reservations from the api
        /// </summary>
        private List<CustomerInfo> customerInfos = new List<CustomerInfo>();
        
        /// <summary>
        /// The current customer who is traveling on the monorail
        /// </summary>
        private CustomerInfo CurrentCustomer { get; set; }

        /// <summary>
        /// Indicates if there is a next travel
        /// </summary>
        private bool CanDoNext { get; set; }

        /// <summary>
        /// Current customer index
        /// </summary>
        private int CurrentIndex { get; set; }
        

        /// <summary>
        /// Stores all the messeages/errors
        /// </summary>
        private readonly List<string> MessagesList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            CurrentIndex = 0;

            Closing += (e, s) => OnShutDown();

            UpdateTimer.Interval = 2500; // 2.5 seconds

            UpdateTimer.AutoReset = true;

            UpdateTimer.Enabled = true;

            UpdateTimer.Elapsed += (e, s) => FetchAPI();
#if DEBUG
            PortInput.Text = "COM4"; // being lazy is fun
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

            Clear.Click += (e, s) => {

                MessagesList.Clear();
                Messages.ItemsSource = null;
                Messages.ItemsSource = MessagesList;
            };
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
                Dispatcher.Invoke(new Action( async () => {

                    // monorail has reached the given station
                    if (data.Equals("Train has arrived\r\n"))
                    {
                        if (CurrentIndex >= customerInfos.Count - 1)
                        {
                            DisplayMessage("[INFO] Lijst is leeg, trein gaat stoppen");
                            StopTrain();
                        }
                        else if (CanDoNext) 
                        {
                            // send the next location for the monorail to travel to
                            CurrentCustomer = customerInfos[CurrentIndex++];
                            await Task.Delay(3000);
                            WriteToArduino($"{CurrentCustomer.StartLocation},{CurrentCustomer.EndLocation}");
                        }
                        
                    }
                    else if (int.TryParse(data, out var nval)) 
                    {
                        if (nval > 0 && nval <= 3)
                        {
                            // Updates api travel location
                            var res = await Api.UpdateTravelLocation(CurrentCustomer, nval);
                            DisplayMessage($"[INFO] {res}");
                        }
                        else
                        {
                            DisplayMessage($"[ARDUINO] Gvd ruben je had een taak {data}");
                        }
                    }

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

            Messages.ScrollIntoView(Messages.Items[Messages.Items.Count - 1]);
        }

        /// <summary>
        /// Tells the arduino to start moving the monorail to a given station
        /// </summary>
        private void StartTrain()
        {
            
            if (Arduino.IsOpen && customerInfos.Count > 0)
            {
                CurrentCustomer = customerInfos[CurrentIndex++];

                WriteToArduino($"{CurrentCustomer.StartLocation},{CurrentCustomer.EndLocation}");

                ArduinoStartTrain.IsEnabled = false; // disable until we need to start again

                CanDoNext = true;
            }
            else
            {
                if (!Arduino.IsOpen)
                {
                    DisplayMessage("[ERROR] Arduino COM poort is niet open");
                }
                else
                {
                    DisplayMessage("[INFO] Niet genoeg mensen");
                }
            }
        } 

        /// <summary>
        /// Tells the arduino to stop moving the monorail
        /// </summary>
        private void StopTrain()
        {
            ArduinoStartTrain.IsEnabled = true;
            CanDoNext = false;
            WriteToArduino("-1");
        }

        /// <summary>
        /// Writes the given data to the ardruino over the serialport
        /// </summary>
        /// <param name="data"> Data to send to the arduino</param>
        private void WriteToArduino(string data)
        {
            if (Arduino.IsOpen)
            {
                Arduino.WriteLine(data);
                Arduino.BaseStream.Flush();
            }
            else
            {
                DisplayMessage("[ERROR] Arduino COM poort is niet open");
            }
        }

        /// <summary>
        /// Gets a list of the current reservations that are stored in our REST API, gets called by a timer every 2.5 seconds
        /// </summary>
        private void FetchAPI()
        {
            // The timer runs on a different thread so we need to invoke
            // in to the main thread before we can update the datagrid with the reservations
            Dispatcher.Invoke(new Action( async () => {

                StatusLabel.Content = "Bijwerken.....";

                customerInfos = await Api.GetAllReservations();

                await Task.Delay(1000);

                Reservations.ItemsSource = null;

                Reservations.ItemsSource = customerInfos;

                StatusLabel.Content = "Reserveringen";
            }));
        }

        /// <summary>
        /// Tries to close the connection on given com port, if it fails a message with the error will be displayed
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
        /// Tries to make a connection over the given com port, if it fails a message with the error will be displayed
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
