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

                // Group bookings by date
                var groupedBookings = new Dictionary<DateTime, List<Reservation>>();
                foreach (Reservation booking in bookings)
                {
                    DateTime dateKey = booking.StartTime.Date;
                    if (!groupedBookings.ContainsKey(dateKey))
                    {
                        groupedBookings[dateKey] = new List<Reservation>();
                    }
                    groupedBookings[dateKey].Add(booking);
                }

                // Display the bookings
                foreach (var group in groupedBookings)
                {
                    Frame groupFrame = new Frame
                    {
                        BackgroundColor = Color.DarkGray,
                        CornerRadius = 10,
                        Margin = new Thickness(10),
                        Padding = new Thickness(10)
                    };

                    StackLayout groupLayout = new StackLayout();

                    Label dateLabel = new Label
                    {
                        Text = group.Key.ToString("dddd yyyy-MM-dd"),
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.White
                    };
                    groupLayout.Children.Add(dateLabel);

                    foreach (Reservation booking in group.Value)
                    {
                        Frame bookingFrame = new Frame
                        {
                            BackgroundColor = Color.LightGray,
                            CornerRadius = 10,
                            Margin = new Thickness(0, 5, 0, 0),
                            Padding = new Thickness(10)
                        };

                        StackLayout bookingLayout = new StackLayout();

                        Label timeLabel = new Label
                        {
                            Text = $"{booking.StartTime.ToString("HH:mm")}-{booking.EndTime.ToString("HH:mm")}",
                            FontAttributes = FontAttributes.Bold
                        };
                        bookingLayout.Children.Add(timeLabel);

                        Label roomLabel = new Label
                        {
                            Text = booking.RoomName,
                            HorizontalOptions = LayoutOptions.EndAndExpand
                        };
                        bookingLayout.Children.Add(roomLabel);

                        bookingFrame.Content = bookingLayout;
                        groupLayout.Children.Add(bookingFrame);
                    }

                    groupFrame.Content = groupLayout;
                    stackLayout.Children.Add(groupFrame);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
