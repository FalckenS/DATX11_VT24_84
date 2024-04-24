using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DATX11_VT24_84
{
    public partial class Bokning
    {
        private readonly Reservation _booking;
        private const string UserID = "1";
        private CancellationTokenSource _cancellationTokenSource;

        public Bokning(Reservation booking)
        {
            _booking = booking;
            InitializeComponent();
            LoadTexts(booking);
            Task.Run(() => AnimatePulsatingEffect(ConfirmButtonLabel, _cancellationTokenSource.Token));
            LoadConfirmBookingButton(booking);
        }

        // Loads all relevant text, except for confirmBookingButton
        private async void LoadTexts(Reservation booking)
        {
            TimeLabel.Text = $"{booking.StartTime:HH:mm} - {booking.EndTime:HH:mm}";
            RoomLabel.Text = $"{booking.RoomName}";
            DateLabel.Text = $"{booking.StartTime:M}";
            LocationLabel.Text = $"{(await BackEnd.GetRoomInfo(booking.RoomName)).Building},";
            FloorLabel.Text = $"våning: {(await BackEnd.GetRoomInfo(booking.RoomName)).Floor}";
        }

        // Load the confirmBookingButton and determine what event should be called based on confirmation status
        private async Task LoadConfirmBookingButton(Reservation booking)
        {
            List<Reservation> ongoingReservations = await BackEnd.GetOngoingReservationsForUser(UserID);
            List<Reservation> upcomingReservations = await BackEnd.GetUpcomingReservationsForUser(UserID);
            List<Reservation> allReservations = ongoingReservations.Concat(upcomingReservations).ToList();

            // Find the reservation in the list
            Reservation reservation = allReservations.FirstOrDefault(i => i.ID == booking.ID);

            if (reservation == null) return;
            if (reservation.Confirmed)
            {
                ConfirmButtonLabel.Text = "\u2713 Bokning Bekräftad";
                ConfirmButtonLabel.BackgroundColor = Color.FromHex("#409c40");
                ConfirmButtonLabel.IsEnabled = false;
            }
            else
            {
                TimeSpan timeUntilReservation = reservation.StartTime - GetRealTime();
    
                // Confirmation ending after 20 minutes
                if (timeUntilReservation.TotalMinutes < -20)
                {
                    ConfirmButtonLabel.Text = "Bekräftningstiden är över";
                    ConfirmButtonLabel.BackgroundColor = Color.IndianRed;
                    ConfirmButtonLabel.IsEnabled = false;
                }
                // Confirmation ending between -20 and 20 minutes
                else if (timeUntilReservation.TotalMinutes <= 0)
                {
                    ConfirmButtonLabel.Text = "Bekräfta bokning | Stänger om "
                                              + (20 + (int)timeUntilReservation.TotalMinutes) + " min";
                    ConfirmButtonLabel.BackgroundColor = Color.LightGreen;
                    ConfirmButtonLabel.IsEnabled = true;
                }
                // Display time left in minutes if less than 1 hour to reservation #"
                else if (timeUntilReservation.TotalHours < 1)
                {
                    ConfirmButtonLabel.Text = "Bekräfta bokning | Öppnar om " + (int)timeUntilReservation.TotalMinutes + " min";
                    ConfirmButtonLabel.BackgroundColor = Color.LightGreen;
                    ConfirmButtonLabel.IsEnabled = false;
                }
                // Display time left in hours if more than or equal to 1 hour to reservation
                else if (timeUntilReservation.TotalHours <= 24)
                {
                    ConfirmButtonLabel.Text = "Bekräfta bokning | Öppnar Om " + (int)timeUntilReservation.TotalHours + " h";
                    ConfirmButtonLabel.BackgroundColor = Color.LightGreen;
                    ConfirmButtonLabel.IsEnabled = false;
                }
                else if (timeUntilReservation.TotalHours <= 48)
                {
                    ConfirmButtonLabel.Text = "Bekräfta bokning | Öppnar imorgon";
                    ConfirmButtonLabel.BackgroundColor = Color.LightGreen;
                    ConfirmButtonLabel.IsEnabled = false;
                }
                else
                {
                    ConfirmButtonLabel.Text = "Bekräfta bokning | Öppnar Om " + (int)timeUntilReservation.TotalDays + " dagar";
                    ConfirmButtonLabel.BackgroundColor = Color.LightGreen;
                    ConfirmButtonLabel.IsEnabled = false;
                }
            }

            // end pulsating animation if the button pulsates
            _cancellationTokenSource?.Cancel();
        }

        private async Task AnimatePulsatingEffect(View view, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await view.FadeTo(0.5, 500, Easing.SinInOut); // Fade to lower opacity
                await view.FadeTo(1, 500, Easing.SinInOut);   // Fade back to full opacity
            }
        }

        private static DateTime GetRealTime()
        {
            // Vintertid: +1
            // Sommartid: +2
            return DateTime.Now.AddHours(2);
        }
        

        private async void OnBackButtonClicked(object sender, EventArgs e)
        { 
            PreviousPage(); // Corrected method call
        }
        public class UpdateMessage
        {
            public string Message { get; set; }
            // Add more properties as needed
        }
        private async void PreviousPage()
        {
            UpdateMessage message = new UpdateMessage { Message = "Updated information" };
            MessagingCenter.Send(this, "UpdatePage", message);

            await Navigation.PopModalAsync();
        }

        private async void OnCancelBookingClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Avboka", "Är du säker att du vill avboka?", "Ja, avboka", "Nej, avboka inte");

            if (answer)
            {
                await BackEnd.DeleteReservation(_booking.ID);
                PreviousPage();
            }
            // If the user clicks "No", no action needs to be taken.
        }

        private void OnRescheduleBookingClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OnMapClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LiveKarta(), false);
        }

        private async void OnConfirmedBookingClicked(object sender, EventArgs e)
        {
            ConfirmButtonLabel.IsEnabled = false;
            _cancellationTokenSource = new CancellationTokenSource();
            // Start a new thread for the pulsating animation,
            // OBS! SHOULD NOT BE AWAIT, animation never ends then
            Task.Run(() => AnimatePulsatingEffect(ConfirmButtonLabel, _cancellationTokenSource.Token));
            await BackEnd.ConfirmReservation(_booking.ID);
            await LoadConfirmBookingButton(_booking); // Update button state
        }
    }
}
