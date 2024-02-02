using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace WeatherApp
{
    public partial class MainWindow : Window
    {
        public string openWeatherApiKey;
        public string globalTimeApiKey;

        public class ApiKeys
        {
            public string openWeatherApiKey { get; set; }
            public string globalTimeApiKey { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            WindowLogging();
        }
        private void WindowLogging()
        {
            string loggingvalue = File.ReadAllText("loggingvalue.json");
            JObject keyValuePairs = JObject.Parse(loggingvalue);
            string value = (string)keyValuePairs["value"];

            if (value == "true")
            {
                MainApp mainApp = new MainApp();
                this.Visibility = Visibility.Hidden;
                mainApp.Show();
            }
        }
        private void btnMainApp_Click(object sender, RoutedEventArgs e)
        {
            MainApp mainApp = new MainApp();
            this.Visibility = Visibility.Hidden;
            mainApp.Show();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void tbWeatherApi_Enter(object sender, RoutedEventArgs e)
        {
            if (tbWeatherApi.Text == "  Please enter Open Weather Api Key...")
            {
                tbWeatherApi.Text = "";
            }
        }
        private void tbWeatherApi_Leave(object sender, RoutedEventArgs e)
        {
            if (tbWeatherApi.Text == "")
            {
                tbWeatherApi.Text = "  Please enter Open Weather Api Key...";
            }
        }
        private void tbTimeApi_Enter(object sender, RoutedEventArgs e)
        {
            if (tbTimeApi.Text == "  Please enter Global Time Api Key...")
            {
                tbTimeApi.Text = "";
            }
        }
        private void tbTimeApi_Leave(object sender, RoutedEventArgs e)
        {
            if (tbTimeApi.Text == "")
            {
                tbTimeApi.Text = "  Please enter Global Time Api Key...";
            }
        }
        private void tbWeatherApi_Search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                openWeatherApiKey = tbWeatherApi.Text;
            }
        }
        private void tbTimeApi_Search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApiKeys apiKeys = new ApiKeys()
                {
                    openWeatherApiKey = tbWeatherApi.Text,
                    globalTimeApiKey = tbTimeApi.Text
                };

                openWeatherApiKey = tbWeatherApi.Text;
                globalTimeApiKey = tbTimeApi.Text;

                if (!(openWeatherApiKey == "" || openWeatherApiKey == "  Please enter Open Weather Api Key...") && !(globalTimeApiKey == "" || globalTimeApiKey == "  Please enter Global Time Api Key..."))
                {
                    btnMainApp.Visibility = Visibility.Visible;
                    string json = JsonConvert.SerializeObject(apiKeys, Formatting.Indented);
                    File.WriteAllText("keys.json", json);
                }
            }
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}