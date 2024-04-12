using System;

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
            TimeLabel.Text = $"{booking.StartTime:HH:mm} - {booking.EndTime:HH:mm}";
            RoomLabel.Text = $"{booking.RoomName}";
            DateLabel.Text = $"{booking.StartTime:M}";
            LocationLabel.Text = $"{(await BackEnd.GetRoomInfo(booking.RoomName)).Building},";
            FloorLabel.Text = $"våning: {(await BackEnd.GetRoomInfo(booking.RoomName)).Floor}";
        }
        
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }                                  
        private void OnEndCancelBookingClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }


        private void OnResceduleBookingClocked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnMapClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}