using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DATX11_VT24_84
{
    public partial class LedigaJustNu : IHasBackButton
    {
        public LedigaJustNu()
        {
            InitializeComponent();
            AddGrids();
            UIUtility.UpdateBackgroundColorMainPages(this);
            ShowCurrentDateTime();
        }

        public void AddClickedMethod(ImageButton backButton)
        {
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
        }

        private void ShowCurrentDateTime()
        {
            // Update the DateTime label with the current date and time
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    string formattedDateTime = DateTime.Now.ToString("dddd d MMMM, HH:mm", new System.Globalization.CultureInfo("sv-SE"));
                    formattedDateTime = char.ToUpper(formattedDateTime[0]) + formattedDateTime.Substring(1);
                    dateTimeLabel.Text = formattedDateTime;
                });
                return true;
            });
        }

        private async Task AddGrids()
        {
            var activityIndicator = new ActivityIndicator
            {
                IsRunning = true,
                IsVisible = true,
                Color = Color.White,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            StackLayoutFrames.Children.Add(activityIndicator);

            List<Room> availableRooms = await BackEnd.GetAllRoomsAvailableNow();
            var groupedRooms = availableRooms.GroupBy(room => room.Building);

            StackLayoutFrames.Children.Remove(activityIndicator);

            foreach (var group in groupedRooms)
            {
                var frame = new Frame
                {
                    BackgroundColor = Color.FromHex("#36474F"),
                    CornerRadius = 15,
                    Margin = new Thickness(50, 5), // Adjust margins here
                    Padding = new Thickness(30) // Add padding to make it a bit squished together from the sides
                };

                var titleLabel = new Label
                {
                    Text = group.Key,
                    TextColor = Color.White,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center
                };

                frame.Content = titleLabel;

                var liveMapButton = new Button
                {
                    Text = "Se livekarta över " + group.Key,
                    TextColor = Color.White,
                    BackgroundColor = Color.FromHex("#00ACFF"),
                    VerticalOptions = LayoutOptions.Center,
                    CornerRadius = 20,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 5, 0, 10) 
                };

                liveMapButton.Clicked += async (sender, e) =>
                {
                    await Navigation.PushModalAsync(new LiveKarta(), false);
                };

                var roomsStackLayout = new StackLayout();

                foreach (var room in group)
                {
                    var grid = new Grid();

                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    var label1 = new Label { Text = $"{room.Name}", TextColor = Color.White, VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold, FontSize = 18 };
                    var capacityLabel = new Label { Text = $"Kapacitet: {room.Capacity} platser", TextColor = Color.White, VerticalOptions = LayoutOptions.Center, FontSize = 14 }; // Adjust FontSize as needed
                    var button = new Button { Text = "Boka", TextColor = Color.White, BackgroundColor = Color.FromHex("#00ACFF"), VerticalOptions = LayoutOptions.Center, CornerRadius = 30, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 5, 0, 0) };

                    Grid.SetRow(capacityLabel, 1);

                    Grid.SetColumn(label1, 0);
                    Grid.SetColumn(capacityLabel, 0);
                    Grid.SetColumn(button, 2);

                    grid.Children.Add(label1, 0, 0);
                    grid.Children.Add(capacityLabel, 0, 1);
                    grid.Children.Add(button, 2, 0);

                    button.Clicked += async (sender, e) =>
                    {
                        var roomName = label1.Text;
                        var building = room.Building;
                        var floor = room.Floor;
                        var capacity = room.Capacity;
                        var today = DateTime.Today;
                        await Navigation.PushModalAsync(new BokaRum(roomName, building, floor, today, capacity, today), false);
                    };

                    roomsStackLayout.Children.Add(grid);

                    var separatorLine = new SeparatorLine();
                    roomsStackLayout.Children.Add(separatorLine);
                }

                frame.Content = new StackLayout
                {
                    Children = { titleLabel, liveMapButton, roomsStackLayout }
                };
                StackLayoutFrames.Children.Add(frame);
            }
        }

       
    }
}
