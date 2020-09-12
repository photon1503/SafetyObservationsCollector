using ASCOM.DriverAccess;
using ASCOM.Utilities;

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

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

        ObservableCollection<SensorItem> items = new ObservableCollection<SensorItem>();
        SensorItem oCloudCover = new SensorItem() { Title = "CloudCover", unit = "%", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oDewPoint = new SensorItem() { Title = "DewPoint", unit = "°C", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oHumidity = new SensorItem() { Title = "Humidity", unit = "%", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oSkyTemperature = new SensorItem() { Title = "Sky Temperature", unit = "°C", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oPressure = new SensorItem() { Title = "Pressure", unit = "hPa", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oRainRate = new SensorItem() { Title = "Rain Rate", unit = "mm/h", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oSkyBrightness = new SensorItem() { Title = "Sky Brightness", unit = "lux", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oSkyQuality = new SensorItem() { Title = "Sky Quality", unit = "mag/sq", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oStarFWHM = new SensorItem() { Title = "Star FWHM", unit = "\"", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oTemperature = new SensorItem() { Title = "Temperature", unit = "°C", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oWindDirection = new SensorItem() { Title = "Wind Direction", unit = "°", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oWindSpeed = new SensorItem() { Title = "Wind speed", unit = "m/s", isChecked = false, age = 0, min = 0, max = 0 };
        SensorItem oWindGust = new SensorItem() { Title = "Wind gust", unit = "m/s", isChecked = false, age = 0, min = 0, max = 0 };

        bool isSafeResult = true;

        public MainWindow()
        {
            InitializeComponent();

            items.Add(oCloudCover);
            items.Add(oDewPoint);
            items.Add(oHumidity);
            items.Add(oSkyTemperature);
            items.Add(oPressure);
            items.Add(oRainRate);
            items.Add(oSkyBrightness);
            items.Add(oStarFWHM);
            items.Add(oSkyQuality);
            items.Add(oTemperature);
            items.Add(oWindDirection);
            items.Add(oWindSpeed);
            items.Add(oWindGust);

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

        private void OnTimedEvent(object sender, EventArgs e)
        {
            bool isSafeSafteyMonitor = oSafetyMonitor.IsSafe;
            colorRect(rectSafteyMonitor, isSafeSafteyMonitor);

            try
            {
                oCloudCover.sValue = oObservingConditions.CloudCover;
            }
            catch (ASCOM.PropertyNotImplementedException ex)
            {
                items.Remove(oCloudCover);
            }
            try
            {
                oDewPoint.sValue = oObservingConditions.DewPoint;
            }
            catch (ASCOM.PropertyNotImplementedException ex) { items.Remove(oDewPoint); }
            try
            {
                oHumidity.sValue = oObservingConditions.Humidity;
            }
            catch (ASCOM.PropertyNotImplementedException ex) { items.Remove(oHumidity); }
            try
            {
                oSkyTemperature.sValue = oObservingConditions.SkyTemperature;
            }
            catch (ASCOM.PropertyNotImplementedException ex) { items.Remove(oSkyTemperature); }
            try
            {
                oPressure.sValue = oObservingConditions.Pressure;
            }
            catch (ASCOM.PropertyNotImplementedException ex) { items.Remove(oPressure); }
            try
            {
                oRainRate.sValue = oObservingConditions.RainRate;
            }
            catch (ASCOM.PropertyNotImplementedException ex) { items.Remove(oRainRate); }
            try
            {
                oSkyBrightness.sValue = oObservingConditions.SkyBrightness;
            }
            catch (ASCOM.PropertyNotImplementedException ex) { items.Remove(oSkyBrightness); }
            try
            {
                oStarFWHM.sValue = oObservingConditions.StarFWHM;
            }
            catch (ASCOM.PropertyNotImplementedException ex)
            {
                items.Remove(oStarFWHM);
            }
            try
            {
                oTemperature.sValue = oObservingConditions.Temperature;
            }
            catch (ASCOM.PropertyNotImplementedException ex) { items.Remove(oTemperature); }
            try
            {
                oWindDirection.sValue = oObservingConditions.WindDirection;
            }
            catch (ASCOM.PropertyNotImplementedException ex) { items.Remove(oWindDirection); }
            try
            {
                oWindSpeed.sValue = oObservingConditions.WindSpeed;
            }
            catch (ASCOM.PropertyNotImplementedException ex) { items.Remove(oWindSpeed); }
            try
            {
                oWindGust.sValue = oObservingConditions.WindGust;
            }
            catch (ASCOM.PropertyNotImplementedException ex) { items.Remove(oWindGust); }

            bool isSafeObs = oCloudCover.isSafe &&
                                oDewPoint.isSafe &&
                                oHumidity.isSafe &&
                                oSkyTemperature.isSafe &&
                                oPressure.isSafe &&
                                oRainRate.isSafe &&
                                oSkyBrightness.isSafe &&
                                oStarFWHM.isSafe &&
                                oSkyQuality.isSafe &&
                                oTemperature.isSafe &&
                                oWindDirection.isSafe &&
                                oWindSpeed.isSafe &&
                                oWindGust.isSafe;

            colorRect(rectObsCond, isSafeObs);

            isSafeResult = isSafeObs && isSafeSafteyMonitor;
            colorRect(rectResult, isSafeResult);
            BoltwoodFile();

            
        }

        void BoltwoodFile()
        {
            string vbdate = "123";
            // Write Boltwood file
            //           10        20        30        40        50        60       
            //   12345678901234567890123456789012345678901234567890123456789012345678901234567890

            int rainflag=0;
            if (oRainRate.sValue>0) { rainflag = 1; }

            

            //   2020-09-12 13:02:46 C m -7,3 
            string boltwood = String.Format("{0,22} {1,1} {2,1} {3,5} {4,5} {5,6} {6,5} {7,2} {8,5} {9,3} {10,1} {11,1} {12,5} {13,11}" +
                " {14,1} {15,1} {16,1} {17,1} {18,1} {19,1}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                "C",                            // 1 unit for temp
                "m",                            // 2 unit for wind 
                oSkyTemperature.sValue,         // 3 Sky-Ambient
                oTemperature.sValue,            // 4 ambient temperature
                0,                               // 5 Sensor case temperature
                oWindSpeed.valueFormatted,               // 6 Wind spped
                oHumidity.sValue,               // 7 Humidity
                oDewPoint.sValue,              // 8 
                0,                              // 9 Heater settings
                oRainRate.sValue > 0 ? 1 : 0,                              // 10 rain flag
                oRainRate.sValue > 0 ? 1 : 0,                              // 11 wet flag
                0,                              // 12 seconds since last valid data
                vbdate,                         // 13 datetime
                0,                              // 14 cloud condition
                0,                              // 15 wind condition
                0,                              // 16 rain condition
                0,                              // 17 daylight condition
                isSafeResult ? 0 : 1,                              // 18 roof close request
                isSafeResult ? 0 : 1                               // 19 alert
                );

            txtBoltwood.Text = "#"+boltwood+"#";
            //Console.WriteLine(boltwood);
        }

        private void colorRect(Rectangle r, bool _isSafe)
        {
            if (_isSafe)
                r.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
            else
                r.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            //todo
        }
    }
}
