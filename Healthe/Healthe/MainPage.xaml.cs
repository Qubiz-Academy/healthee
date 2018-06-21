using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace Healthe
{
    public partial class MainPage : ContentPage
    {
        DistanceCalculator distanceCalculator;
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            Distance = 2.3;
            Calories = 340;
            Ok = IsLocationAvailable();
            distanceCalculator = new DistanceCalculator(new Coordinate(47.042735223403916, 21.934252070104094));
            var distance = distanceCalculator.GetDistanceInKilometers(new Coordinate(47.0414104187296, 21.93694286140203));
            Distance = Math.Round(distance, 2);
        }

        public bool IsLocationAvailable()
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
