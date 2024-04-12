using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Boka : ContentPage
    {
        private DateTime _currentDate;
        private bool _isListaSelected = true;
        private bool _groupRoomsPopulated = false; 



        public Boka()
        {
            InitializeComponent();
            UIUtility.UpdateBackgroundColorMainPages(this);
            _currentDate = DateTime.Today;
            UpdateDateLabel();
            UpdateDatePicker();
            PopulateGroupRooms();
        }
        
        private async void PopulateGroupRooms()
{
    if (_isListaSelected && !_groupRoomsPopulated) 
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

        List<string> roomNames = await BackEnd.GetAllRoomNames();
        StackLayout roomNamesContainer = new StackLayout();

        foreach (string roomName in roomNames)
        {
            // Create green frame
            Frame greenFrame = new Frame
            {
                Padding = new Thickness(0), // Remove padding
                BackgroundColor = Color.FromHex("#27AD72"), // Set background color to green
                HorizontalOptions = LayoutOptions.Start, // Align the frame to the left
                VerticalOptions = LayoutOptions.Fill, // Fill parent's height
                WidthRequest = 80, // Set a fixed width for the frame
                HeightRequest = 28, // Set a fixed height for both frames
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
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(5, 2, 0, 0) 
            };

            Room roomInfo = await BackEnd.GetRoomInfo(roomName);
            string buildingName = roomInfo.Building;
            string floor = roomInfo.Floor;
            string capacity = roomInfo.Capacity;

            Label buildingLabel = new Label
            {
                Text = buildingName,
                FontSize = 12,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(5, 0, 0, 25) // Add margin to position the label
            };

            // Add roomLabel and buildingLabel to a vertical stack layout
            StackLayout labelsLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children = { roomLabel, buildingLabel }
            };

            greenFrame.Content = labelsLayout;

            Frame whiteFrame = new Frame
            {
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand, 
                VerticalOptions = LayoutOptions.Fill, 
                HeightRequest = 28, 
                BorderColor = Color.Black,
                HasShadow = false 
            };

            StackLayout frameLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0
            };
            frameLayout.Children.Add(greenFrame);
            frameLayout.Children.Add(whiteFrame);

            // Add tap gesture recognizer to whiteFrame
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                // Open BokaRum page with the selected room name and date
                await Navigation.PushModalAsync(new BokaRum(roomName, buildingName, floor, _currentDate, capacity, DatePicker.Date)); 
            };
            whiteFrame.GestureRecognizers.Add(tapGestureRecognizer);

            roomNamesContainer.Children.Add(frameLayout);
        }

        ListMapFrame.Content = new ScrollView { Content = roomNamesContainer };
        _groupRoomsPopulated = true; 
    }
}



        private void OnPreviousDateClicked(object sender, EventArgs e)
        {
            if (_currentDate > DateTime.Today.AddDays(0))
            {
                _currentDate = _currentDate.AddDays(-1);
                UpdateDateLabel();
                UpdateDatePicker();
            }
        }

        private void OnNextDateClicked(object sender, EventArgs e)
        {
            if (_currentDate < DateTime.Today.AddDays(7))
            {
                _currentDate = _currentDate.AddDays(1);
                UpdateDateLabel();
                UpdateDatePicker();
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
            DatePicker.MinimumDate = DateTime.Today.AddDays(0); //Kan inte kolla på bokningar bak i tiden
            DatePicker.MaximumDate = DateTime.Today.AddDays(7); // Kan boka 7 dar i framtiden
        }
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
        }
    }
}
