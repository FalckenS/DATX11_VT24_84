﻿using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace DATX11_VT24_84
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
            FakeBookingCreator.CreatePreMadeBookings(); // Optional
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}