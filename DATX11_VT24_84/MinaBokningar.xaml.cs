// MinaBokningar.xaml.cs
using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace DATX11_VT24_84
{
    public partial class MinaBokningar : ContentPage
    {
        public MinaBokningar()
        {
            InitializeComponent();
            AddTopTriangles();
            LoadBookings();
            UIUtility.UpdateBackgroundColorMainPages(this);
        }

        private void AddTopTriangles()
        {
            // Av okänd anledning verkar SizeChanged vara det enda sättet att få korrekt Width och Height
            SizeChanged += (sender, e) =>
            {
                UIUtility.AddTopTriangles(MainLayout, Width, Height);
            };
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
        }

        private async void LoadBookings()
        {
            try
            {
                // Show activity indicator while loading
                activityIndicator.IsVisible = true;
                activityIndicator.IsRunning = true;

                // Retrieve the bookings for the user
                List<Reservation> bookings = await BackEnd.GetUpcomingReservationsForUser("1");

                // Group bookings by date
                var groupedBookings = new Dictionary<DateTime, List<Reservation>>();
                foreach (Reservation booking in bookings)
                {
                    DateTime dateKey = booking.StartTime.Date;
                    Console.WriteLine($"datekey: {dateKey}");
                    if (!groupedBookings.ContainsKey(dateKey))
                    {
                        groupedBookings[dateKey] = new List<Reservation>();
                    }
                    groupedBookings[dateKey].Add(booking);
                }

                // Display the bookings
                foreach (var group in groupedBookings)
                {
                    // Create a gray background frame to encapsulate the day's bookings
                    Frame grayFrame = new Frame
                    {
                        BackgroundColor = Color.FromHex("#36474F"),
                        Margin = new Thickness(15, 10, 15, 10),
                        CornerRadius = 20
                    };
                    stackLayout.Children.Add(grayFrame);

                    // Create a new StackLayout for each day's content
                    StackLayout dayContentLayout = new StackLayout();
                    grayFrame.Content = dayContentLayout;

                    // Add the date label to the day's content layout
                    Label dateLabel = new Label
                    {
                        Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(group.Key.ToString("dddd d MMMM yyyy", new CultureInfo("sv-SE"))), // Display day in Swedish and capitalize the first letter
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 18,
                        HorizontalTextAlignment = TextAlignment.Start,
                        TextColor = Color.White
                    };
                    dayContentLayout.Children.Add(dateLabel);

                    // Display each reservation in the group under the respective day
                    foreach (Reservation booking in group.Value)
                    {
                        // Create a white background for individual reservations
                        Frame reservationFrame = new Frame
                        {
                            BackgroundColor = Color.White,
                            CornerRadius = 20
                        };

                        // Create a grid to contain the time on the left and room name on the right with arrow
                        Grid grid = new Grid();
                        reservationFrame.Content = grid;

                        // Add the start and end time to the left side of the grid
                        Label timeLabel = new Label
                        {
                            Text = $"{booking.StartTime:HH:mm} – {booking.EndTime:HH:mm}",
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 16,
                            TextColor = Color.Black,
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            VerticalOptions = LayoutOptions.Center
                        };
                        grid.Children.Add(timeLabel);

                        // Add the room name to the right side of the grid
                        Label roomLabel = new Label
                        {
                            Text = booking.RoomName,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 16, // Set the font size to 16
                            TextColor = Color.Black,
                            HorizontalOptions = LayoutOptions.End, // Align to the start
                            VerticalOptions = LayoutOptions.Center
                        };
                        Grid.SetColumn(roomLabel, 1); // Set the column index to 1
                        grid.Children.Add(roomLabel);

                        // Attach TapGestureRecognizer to reservationFrame
                        TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += async (sender, e) =>
                        {
                            // Display confirmation booking page with reservation details
                            await DisplayConfirmationBookingPage(booking);
                        };
                        reservationFrame.GestureRecognizers.Add(tapGestureRecognizer);

                        dayContentLayout.Children.Add(reservationFrame);
                    }
                }

                // Hide activity indicator after loading
                activityIndicator.IsVisible = false;
                activityIndicator.IsRunning = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task DisplayConfirmationBookingPage(Reservation booking)
        {
            // Navigate to bokningar page with the selected booking
            await Navigation.PushModalAsync(new Bokning(booking), false);
        }
    }
}
