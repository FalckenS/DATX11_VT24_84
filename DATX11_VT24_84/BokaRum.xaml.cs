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
        public BokaRum(string roomName, string timeFrame)
        {
            InitializeComponent();
            UIUtility.UpdateBackgroundColorOtherPages(this);
            RoomNameLabel.Text = roomName;
            TimeFramLabel.Text = timeFrame;
        }
    }
}