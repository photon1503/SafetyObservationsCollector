using System;
using System.ComponentModel;
using System.Windows.Media;

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

            public bool isImplemented = true;
            private double _sValue = -999;
            private bool _isSafe = true;
            private bool _isChecked = false;

            public double sValue
            {
                get { return _sValue; }
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

            public double min { get; set; }

            public double max { get; set; }

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
