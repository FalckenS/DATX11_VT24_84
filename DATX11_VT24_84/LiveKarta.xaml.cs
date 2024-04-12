using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LiveKarta : ContentPage
    {
        public LiveKarta()
        {
            InitializeComponent();
            UIUtility.UpdateBackgroundColorOtherPages(this);

        }
        
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
        }
    }
}