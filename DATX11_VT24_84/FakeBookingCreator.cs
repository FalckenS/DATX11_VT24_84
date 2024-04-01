using System;
using System.Threading.Tasks;

namespace DATX11_VT24_84
{
    public static class FakeBookingCreator
    {
        public static async Task CreatePreMadeBookings()
        {
            // Replace these values with your desired booking details
            string userID = "userID";
            string roomName = "M0167A";
            string roomName2 = "M0167B";
            DateTime startTime1 = new DateTime(2024, 4, 2, 9, 0, 0); // Booking starts on April 2, 2024, at 9 AM
            DateTime endTime1 = new DateTime(2024, 4, 2, 10, 0, 0);   // Booking ends on April 2, 2024, at 10 AM
            
            DateTime startTime2 = new DateTime(2024, 4, 3, 14, 0, 0); // Booking starts on April 3, 2024, at 2 PM
            DateTime endTime2 = new DateTime(2024, 4, 3, 15, 0, 0);   // Booking ends on April 3, 2024, at 3 PM
           
            DateTime startTime3 = new DateTime(2024, 4, 4, 14, 0, 0); // Booking starts on April 4, 2024, at 2 PM
            DateTime endTime3 = new DateTime(2024, 4, 4, 15, 0, 0);   // Booking ends on April 4, 2024, at 3 PM

            // Create pre-made bookings
            await BackEnd.CreateReservation(userID, roomName, startTime1, endTime1);
            await BackEnd.CreateReservation(userID, roomName2, startTime2, endTime2);
            await BackEnd.CreateReservation(userID, roomName, startTime3, endTime3);
        }
    }
}