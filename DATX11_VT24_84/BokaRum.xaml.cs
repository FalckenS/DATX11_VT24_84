using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BokaRum : ContentPage
    {
        
        public BokaRum(string roomName, string building, string floor)
        {
            InitializeComponent();
            UIUtility.UpdateBackgroundColorOtherPages(this);
            RoomNameLabel.Text = roomName; // Set the room name label with the passed parameter
            BuildingLabel.Text = building; // Set the building information
            FloorLabel.Text = $"Våning  {floor}";

            // Set default start time to the next quarter-hour increment
            DateTime currentTime = DateTime.Now;
            int currentMinute = currentTime.Minute;
            int nearestQuarter = (currentMinute / 15 + 1) * 15;
            int currentHour = currentTime.Hour;
            if (nearestQuarter >= 60)
            {
                currentHour = (currentHour + 1) % 24;
                nearestQuarter = 0;
            }

            StartHourPicker.SelectedIndex = currentHour;
            StartMinutePicker.SelectedIndex = nearestQuarter / 15;

            // Set default end time to the next hour from the start time
            int endHour = (currentHour + 1) % 24;
            EndHourPicker.SelectedIndex = endHour;
            EndMinutePicker.SelectedIndex = nearestQuarter / 15;

            // Update end time based on selected start time
            UpdateEndTime();
        }


        // Event handler for start hour picker
        private void OnStartHourSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEndTime();
        }

        // Event handler for start minute picker
        private void OnStartMinuteSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEndTime();
        }

        // Method to update the end time based on the selected start time
        private void UpdateEndTime()
        {
            int startHour = int.Parse(StartHourPicker.SelectedItem.ToString());
            int endHour = startHour + 1;

            if (endHour >= 24)
            {
                endHour = 0;
            }

            EndHourPicker.SelectedIndex = endHour;
            EndMinutePicker.SelectedIndex = StartMinutePicker.SelectedIndex;
        }

        // Event handler for end hour picker
        private void OnEndHourSelectedIndexChanged(object sender, EventArgs e)
        {
            // You can add additional logic here if needed
        }

        // Event handler for end minute picker
        private void OnEndMinuteSelectedIndexChanged(object sender, EventArgs e)
        {
            // You can add additional logic here if needed
        }

        // Event handler for booking button click
        private async void OnBookRoomClicked(object sender, EventArgs e)
        {
            try
            {
                // Get the selected start and end times from the pickers
                int startHour = int.Parse(StartHourPicker.SelectedItem.ToString());
                int startMinute = int.Parse(StartMinutePicker.SelectedItem.ToString());
                int endHour = int.Parse(EndHourPicker.SelectedItem.ToString());
                int endMinute = int.Parse(EndMinutePicker.SelectedItem.ToString());

                // Calculate the start and end DateTime objects
                DateTime startTime = DateTime.Today.AddHours(startHour).AddMinutes(startMinute);
                DateTime endTime = DateTime.Today.AddHours(endHour).AddMinutes(endMinute);

                // Get the room name from somewhere (you need to implement this logic)
                string roomName = RoomNameLabel.Text; // Replace "Room Name" with the actual room name

                // Create the reservation with the calculated start time
                await BackEnd.CreateReservation("datx11.vt24.84@gmail.com", roomName, startTime, endTime); 

                // Display a success message (you can implement this as needed)
                await DisplayAlert("Success", "Booking created successfully", "OK");
            }
            catch (Exception ex)
            {
                // Display an error message if something goes wrong
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }


    }
}
