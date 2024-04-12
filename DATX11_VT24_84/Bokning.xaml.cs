using System;
using Xamarin.Forms;

namespace DATX11_VT24_84
{
    public partial class Bokning
    {
        public Bokning(Reservation booking)
        {
            InitializeComponent();
            UIUtility.UpdateBackgroundColorMainPages(this);
            LoadTexts(booking);
        }

        private async void LoadTexts(Reservation booking)
        {
            // Populate labels with reservation details
            TimeLabel.Text = $"{booking.StartTime:HH:mm} - {booking.EndTime:HH:mm}";
            RoomLabel.Text = $"{booking.RoomName}";
            DateLabel.Text = $"{booking.StartTime:M}";
            Console.WriteLine($"booking.RoomName: {booking.RoomName}");
            //LocationLabel.Text = $"{(await BackEnd.GetRoomInfo(booking.RoomName)).Building}";
        }
        
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }                                  
        private void OnEndCancelBookingClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        
        
        
        
        
        
        
    }
}