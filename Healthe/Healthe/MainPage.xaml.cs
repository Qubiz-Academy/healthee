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
using Xamarin.Forms.Maps;

namespace Healthe
{
    public partial class MainPage : ContentPage
    {
        DistanceCalculator distanceCalculator;
        Coordinate _lastPosition;
        Double DistanceInKilometers = 0;
        DateTime initialTime;
        Timer timer;

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            Distance = 0;
            Calories = 0;
            Minutes = 0;
            Ok = IsLocationAvailable();


            var position = CrossGeolocator.Current.GetLastKnownLocationAsync().Result;
            // distanceCalculator = new DistanceCalculator(new Coordinate(position.Latitude, position.Longitude));
            //var distance = distanceCalculator.GetDistanceInKilometers(new Coordinate(47.0414104187296, 21.93694286140203));
            //Distance = Math.Round(distance, 2);
            StartTracking();
            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
            initialTime = DateTime.Now;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            TimeSpan result = now.Subtract(initialTime);

            Minutes = (int)result.TotalMinutes;
            Seconds = result.Seconds;
        }

        private void Current_PositionChanged(object sender, PositionEventArgs e)
        {
            var newPosition = new Xamarin.Forms.Maps.Position(e.Position.Latitude, e.Position.Longitude);

            var mapSpan = new MapSpan(newPosition, 0.1, 0.1);
            RunMap.MoveToRegion(mapSpan);

            var pin = new Pin();
            pin.Position = newPosition;
            pin.Type = PinType.Generic;
            pin.Label = "test";

            RunMap.Pins.Add(pin);

            Coordinate coordinate = new Coordinate(e.Position.Latitude, e.Position.Longitude);
            if (_lastPosition == null)
            {
                _lastPosition = coordinate;
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
            Seconds = result.Seconds;
        }

        public void StartTracking()
        {
            CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(1), 0.1);

        }

        public static async Task<Plugin.Geolocator.Abstractions.Position> GetCurrentPosition()
        {
            Plugin.Geolocator.Abstractions.Position position = null;
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;

                position = await locator.GetLastKnownLocationAsync();

                if (position != null)
                {
                    return position;
                }
                if (!IsLocationAvailable())
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
            set
            {
                ok = value;
                OnPropertyChanged();
            }
        }
        private int _minutes;

        public int Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
                OnPropertyChanged();
            }
        }

        private int _seconds;

        public int Seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
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
                Calories = (int)(_distance * 50);
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
