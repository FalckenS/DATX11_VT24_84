using System;

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
            await Navigation.PushModalAsync(new LedigaJustNu(), false);
            Room room = await BackEnd.GetRoomByName("432q43");
        }
        
        private async void OnBokaButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new Boka());
        }
        
        private async void OnMinaBokningarButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new MinaBokningar());
        }
    }
}