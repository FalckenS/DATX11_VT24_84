using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace DATX11_VT24_84
{
    public partial class MinaBokningar : ContentPage
    {
        public MinaBokningar()
        {
            InitializeComponent();
            LoadBookings();
        }

        private async void LoadBookings()
        {
            try
            {

                // Retrieve the bookings for the user
                List<Reservation> bookings = await BackEnd.GetReservationsForUser("userID");

                // Display the bookings
                foreach (Reservation booking in bookings)
                {
                    Label label = new Label();
                    label.Text = $"Room: {booking.RoomName}, Start: {booking.StartTime}, End: {booking.EndTime}";
                    stackLayout.Children.Add(label);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
