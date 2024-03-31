using System.Transactions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BokaRum : ContentPage
    {
        public BokaRum(string roomName, string building, string floor)
        {
            InitializeComponent();
            UIUtility.UpdateBackgroundColorOtherPages(this);
            RoomNameLabel.Text = roomName;
            BuildingLabel.Text = building; // Set the building information
            FloorLabel.Text = $"Våning  {floor}";
        }
    }
}