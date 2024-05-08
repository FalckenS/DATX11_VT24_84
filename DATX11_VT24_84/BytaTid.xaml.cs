using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;


namespace DATX11_VT24_84
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BytaTid : ContentPage
    {
        private string _roomName;
        private IRefreshable _page;
        private DateTime _bookingDate;
        private readonly Reservation _booking;

        public BytaTid(Reservation booking, IRefreshable page)
        {
            _booking = booking;
            _page = page;
            
            InitializeComponent();
           
            Task<Room> roomInfo = BackEnd.GetRoomInfo(booking.RoomName);
            _roomName = booking.RoomName;
            _bookingDate = booking.StartTime;
            string building = roomInfo.Result.Building;
            string floor = roomInfo.Result.Floor;
            string capacity = roomInfo.Result.Capacity;
            
            FetchAndDisplayBookings();
            
            RoomNameLabel.Text = _roomName; 
            BuildingLabel.Text = building; 
            FloorLabel.Text = $"Våning  {floor}";
            string dayName = _bookingDate.ToString("dddd", new System.Globalization.CultureInfo("sv-SE"));
            string capitalizedDayName = char.ToUpper(dayName[0]) + dayName.Substring(1);
            string formattedDate = _bookingDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("sv-SE"));
            string displayText = capitalizedDayName + " " + formattedDate;

            CurrentDayLabel.Text = displayText;
            CapacityLabel.Text = $"Kapacitet: {capacity} platser";

            DateTime currentTime = DateTime.Now;
            int currentMinute = currentTime.Minute;
            int nearestFiveMinutes = ((currentMinute + 4) / 5) * 5; 
            int currentHour = currentTime.Hour;

            if (nearestFiveMinutes >= 60)
            {
                currentHour = (currentHour + 1) % 24;
                nearestFiveMinutes = 0;
            }

            StartHourPicker.SelectedIndex = currentHour;
            StartMinutePicker.SelectedIndex = nearestFiveMinutes / 5;

            int endHour = (currentHour + 1) % 24;
            EndHourPicker.SelectedIndex = endHour;
            EndMinutePicker.SelectedIndex = nearestFiveMinutes / 5;

            UpdateEndTime();

        }


        private async void FetchAndDisplayBookings()
        {
            try
            {
                // Fetch reservations for the specified room and date
                List<Reservation> reservations = await BackEnd.GetReservationsForRoom(_roomName, _bookingDate);

                // Filter out past bookings
                reservations = reservations.Where(booking => booking.StartTime > DateTime.Now).ToList();

                // Filter reservations for the specified room
                reservations = reservations.Where(booking => booking.RoomName == _roomName).ToList();

                // Display booking information
                if (reservations.Count == 0)
                {
                    BookingInfoLabel.Text = "Inga bokningar framöver för denna dag";
                }
                else
                {
                    string bookingInfo = "";
                    foreach (var booking in reservations)
                    {
                        string startTime = booking.StartTime.ToString("HH:mm");
                        string endTime = booking.EndTime.ToString("HH:mm");
                        if (booking == _booking) //funkar inte
                        {
                            bookingInfo += $"Aktuell bokning {startTime} - {endTime}\n";
                        }
                        else
                        {
                            bookingInfo += $"Bokat {startTime} - {endTime}\n";
                        }
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
          
        }

      
        private void OnEndMinuteSelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

       
        private async void OnBookRoomClicked(object sender, EventArgs e)
        {
            try
            {
                
                int startHour = int.Parse(StartHourPicker.SelectedItem.ToString());
                int startMinute = int.Parse(StartMinutePicker.SelectedItem.ToString());
                int endHour = int.Parse(EndHourPicker.SelectedItem.ToString());
                int endMinute = int.Parse(EndMinutePicker.SelectedItem.ToString());

                DateTime startTime = _bookingDate.AddHours(startHour).AddMinutes(startMinute);
                DateTime endTime = _bookingDate.AddHours(endHour).AddMinutes(endMinute);


                string roomName = RoomNameLabel.Text;
                // avbokar samt bokar
                await BackEnd.DeleteReservation(_booking.ID);
                await BackEnd.CreateReservation("1", roomName, startTime, endTime); 

                await DisplayAlert("Success", "Booking created successfully", "OK");

                FetchAndDisplayBookings();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            _page.RefreshData();
            await Navigation.PopModalAsync(false);
        } 

    }
}
