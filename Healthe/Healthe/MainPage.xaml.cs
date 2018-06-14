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
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            Distance = 2.3;
            Calories = 340;
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
