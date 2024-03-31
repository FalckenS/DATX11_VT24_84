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
            RoomNameLabel.Text = roomName;
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
        private void OnBookRoomClicked(object sender, EventArgs e)
        {
            // Handle the booking button click event
        }
    }
}
