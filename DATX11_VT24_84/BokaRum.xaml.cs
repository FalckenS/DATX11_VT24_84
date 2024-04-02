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
            BuildingLabel.Text = building; 
            FloorLabel.Text = $"Våning  {floor}";

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

            int endHour = (currentHour + 1) % 24;
            EndHourPicker.SelectedIndex = endHour;
            EndMinutePicker.SelectedIndex = nearestQuarter / 15;

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
                int startHour = int.Parse(StartHourPicker.SelectedItem.ToString());
                int startMinute = int.Parse(StartMinutePicker.SelectedItem.ToString());
                int endHour = int.Parse(EndHourPicker.SelectedItem.ToString());
                int endMinute = int.Parse(EndMinutePicker.SelectedItem.ToString());

                DateTime startTime = DateTime.Today.AddHours(startHour).AddMinutes(startMinute);
                DateTime endTime = DateTime.Today.AddHours(endHour).AddMinutes(endMinute);

                string roomName = RoomNameLabel.Text; 

                await BackEnd.CreateReservation("datx11.vt24.84@gmail.com", roomName, startTime, endTime); 

                await DisplayAlert("Success", "Booking created successfully", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }


    }
}
