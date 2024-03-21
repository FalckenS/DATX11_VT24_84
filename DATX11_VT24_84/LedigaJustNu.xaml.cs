using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LedigaJustNu : ContentPage
    {
        // instance för att kunna använda funktionerna i MainPage.xaml.cs
        private MainPage _mainPageInstance = new MainPage();
        public LedigaJustNu()
        {
            InitializeComponent();
            UIHelper.UpdateBackgroundColor(this);
            //_mainPageInstance.AddTrianglesAndBackButton();
        }
        
        private async void OnBokaButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new Boka());
            //_mainPageInstance.OnBokaButtonClicked(sender, e);
        }
            
    }
}