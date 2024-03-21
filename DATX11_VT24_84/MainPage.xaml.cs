using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System;
using Xamarin.Forms;

namespace DATX11_VT24_84
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            UIHelper.UpdateBackgroundColor(this);
            AddTrianglesAndBackButton();
        }

        public void AddTrianglesAndBackButton()
        {
            // Av okänd anledning verkar SizeChanged vara det enda sättet att få korrekt Width och Height
            SizeChanged += (sender, e) =>
            {
                UIHelper.AddTopTriangles(MainLayout, Width, Height);
            };
        }
        
        private async void OnLedigaJustNuButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LedigaJustNu());
        }
        
        public async void OnBokaButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new Boka());
        }
        // ändra från privat för att kunna använda
        
        public async void OnMinaBokningarButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new MinaBokningar());
        }
        // ändra från privat för att kunna använda
    }
}