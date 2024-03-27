using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_MachineService.Models;

namespace WPF_MachineService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DetectionObjectContext context;
        Scanning messageBox = new Scanning();
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void LoadDetectionData(string json)
        {
            Window window = new Window
            {
                Content = messageBox,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Title = "Scanning"
            };
            window.Show();
            await Task.Delay(2000);
            List<Product> productsToDisplay = new List<Product>();          
            JObject jsonObject = JObject.Parse(json);
            JArray predictions = (JArray)jsonObject["predictions"];
            foreach (var prediction in predictions)
            {
                string className = (string)prediction["class"];             
                var productsInDB = context.Products.Where(p => p.ProName == className).ToList();             
                if (productsInDB == null)
                {               
                    MessageBox.Show("No product found in the database matching the class: " + className);
                }
               
            }           
            lvListView.ItemsSource = productsToDisplay;
            lvListView.DataContext = this;                      
            window.Close();
        }
    }
}