using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WeatherApp;

namespace WeatherApp
{
    public partial class MainApp : Window
    {
        public class LoggingValue
        {
            public string value { get; set; }
        }

        public DateTime uhrzeit;
        public string openWeatherApiKey;
        public string globalTimeApiKey;

        public MainApp()
        {
            InitializeComponent();
            Check();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_TickNow;
            timer.Tick += Timer_TickCity;
            timer.Start();
        }
        private void Check()
        {

            LoggingValue loggingValue = new LoggingValue()
            {
                value = "true"
            };
            string json = JsonConvert.SerializeObject(loggingValue, Formatting.Indented);
            File.WriteAllText("loggingvalue.json", json);

            json = File.ReadAllText("keys.json");
            JObject jsonObj = JObject.Parse(json);
            openWeatherApiKey = (string)jsonObj["openWeatherApiKey"];
            globalTimeApiKey = (string)jsonObj["globalTimeApiKey"];
        }
        private void btnSettings_Open(object sender, RoutedEventArgs e)
        {

            LoggingValue loggingValue = new LoggingValue()
            {
                value = "false"
            };
            string json = JsonConvert.SerializeObject(loggingValue, Formatting.Indented);
            File.WriteAllText("loggingvalue.json", json);

            MainWindow mainWindow = new MainWindow();
            this.Visibility = Visibility.Hidden;
            mainWindow.Show();

        }
        private void Timer_TickNow(object sender, EventArgs e)
        {
            tbDate.Text = DateTime.Now.ToString("dd.MM.yyyy");
            tbTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        private void Timer_TickCity(object sender, EventArgs e)
        {
            uhrzeit = uhrzeit.AddSeconds(1);

            tbGlobeTime.Text = uhrzeit.ToString("HH:mm:ss");
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void tbInput_Enter(object sender, RoutedEventArgs e)
        {
            if (tbInput.Text == "Start entering City Name.")
            {
                tbInput.Text = "";
            }
        }
        private void tbInput_Leave(object sender, RoutedEventArgs e)
        {
            if (tbInput.Text == "")
            {
                tbInput.Text = "Start entering City Name.";
            }
        }
        private void tbInput_Search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search();
            }
        }
        private void btnInput_Search(object sender, RoutedEventArgs e)
        {
            Search();
        }
        private async Task Search()
        {
            //Get Weather Api Request
            HttpClient client = new HttpClient();
            string requestBody = tbInput.Text;
            string responseBody = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/forecast?q={requestBody}&appid={openWeatherApiKey}");
            JObject rss = JObject.Parse(responseBody);

            //Get City Name
            string name = (string)rss["city"]["name"];
            tbCityName.Text = name;

            //Set Background adjustment
            string bg = (string)rss["list"][0]["weather"][0]["main"];
            bgImage.Source = new BitmapImage(new Uri($"Pictures/{bg}.jpg", UriKind.RelativeOrAbsolute));

            //Get Temperatur forecast
            double tempOne = (double)rss["list"][0]["main"]["temp"];
            double text = Math.Round(tempOne - 273.15);
            tempMo.Text = Convert.ToString(text + "°");

            double tempTwo = (double)rss["list"][8]["main"]["temp"];
            text = Math.Round(tempTwo - 273.15);
            tempTu.Text = Convert.ToString(text + "°");

            double tempThree = (double)rss["list"][16]["main"]["temp"];
            text = Math.Round(tempThree - 273.15);
            tempWe.Text = Convert.ToString(text + "°");

            double tempFour = (double)rss["list"][24]["main"]["temp"];
            text = Math.Round(tempFour - 273.15);
            tempTh.Text = Convert.ToString(text + "°");

            double tempFive = (double)rss["list"][32]["main"]["temp"];
            text = Math.Round(tempFive - 273.15);
            tempFr.Text = Convert.ToString(text + "°");

            //Wind speed
            string speed = (string)rss["list"][0]["wind"]["speed"];
            tbWindSpeed.Text = speed + " m/s";

            //Icon adjustment
            string iconOne = (string)rss["list"][0]["weather"][0]["icon"];
            imToday.Source = new BitmapImage(new Uri($"Icons/{iconOne}.png", UriKind.RelativeOrAbsolute));

            string iconTwo = (string)rss["list"][8]["weather"][0]["icon"];
            imTomorrow.Source = new BitmapImage(new Uri($"Icons/{iconTwo}.png", UriKind.RelativeOrAbsolute));

            string iconThree = (string)rss["list"][16]["weather"][0]["icon"];
            im3Days.Source = new BitmapImage(new Uri($"Icons/{iconThree}.png", UriKind.RelativeOrAbsolute));

            string iconFour = (string)rss["list"][24]["weather"][0]["icon"];
            im4Days.Source = new BitmapImage(new Uri($"Icons/{iconFour}.png", UriKind.RelativeOrAbsolute));

            string iconFive = (string)rss["list"][32]["weather"][0]["icon"];
            im5Days.Source = new BitmapImage(new Uri($"Icons/{iconFive}.png", UriKind.RelativeOrAbsolute));

            //Get GlobalTime Api Request
            client.DefaultRequestHeaders.Add("X-Api-Key", globalTimeApiKey);
            responseBody = await client.GetStringAsync($"https://api.api-ninjas.com/v1/worldtime?city={requestBody}");
            JObject rsss = JObject.Parse(responseBody);

            //Set Global City Time
            int hour = (int)rsss["hour"];
            int minute = (int)rsss["minute"];
            int second = (int)rsss["second"];
            tbGlobeTime.Text = ($"{hour}:{minute}:{second}");

            uhrzeit = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) + new TimeSpan(hour, minute, second);
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