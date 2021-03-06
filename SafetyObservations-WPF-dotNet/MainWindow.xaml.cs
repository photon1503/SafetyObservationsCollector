﻿using ASCOM.DriverAccess;
using ASCOM.Utilities;

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WinForms = System.Windows.Forms;
using System.IO;


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

        private string myPath; //safeme

        ObservableCollection<SensorItem> items = new ObservableCollection<SensorItem>();
        SensorItem oCloudCover = new SensorItem("CloudCover", "%");
        SensorItem oDewPoint = new SensorItem("DewPoint", "°C");
        SensorItem oHumidity = new SensorItem("Humidity", "%");
        SensorItem oSkyTemperature = new SensorItem("Sky Temperature", "°C");
        SensorItem oPressure = new SensorItem("Pressure", "hPa");
        SensorItem oRainRate = new SensorItem("Rain Rate", "mm/h");
        SensorItem oSkyBrightness = new SensorItem("Sky Brightness", "lux");
        SensorItem oSkyQuality = new SensorItem("Sky Quality", "mag/sq");
        SensorItem oStarFWHM = new SensorItem("Star FWHM", "\"");
        SensorItem oTemperature = new SensorItem("Temperature", "°C");
        SensorItem oWindDirection = new SensorItem("Wind Direction", "°");
        SensorItem oWindSpeed = new SensorItem("Wind speed", "m/s");
        SensorItem oWindGust = new SensorItem("Wind gust", "m/s");

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
            myPath = oProfile.GetValue(profileID, "weatherPath");
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

            bntStart.IsEnabled = false;
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
            // Write Boltwood file
            // see https://diffractionlimited.com/wp-content/uploads/2016/04/Cloud-SensorII-Users-Manual.pdf
            //           10        20        30        40        50        60        70        80        90       100   *
            //   12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234
            //   2020-09-12 14:58:46.54 C m   -3,9   37,6      0   0,28  -1    100   0 0 0     0  44086,62414 0 0 0 0 1 1

            string boltwood = String.Format("{0,22} {1,1} {2,1} {3,6} {4,6} {5,6} {6,6} {7,3} {8,6} {9,3} {10,1} {11,1} {12,5} {13,12}" +
                " {14,1} {15,1} {16,1} {17,1} {18,1} {19,1}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                "C",                            // 1 unit for temp
                "m",                            // 2 unit for wind 
                oSkyTemperature.valueEN,         // 3 Sky-Ambient
                oTemperature.valueEN,            // 4 ambient temperature
                0,                               // 5 Sensor case temperature
                oWindSpeed.valueEN,       // 6 Wind spped
                oHumidity.valueEN,               // 7 Humidity
                oDewPoint.valueEN,              // 8 Dew Point
                0,                              // 9 Heater settings
                oRainRate.sValue > 0 ? 1 : 0,                              // 10 rain flag
                oRainRate.sValue > 0 ? 1 : 0,                              // 11 wet flag
                0,                              // 12 seconds since last valid data
                DateTime.Now.ToOADate().ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-us")).Substring(0, 11),                         // 13 datetime
                0,                              // 14 cloud condition
                0,                              // 15 wind condition
                0,                              // 16 rain condition
                0,                              // 17 daylight condition
                isSafeResult ? 0 : 1,                              // 18 roof close request
                isSafeResult ? 0 : 1                               // 19 alert
                );

            txtBoltwood.Text = "#" + boltwood + "#";

            try
            {
                File.WriteAllText(System.IO.Path.Combine(myPath, "soc-dot.dat"), boltwood);
                File.WriteAllText(System.IO.Path.Combine(myPath, "soc-comma.dat"), boltwood.Replace('.', ','));
            }
            catch (Exception ex)
            {
                throw new System.InvalidOperationException("Please set folder for weather file first!");
            }
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

        private void btnSetFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();
            myPath = dialog.SelectedPath;
            oProfile.WriteValue(profileID, "weatherPath", myPath);

        }
    }
}
