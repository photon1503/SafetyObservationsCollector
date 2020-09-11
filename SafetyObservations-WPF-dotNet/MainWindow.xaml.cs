using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;
using ASCOM.Utilities;
using ASCOM.DriverAccess;
using System.Timers;
using System.Collections.ObjectModel;

namespace SafetyObservations_WPF_dotNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservingConditions oObservingConditions;
        private string observingConditionsID;
        private SafetyMonitor oSafetyMonitor;
        private string safetyMonitorID;
        private Profile oProfile;
        private string profileID = "WeatherSafetyCollector";

        ObservableCollection<TodoItem> items = new ObservableCollection<TodoItem>();        
        TodoItem oCloudCover = new TodoItem() {Title = "CloudCover", unit = "%", isChecked = false, isSafe = false, age = 0, min = 0, max = 0 };
        TodoItem oDewPoint = new TodoItem() { Title = "DewPoint", unit = "°C", isChecked = false, isSafe = false, age = 0, min = 0, max = 0 };
        TodoItem oHumidity = new TodoItem() { Title = "Humidity", unit = "%", isChecked = false, isSafe = false, age = 0, min = 0, max = 0 };
        TodoItem oSkyTemperature = new TodoItem() { Title = "Sky Temperature", unit = "°C", isChecked = false, isSafe = false, age = 0, min = 0, max = 0 };
        TodoItem oPressure = new TodoItem() { Title = "Pressure", unit = "hPa", isChecked = false, isSafe = false, age = 0, min = 0, max = 0 };
        TodoItem oRainRate = new TodoItem() { Title = "Rain Rate", unit = "mm/h", isChecked = false, isSafe = false, age = 0, min = 0, max = 0 };



        public MainWindow()
        {
            InitializeComponent();


            items.Add(oCloudCover);
            items.Add(oDewPoint);
            items.Add(oHumidity);
            items.Add(oSkyTemperature);
            items.Add(oPressure);
            items.Add(oRainRate);

            lbTodoList.ItemsSource = items;

            //ASCOM init
            oProfile = new Profile();

            if (!oProfile.IsRegistered(profileID))
            {
                oProfile.Register(profileID, "Weather Safety Collector Profile");
            }

            observingConditionsID = oProfile.GetValue(profileID, "observingsConditionID");            
            safetyMonitorID = oProfile.GetValue(profileID, "safetyMonitorID");            
        }




        private void Connect()
        {
            if (!String.IsNullOrEmpty(observingConditionsID))
            {
                try
                {
                    oObservingConditions = new ObservingConditions(observingConditionsID) { Connected = true };
                    
                }
                catch (Exception ex)
                {
                    String msg = ex.Message;
                    if (ex.InnerException != null)
                        msg += " - " + ex.InnerException.Message;
                    MessageBox.Show(string.Format("Connect failed with error {0}", msg));
                }
            }

            if (!String.IsNullOrEmpty(safetyMonitorID))
            {
                try
                {
                    oSafetyMonitor = new SafetyMonitor(safetyMonitorID) { Connected = true };
                    
                }
                catch (Exception ex)
                {
                    String msg = ex.Message;
                    if (ex.InnerException != null)
                        msg += " - " + ex.InnerException.Message;
                    MessageBox.Show(string.Format("Connect failed with error {0}", msg));
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (oObservingConditions != null && oObservingConditions.Connected) return;
            try
            {
                observingConditionsID = ObservingConditions.Choose(observingConditionsID);
                oProfile.WriteValue(profileID, "observingsConditionID", observingConditionsID);
            }
            catch (Exception ex)
            {
                String msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " - " + ex.InnerException.Message;
                MessageBox.Show(string.Format("Choose failed with error {0}", msg));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (oSafetyMonitor != null && oSafetyMonitor.Connected) return;
            try
            {
                safetyMonitorID = SafetyMonitor.Choose(safetyMonitorID);
                oProfile.WriteValue(profileID, "safetyMonitorID", safetyMonitorID);
            }
            catch (Exception ex)
            {
                String msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " - " + ex.InnerException.Message;
                MessageBox.Show(string.Format("Choose failed with error {0}", msg));
            } 
            
        }

        private void bntStart_Click(object sender, RoutedEventArgs e)
        {
            Connect();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimedEvent;
            timer.Start();
        }

        private  void OnTimedEvent(object sender, EventArgs e)
        {
            bool isSafeSafteyMonitor = oSafetyMonitor.IsSafe;
            if (isSafeSafteyMonitor)
                rectSafteyMonitor.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
        else
                rectSafteyMonitor.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);


            oCloudCover.value = oObservingConditions.CloudCover;
            oDewPoint.value = oObservingConditions.DewPoint;
            oHumidity.value = oObservingConditions.Humidity;

            //ICollectionView view = CollectionViewSource.GetDefaultView(items);
            lbTodoList.Items.Refresh();




        }

        public class TodoItem
        {
            public string Title { get; set; }
            public double value { get; set; }
            public string unit { get; set; }
            public bool isChecked { get; set; }
            public bool isSafe { get; set; }
            public double age { get; set; }
            public double min { get; set; }
            public double max { get; set; }
        }
    }
}
