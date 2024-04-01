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
            DateTime startTime1 = DateTime.Now.AddDays(1).AddHours(9); // Example: Booking starts tomorrow at 9 AM
            DateTime endTime1 = DateTime.Now.AddDays(1).AddHours(10); // Example: Booking ends tomorrow at 10 AM

            DateTime startTime2 = DateTime.Now.AddDays(2).AddHours(14); // Example: Booking starts day after tomorrow at 2 PM
            DateTime endTime2 = DateTime.Now.AddDays(2).AddHours(15); // Example: Booking ends day after tomorrow at 3 PM

            // Create pre-made bookings
            await BackEnd.CreateReservation(userID, roomName, startTime1, endTime1);
            await BackEnd.CreateReservation(userID, roomName, startTime2, endTime2);
        }
    }
}