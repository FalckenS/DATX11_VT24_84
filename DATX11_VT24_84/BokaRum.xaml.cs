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
    public partial class BokaRum : ContentPage
    {
        
        //Kan enkelt lägga till timeFramen också på denna sida, inte speciellt svårt. Behövs inte för nu dock. 
        public BokaRum(string roomName)
        {
            InitializeComponent();
            UIUtility.UpdateBackgroundColorOtherPages(this);
            RoomNameLabel.Text = roomName;
            //TimeFramLabel.Text = timeFrame;
        }
    }
}