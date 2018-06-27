using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;


namespace Healthe
{
    public partial class MainPage : ContentPage
    {
        DistanceCalculator distanceCalculator;
        Coordinate _lastPosition;
        Double DistanceInKilometers=0;
        DateTime initialTime;

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            Distance = 2.3;
            Calories = 340;
            Minutes = 0;
            Ok = IsLocationAvailable();
            
            
            var position = CrossGeolocator.Current.GetLastKnownLocationAsync().Result;
           // distanceCalculator = new DistanceCalculator(new Coordinate(position.Latitude, position.Longitude));
            //var distance = distanceCalculator.GetDistanceInKilometers(new Coordinate(47.0414104187296, 21.93694286140203));
            //Distance = Math.Round(distance, 2);
            StartTracking();
            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
            initialTime = DateTime.Now;
        }

        private void Current_PositionChanged(object sender, PositionEventArgs e)
        {
            Coordinate coordinate = new Coordinate(e.Position.Latitude, e.Position.Longitude);
            if(_lastPosition==null)
            {
                _lastPosition =coordinate;
                return;

            }
            distanceCalculator = new DistanceCalculator(_lastPosition);
            var distance = distanceCalculator.GetDistanceInKilometers(coordinate);
            DistanceInKilometers += distance;

            Distance = Math.Round(DistanceInKilometers, 2);
            _lastPosition = coordinate;
            var now = DateTime.Now;
            TimeSpan result = now.Subtract(initialTime);

            Minutes = (int)result.TotalMinutes;
        }

        public void StartTracking()
        {
            CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(1), 0.1);
            
        }

        public static async Task<Position> GetCuerrentPosition()
        {
            Position position = null;
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;

                position = await locator.GetLastKnownLocationAsync();

                if(position!=null)
                {
                    return position;
                }
                if(!IsLocationAvailable())
                {
                    return null;
                }
                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location: " + ex);
            }

            if (position == null)
                return null;
            var output = string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAltitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
                    position.Timestamp, position.Latitude, position.Longitude,
                    position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);
            Debug.WriteLine(output);

            return position;
        }

        public static bool IsLocationAvailable()
        {
            if (!CrossGeolocator.IsSupported)
                return false;

            return CrossGeolocator.Current.IsGeolocationAvailable;
        }
        private bool ok;

        public bool Ok
        {
            get { return ok; }
            set {
                ok = value;
                OnPropertyChanged();
            }
        }
        private int _minutes;

        public int Minutes
        {
            get { return _minutes; }
            set { _minutes = value;
                OnPropertyChanged();
            }
        }

        private double _distance;
        public double Distance
        {
            get { return _distance; }
            set
            {
                _distance = value;
                OnPropertyChanged();
            }
        }

        private int _calories;

        public int Calories
        {
            get { return _calories; }
            set
            {
                _calories = value;
                OnPropertyChanged();
            }
        }
    }
}
