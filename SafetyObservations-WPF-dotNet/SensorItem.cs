using System;
using System.ComponentModel;
using System.Windows.Media;
using ASCOM.Utilities;

namespace SafetyObservations_WPF_dotNet
{
    public partial class MainWindow
    {
        public class SensorItem : INotifyPropertyChanged
        {


            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public string Title { get; set; }

            //public bool isImplemented = true;
            private double _sValue = -999;
            private bool _isSafe = true;
            private bool _isChecked = false;

            private Profile oProfile;
            private string profileID = "WeatherSafetyCollector";

            public SensorItem(string _title, string _unit)
            {
                oProfile = new Profile();
                if (!oProfile.IsRegistered(profileID))
                {
                    oProfile.Register(profileID, "Weather Safety Collector Profile");
                }
                Title = _title;
                unit = _unit;
                sValue = 999;
                isChecked = bool.Parse(oProfile.GetValue(profileID, "isChecked", this.Title, "false"));
                age = 0;
                min = double.Parse(oProfile.GetValue(profileID, "min", this.Title, "0"));
                max = double.Parse(oProfile.GetValue(profileID, "max", this.Title, "0"));
            }

            public double sValue
            {
                get
                {
                    if (this.Title == "Wind speed")
                        return Math.Abs(_sValue);

                    return _sValue;
                }
                set
                {
                    if (value != _sValue)
                    {
                        _sValue = value;
                        NotifyPropertyChanged("valueFormatted");            //Notify the UI that the value needs to be refreshed
                        NotifyPropertyChanged("color");
                    }
                }
            }

            public string valueEN
            {
                get
                {
                    return this.sValue.ToString("0.00", System.Globalization.CultureInfo.CreateSpecificCulture("en-us"));
                }
                set { }
            }

            public string valueFormatted
            {
                get
                {
                    return this.sValue.ToString("F2");
                }
                set { }
            }

            public string unit { get; set; }

            public bool isChecked
            {
                get
                {
                    return _isChecked;
                }
                set
                {
                    if (value != _isChecked)
                    {
                        _isChecked = value;
                        NotifyPropertyChanged("color");
                        oProfile.WriteValue(profileID, "isChecked", _isChecked.ToString(), this.Title);
                    }
                }
            }

            public bool isSafe
            {
                get
                {
                    if (!this.isChecked) return true;

                    isSafe = this.sValue >= this.min && this.sValue <= this.max;
                    return _isSafe;
                }
                set
                {
                    if (value != _isSafe)
                    {
                        _isSafe = value;
                        NotifyPropertyChanged("color");
                    }
                }
            }
            public double age { get; set; }

            private double _min = 0;
            public double min
            {
                get { return _min; }
                set
                {
                    if (_min != value)
                    {
                        _min = value;
                        oProfile.WriteValue(profileID, "min", _min.ToString(), this.Title);
                    }
                }
            }

            private double _max;
            public double max
            {
                get { return _max; }
                set
                {
                    if (_max != value)
                    {
                        _max = value;
                        oProfile.WriteValue(profileID, "max", _max.ToString(), this.Title);
                    }
                }
            }

            public Brush color
            {
                get
                {
                    if (!this.isChecked)
                        return Brushes.LightGray;

                    if (this.isSafe)
                        return Brushes.Green;
                    else
                        return Brushes.Red;
                }
                set { }
            }
        }
    }
}
