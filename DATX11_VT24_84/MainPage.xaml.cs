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
    public partial class MainPage : IHasBackButton
    {
        public MainPage()
        {
            InitializeComponent();
            UIHelper.UpdateBackgroundColor(this);
            AddTrianglesAndBackButton();
        }
        
        public void AddClickedMethod(ImageButton backButton)
        {
            backButton.Clicked += OnXButtonClicked;
        }

        private void AddTrianglesAndBackButton()
        {
            // Av okänd anledning verkar SizeChanged vara det enda sättet att få korrekt Width och Height
            SizeChanged += (sender, e) =>
            {
                UIHelper.AddTopTriangles(MainLayout, Width, Height);
                UIHelper.AddBackButton(MainLayout, this);
            };
        }
        
        private async void OnLedigaJustNuButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new LedigaJustNu());
        }
        
        private async void OnBokaButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new Boka());
        }
        
        private async void OnMinaBokningarButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new MinaBokningar());
        }

        private async void OnXButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}