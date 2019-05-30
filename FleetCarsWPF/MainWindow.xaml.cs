using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Newtonsoft.Json;
using api.Models;

namespace FleetCarsWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string Apiroot = "http://10.0.2.2:5000/api";

        public MainWindow()
        {
            InitializeComponent();

            FleetCarsGrid.ItemsSource = GetDataFromApi();
        }

        public List<Cars> GetDataFromApi()
        {
            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString($"{Apiroot}/cars");

                List<Cars> cars = JsonConvert.DeserializeObject<List<Cars>>(json);

                return cars;
            }
        }

        public Cars GetSelectedItem()
        {
            var x = FleetCarsGrid.SelectedIndex;
            var cars = GetDataFromApi();
            return cars[x];
        }

        public string UpdateCars(int id, string name, string manufacturer)
        {
            Cars car = new Cars();
            car.Id = id;
            car.Model = name;
            car.Manufacturer = manufacturer;

            string json = JsonConvert.SerializeObject(car);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create($"{Apiroot}/cars/{id}");
            req.Method = "PUT";
            req.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            };

            var httpResponse = (HttpWebResponse)req.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Model_Number.Text = $"{GetSelectedItem().Id}";
            Model_Name.Text = $"{GetSelectedItem().Model}";
            Manufacturer.Text = $"{GetSelectedItem().Manufacturer}";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(UpdateCars(int.Parse(Model_Number.Text), Model_Name.Text, Manufacturer.Text));

            FleetCarsGrid.ItemsSource = GetDataFromApi();

            Model_Number.Text = $"";
            Model_Name.Text = $"";
            Manufacturer.Text = $"";
        }
    }
}
