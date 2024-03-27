using System;
using Google.Apis.Calendar.v3.Data;
using Xamarin.Forms.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace DATX11_VT24_84
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            UIUtility.UpdateBackgroundColor(this);
            AddTrianglesAndBackButton();
        }

        private void AddTrianglesAndBackButton()
        {
            // Av okänd anledning verkar SizeChanged vara det enda sättet att få korrekt Width och Height
            SizeChanged += (sender, e) =>
            {
                UIUtility.AddTopTriangles(MainLayout, Width, Height);
            };
        }
        
        private async void OnLedigaJustNuButtonClicked(object sender, EventArgs e)
        {
        }
        
        private async void OnBokaButtonClicked(object sender, EventArgs e)
        {
        }
        
        private async void OnMinaBokningarButtonClicked(object sender, EventArgs e)
        {
        }
    }
}