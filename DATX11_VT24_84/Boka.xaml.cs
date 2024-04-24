using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Boka : ContentPage
    {
        private DateTime _currentDate;

        public Boka()
        {
            InitializeComponent();
            _currentDate = DateTime.Today;
            UpdateDateLabel();
            UpdateDatePicker();
            PopulateGroupRooms();
        }
        
        

        private async void PopulateGroupRooms()
{
    var activityIndicator = new ActivityIndicator
    {
        IsRunning = true,
        IsVisible = true,
        Color = Color.White,
        HorizontalOptions = LayoutOptions.CenterAndExpand,
        VerticalOptions = LayoutOptions.CenterAndExpand
    };

    ListMapFrame.Content = activityIndicator;

    List<Reservation> allBookings = (await BackEnd.GetRoomsWithReservationsForDate(_currentDate))
        .SelectMany(roomWithReservations => roomWithReservations.Reservations)
        .ToList();

    List<string> roomNames = await BackEnd.GetAllRoomNames();
    StackLayout roomNamesContainer = new StackLayout();

    double hourLabelWidth = 30; // Adjust as needed
    double timeBarWidth = hourLabelWidth * (17 - 8 + 1); // Assuming each hour label has a width of 40
    Grid timeBarGrid = new Grid
    {
        BackgroundColor = Color.LightGray,
        HeightRequest = 40,
        WidthRequest = timeBarWidth,
        HorizontalOptions = LayoutOptions.Start,
        VerticalOptions = LayoutOptions.Start,
        Margin = new Thickness(77, 0, 0, 0), // Added margin to the left
        ColumnSpacing = 0
    };

    // Add labels for each hour from 8 AM to 5 PM
    for (int i = 8; i <= 17; i++)
    {
        timeBarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

        Label hourLabel = new Label
        {
            Text = i.ToString("00"),
            FontSize = 14,
            FontAttributes = FontAttributes.Bold, // Set font to bold
            TextColor = Color.Black,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };


        Grid.SetColumn(hourLabel, i - 8);
        timeBarGrid.Children.Add(hourLabel);
    }

    roomNamesContainer.Children.Add(timeBarGrid);

    foreach (string roomName in roomNames)
    {
        Frame greenFrame = new Frame
        {
            Padding = new Thickness(0),
            BackgroundColor = Color.FromHex("#27AD72"),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            WidthRequest = 125,
            HeightRequest = 65, // Increased height to accommodate two labels
            BorderColor = Color.Black,
            HasShadow = false
        };

        Label roomLabel = new Label
        {
            Text = roomName,
            FontSize = 16,
            TextColor = Color.White,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = TextAlignment.Start,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.Start, // Align to the top
            Padding = new Thickness(5, 2, 0, 0)
        };

        Label buildingLabel = new Label
        {
            Text = "", // Leave it blank for now
            FontSize = 14,
            TextColor = Color.White,
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = TextAlignment.Start,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.End, // Align to the bottom
            Padding = new Thickness(5, 0, 0, 2)
        };

        StackLayout labelsLayout = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            Children = { roomLabel, buildingLabel }
        };

        greenFrame.Content = labelsLayout;

        var roomInfo = await BackEnd.GetRoomInfo(roomName);
        if (roomInfo != null)
        {
            buildingLabel.Text = roomInfo.Building; 
        }
        greenFrame.Content = labelsLayout;
        StackLayout frameLayout = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Spacing = 0
        };

        Grid bookingGrid = new Grid
        {
            BackgroundColor = Color.LightGray,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand,
            Padding = new Thickness(0),
            Margin = new Thickness(5, 0, 0, 0)
        };

        DateTime currentTime = _currentDate.Date.AddHours(8);
        for (int i = 0; i < 10; i++)
        {
            BoxView box = new BoxView { Margin = new Thickness(0.5) };
            DateTime bookingStartTime = currentTime;
            DateTime bookingEndTime = currentTime.AddHours(1);

            if (allBookings.Any(b => b.RoomName == roomName && b.StartTime <= bookingStartTime && b.EndTime > bookingStartTime))
            {
                box.BackgroundColor = Color.Red;
            }
            else
            {
                box.BackgroundColor = Color.White;
            }

            Grid.SetColumn(box, i);
            bookingGrid.Children.Add(box);

            box.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    var room = await BackEnd.GetRoomInfo(roomName);
                    await Navigation.PushModalAsync(new BokaRum(roomName, room.Building, room.Floor, _currentDate, room.Capacity));
                })
            });

            currentTime = bookingEndTime;
        }

        frameLayout.Children.Add(greenFrame);
        frameLayout.Children.Add(bookingGrid);

        roomNamesContainer.Children.Add(frameLayout);
    }

    ListMapFrame.Content = new ScrollView { Content = roomNamesContainer };
}

        
        private void OnPreviousDateClicked(object sender, EventArgs e)
        {
            if (_currentDate > DateTime.Today.AddDays(0))
            {
                _currentDate = _currentDate.AddDays(-1);
                UpdateDateLabel();
                UpdateDatePicker();
                PopulateGroupRooms(); // Refresh rooms for the new date
            }
        }

        private void OnNextDateClicked(object sender, EventArgs e)
        {
            if (_currentDate < DateTime.Today.AddDays(7))
            {
                _currentDate = _currentDate.AddDays(1);
                UpdateDateLabel();
                UpdateDatePicker();
                PopulateGroupRooms(); // Refresh rooms for the new date
            }
        }

        private void OnDateLabelTapped(object sender, EventArgs e)
        {
            DatePicker.Focus();
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            _currentDate = e.NewDate;
            UpdateDateLabel();
            DatePicker.IsVisible = false;
            UpdateDatePicker();
            PopulateGroupRooms(); // Refresh rooms for the new date
        }

        private void UpdateDateLabel()
        {
            string formattedDate =
                _currentDate.ToString("dddd, d MMMM yyyy", new System.Globalization.CultureInfo("sv-SE"));
            formattedDate = char.ToUpper(formattedDate[0]) + formattedDate.Substring(1);
            DateDisplayLabel.Text = formattedDate;
            DateDisplayLabel.FontAttributes = FontAttributes.Bold;
            DateDisplayLabel.TextColor = Color.Black;
        }

        private void UpdateDatePicker()
        {
            DatePicker.Date = _currentDate;
            DatePicker.MinimumDate = DateTime.Today.AddDays(0); // Can't check bookings in the past
            DatePicker.MaximumDate = DateTime.Today.AddDays(7); // Can book 7 days in advance
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
        }
    }
}
