using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TestApp.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            BindingContext = new { Username = "Not Set" };
            InitializeComponent();
        }
    }
}
