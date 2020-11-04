using System.Windows;

namespace ArduinoPanel
{
    public partial class App : Application
    {
        public static void Error(string message)
        {
            MessageBox.Show(message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
