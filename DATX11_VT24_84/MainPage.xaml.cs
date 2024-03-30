﻿using System;
using System.Collections.Generic;

namespace DATX11_VT24_84
{
    public partial class MainPage
    {
        public MainPage()
        {   
            InitializeComponent();
            UIUtility.UpdateBackgroundColorMainPages(this);
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
            await Navigation.PushModalAsync(new LedigaJustNu(), false);
        }
        
        private async void OnBokaButtonClicked(object sender, EventArgs e)
        {
        }
        
        private async void OnMinaBokningarButtonClicked(object sender, EventArgs e)
        {
        }
    }
}