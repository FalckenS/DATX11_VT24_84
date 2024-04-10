using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Boka : ContentPage
    {
        private DateTime _currentDate;
        private bool _isListaSelected = true;
        private bool _groupRoomsPopulated = false; // Track whether group rooms have been populated



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
                List<string> roomNames = await BackEnd.GetAllRoomNames();
                StackLayout roomNamesContainer = new StackLayout
                {
                    Spacing = 0 // Set spacing between child elements to 0
                };

                foreach (string roomName in roomNames)
                {
                    Frame frame = new Frame
                    {
                        Padding = new Thickness(10), // Padding around the text inside the frame
                        BackgroundColor = Color.FromHex("#27AD72"), // Set background color to green
                        HorizontalOptions = LayoutOptions.StartAndExpand, // Align the frame to the left
                        VerticalOptions = LayoutOptions.Start, // Align the frame to the top
                        WidthRequest = 55, // Set a fixed width for the frame
                        HeightRequest = 40, // Set a fixed height for the frame
                        BorderColor = Color.Black,
                        HasShadow = false 
                    };

                    Label roomLabel = new Label
                    {
                        Text = roomName,
                        FontSize = 14,
                        TextColor = Color.White,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    };

                    frame.Content = roomLabel;
                    roomNamesContainer.Children.Add(frame);
                }

                ListMapFrame.Content = new ScrollView { Content = roomNamesContainer };
                _groupRoomsPopulated = true; // Set to true after populating
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
            DatePicker.MaximumDate = DateTime.Today.AddDays(7); // Kan boka 14 dar i framtiden
        }
        private void OnListaLabelTapped(object sender, EventArgs e)
        {
            _isListaSelected = true;
            UpdateOverlayPositionLeft();
            PopulateGroupRooms(); // Call the method to populate the group rooms when Lista is selected
        }

        private void OnKartaLabelTapped(object sender, EventArgs e)
        {
            _isListaSelected = false;
            UpdateOverlayPositionRight();
            // Clear the content of ListMapFrame when switching to Karta
            ListMapFrame.Content = null;
            _groupRoomsPopulated = false; // Track whether group rooms have been populated

        }
        private void UpdateOverlayPositionRight()
        {
            OverlayBox.TranslateTo(120, 0, 250);
        }
        private void UpdateOverlayPositionLeft()
        {
            OverlayBox.TranslateTo(0, 0, 250);
        }
        
    }
}
