using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;


namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BokaRum : ContentPage
    {
        private string _roomName;
        private DateTime _bookingDate;
        public BokaRum(string roomName, string building, string floor, DateTime bookingDate)
        {
            InitializeComponent();
            _roomName = roomName;
            _bookingDate = bookingDate;
            FetchAndDisplayBookings();

            UIUtility.UpdateBackgroundColorOtherPages(this);
            RoomNameLabel.Text = roomName; 
            BuildingLabel.Text = building; 
            FloorLabel.Text = $"Våning  {floor}";
            string dayName = bookingDate.ToString("dddd", new System.Globalization.CultureInfo("sv-SE"));
            string capitalizedDayName = char.ToUpper(dayName[0]) + dayName.Substring(1);
            string formattedDate = bookingDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("sv-SE"));
            string displayText = capitalizedDayName + " " + formattedDate;

            CurrentDayLabel.Text = displayText;

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

        private async void FetchAndDisplayBookings()
        {
            try
            {
                // Fetch reservations for the specified room and date
                List<Reservation> reservations = await BackEnd.GetReservationsForRoom(_roomName, _bookingDate);

                // Display booking information
                if (reservations.Count == 0)
                {
                    BookingInfoLabel.Text = "No bookings for the room today";
                }
                else
                {
                    string bookingInfo = "";
                    foreach (var booking in reservations)
                    {
                        string startTime = booking.StartTime.ToString("HH:mm");
                        string endTime = booking.EndTime.ToString("HH:mm");
                        bookingInfo += $"Booked {startTime} - {endTime}\n";
                    }
                    BookingInfoLabel.Text = bookingInfo;
                }
            }
            catch (Exception ex)
            {
                // Handle any errors gracefully
                Console.WriteLine($"Error fetching reservations: {ex.Message}");
                BookingInfoLabel.Text = "Error fetching reservations";
            }
        }
        
        private async Task<List<Reservation>> FetchReservationsForRoom(string roomName, DateTime bookingDate)
        {
            // Placeholder implementation to simulate reservations
            List<Reservation> reservations = new List<Reservation>();

            // Add sample reservations (replace with actual logic to fetch reservations)
            reservations.Add(new Reservation("userID1", roomName, DateTime.Today.AddHours(9), DateTime.Today.AddHours(11), "1"));
            reservations.Add(new Reservation("userID2", roomName, DateTime.Today.AddHours(14), DateTime.Today.AddHours(15), "2"));

            return reservations;
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
