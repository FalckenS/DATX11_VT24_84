using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PossibleInvalidOperationException
#pragma warning disable CS0168 // Variable is declared but never used
#pragma warning disable CS0618 // Type or member is obsolete

namespace DATX11_VT24_84
{
    /*
    Denna BackEnd fungerar endast rätt när det är sommartid. Eftersom det här systemet endast är en prototyp och kommer
    ersättas av nåt annat system (tex TimeEdit) så bedömer jag att det inte behövs lösas.
    Allt skriven av Samuel Falck.
    */
    
    public static class BackEnd
    {
        /*
        Vintertid:
        
        FRÅN OSS TILL API: En timma + så vi måste TA BORT EN innan vi skickar för det ska bli rätt!
        FRÅN API TILL OSS: En timma - så vi måste LÄGGA TILL EN för det ska bli rätt!
        
        Sommartid:
        
        FRÅN OSS TILL API: Två timmar + så vi måste TA BORT TVÅ innan vi skickar för det ska bli rätt!
        FRÅN API TILL OSS: Två timmar - så vi måste LÄGGA TILL TVÅ för det ska bli rätt!
        */
        private const int HourDifference = 2;
        
        private const string CalendarID = "datx11.vt24.84@gmail.com";
        private const string TimeZone = "Europe/Stockholm";
        
        // -------------------------------------------- Confirm reservation --------------------------------------------

        public static async Task ConfirmReservation(string reservationID)
        {
            CalendarService calendarService = GetCalendarService();
            Event calendarEvent = await calendarService.Events.Get(CalendarID, reservationID).ExecuteAsync();
            // Ändra händelse beskrivning
            calendarEvent.Description = "True";
            await calendarService.Events.Update(calendarEvent, CalendarID, reservationID).ExecuteAsync();
        }
        
        // -------------------------------------------- Delete reservations --------------------------------------------
        
        public static async Task DeleteReservation(string reservationID)
        {
            CalendarService calendarService = GetCalendarService();
            try
            {
                EventsResource.DeleteRequest request = calendarService.Events.Delete(CalendarID, reservationID);
                await request.ExecuteAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Något gick fel med att ta bort bokningen!");
            }
        }
        
        // -------------------------------------------- Create reservations --------------------------------------------
        
        // Ska ha vanlig tid som input
        public static async Task CreateReservationStartsNow(string userID, string roomName, DateTime endTime)
        {
            await CreateReservation(userID, roomName, GetRealTime().AddMinutes(1), endTime, true);
        }
        
        // Ska ha vanlig tid som input
        public static async Task CreateReservation(string userID, string roomName, DateTime startTime, DateTime endTime)
        {
            await CreateReservation(userID, roomName, startTime, endTime, false);
        }
        
        // Ska ha vanlig tid som input, kastar olika exception beroende på vad som går fel
        private static async Task CreateReservation(string userID, string roomName, DateTime startTime, DateTime endTime,
            bool isNow)
        {
            List<string> allRoomNames = await GetAllRoomNames();
            if (!allRoomNames.Contains(roomName))
            {
                throw new Exception("Ogiltigt rumsnamn!");
            }
            
            if (endTime < startTime)
            {
                throw new Exception("Starttiden är efter sluttid!");
            }
            
            if (startTime < GetRealTime())
            {
                throw new Exception("Starttiden är innan nu!");
            }
            
            if ((startTime - DateTime.Now).TotalDays > 7)
            {
                throw new Exception("Kan inte boka grupprum så här långt fram i tiden!");
            }
            
            if ((endTime - startTime).TotalHours > 2)
            {
                throw new Exception("Bokning för lång!");
            }
            
            int numOfUpcomingReservations = (await GetUpcomingReservationsForUser(userID)).Count;
            if (4 < numOfUpcomingReservations)
            {
                throw new Exception("Redan för många bokningar för användaren!");
            }
            
            bool isRoomAvailable = await IsRoomAvailable(roomName, startTime, endTime);
            if (!isRoomAvailable)
            {
                throw new Exception("Rummet är inte tillgänglig under den tiden!");
            }
            
            // Gör tiden rätt för APIn
            startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 
                0).AddHours(-HourDifference);
            endTime =   new DateTime(endTime.Year,   endTime.Month,   endTime.Day,   endTime.Hour,   endTime.Minute,   
                0).AddHours(-HourDifference);

            string confirmed = "False";
            if (isNow)
            {
                // Om bokningen börjar nu, bekräfta bokning
                confirmed = "True";
            }
            try
            {
                Event newReservation = new Event()
                {
                    Summary = userID,
                    Location = roomName,
                    Description = confirmed,
                    Start = new EventDateTime()
                    {
                        DateTime = startTime,
                        TimeZone = TimeZone
                    },
                    End = new EventDateTime()
                    {
                        DateTime = endTime,
                        TimeZone = TimeZone
                    }
                };
                CalendarService calendarService = GetCalendarService();
                EventsResource.InsertRequest request = calendarService.Events.Insert(newReservation, CalendarID);
                await request.ExecuteAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Något gick fel när bokningen skapades!");
            }
        }
        
        // ------------------------------------------ Available rooms methods ------------------------------------------

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static async Task<List<Room>> GetAllRoomsAvailableNow()
        {
            List<Room> roomsAvailableNow = new List<Room>();
            
            List<Reservation> allReservations = await GetAllReservations();
            List<Reservation> ongoingReservations = allReservations.Where(reservation =>
                reservation.StartTime <= GetRealTime() && GetRealTime() < reservation.EndTime && reservation.Confirmed
            ).ToList();
            
            List<string> roomsNotAvailableNow_Names = ongoingReservations.Select(
                ongoingReservation => ongoingReservation.RoomName).ToList();
            List<string> allRoomNames = await GetAllRoomNames();
            
            // Retunerar alla element i allRooms som INTE finns med i roomsNotAvailableNow_Names
            List<string> roomsAvailableNow_Names = allRoomNames.Where(
                roomName => !roomsNotAvailableNow_Names.Contains(roomName)).ToList();
            foreach (string roomName in roomsAvailableNow_Names)
            {
                Room room = await GetRoomInfo(roomName);
                roomsAvailableNow.Add(room);
            }
            return roomsAvailableNow;
        }
        
        public static async Task<bool> IsRoomAvailableNow(string roomName)
        {
            List<Room> allRoomsAvailableNow = await GetAllRoomsAvailableNow();
            return allRoomsAvailableNow.Any(room => room.Name == roomName);
        }
        
        // Ska ha vanlig tid som input
        public static async Task<bool> IsRoomAvailable(string roomName, DateTime startTime, DateTime endTime)
        {
            List<Reservation> allReservations = await GetAllReservations();
            List<Reservation> reservationsDuringTime = allReservations.Where(reservation =>
                reservation.StartTime <= startTime             && startTime           <  reservation.EndTime ||
                reservation.StartTime <  endTime               && endTime             <= reservation.EndTime ||
                startTime             <  reservation.StartTime && reservation.EndTime <  endTime).ToList();
            return reservationsDuringTime.All(reservation => reservation.RoomName != roomName);
        }
        
        //  --------------------------------------------- Get reservations ---------------------------------------------
        
        // Returnera lista med pågående bokningar, sorterad efter start time
        public static async Task<List<Reservation>> GetOngoingReservationsForUser(string userID)
        {
            List<Reservation> allReservations = await GetAllReservations();
            return allReservations.Where(reservation =>
                reservation.StartTime <= GetRealTime() && GetRealTime() < reservation.EndTime &&
                reservation.UserID == userID).ToList();
        }
        
        // Returnera lista med bokningar 7 dagar framåt, sorterad efter start time
        public static async Task<List<Reservation>> GetUpcomingReservationsForUser(string userID)
        {
            List<Reservation> allReservations = await GetAllReservations();
            return allReservations.Where(reservation =>
                GetRealTime() < reservation.StartTime && reservation.EndTime <= GetRealTime().AddDays(7) &&
                reservation.UserID == userID).ToList();
        }
        
        // Returnera lista med bokningar för ett rum under en dag
        public static async Task<List<Reservation>> GetReservationsForRoom(string roomName, DateTime date)
        {
            List<Reservation> allReservations = await GetAllReservations();
            return allReservations.Where(reservation =>
                reservation.RoomName == roomName &&
                (reservation.StartTime.Date == date.Date) || (reservation.EndTime.Date == date.Date)).ToList();
        }
        
        // Returnera lista med vanlig tid sorterad efter start time
        private static async Task<List<Reservation>> GetAllReservations()
        {
            CalendarService calendarService = GetCalendarService();
            EventsResource.ListRequest request = calendarService.Events.List(CalendarID);
            
            request.TimeMinDateTimeOffset = DateTime.Now.AddDays(-1); // Filtrera bort alla som slutar före startTime
            request.TimeMaxDateTimeOffset = DateTime.Now.AddDays(8);  // Filtrera bort alla som börjar efter endTime
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 2500;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
    
            Events events = await request.ExecuteAsync();
            return events.Items.Select(e => new Reservation(
                e.Summary,
                e.Location,
                e.Start.DateTime.Value.AddHours(HourDifference),
                e.End.DateTime.Value.AddHours(HourDifference),
                e.Id,
                bool.Parse(e.Description))).ToList();
        }
        
        //  ------------------------------------------------- Utility -------------------------------------------------
        
        private static CalendarService GetCalendarService()
        {
            Assembly assembly = typeof(MainPage).GetTypeInfo().Assembly;
            const string filePath = "DATX11_VT24_84.disco-catcher-418315-d913f85a8669.json";
            Stream stream = assembly.GetManifestResourceStream(filePath);
            if (stream == null)
            {
                throw new FileNotFoundException("Could not find json file.", filePath);
            }
            GoogleCredential credential = GoogleCredential.FromStream(stream).CreateScoped(
                CalendarService.Scope.Calendar);
            CalendarService calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DATX11_VT24_84",
            });
            return calendarService;
        }

        private static DateTime GetRealTime()
        {
            return DateTime.Now.AddHours(HourDifference);
        }
        
        //  -------------------------------------------- Room data back end --------------------------------------------
        
        public static async Task<Room> GetRoomInfo(string roomName)
        {
            List<Room> allRooms = await GetAllRooms();
            try
            {
                return allRooms.First(r => r.Name.Equals(roomName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception e)
            {
                throw new Exception("Ogiltigt rumsnamn!");
            }
        }

        private static async Task<List<string>> GetAllRoomNames()
        {
            List<Room> allRooms = await GetAllRooms();
            return (from room in allRooms select room.Name).ToList();
        }
        
        private static async Task<List<Room>> GetAllRooms()
        {
            const string filePath = "DATX11_VT24_84.rooms.json";
            Assembly assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(filePath);
            if (stream == null)
            {
                throw new FileNotFoundException("Could not find json file.", filePath);
            }
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = await reader.ReadToEndAsync();
                RoomList roomList = JsonConvert.DeserializeObject<RoomList>(json);
                return roomList.Rooms;
            }
        }
    }
    
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Room
    {
        public Room(string name, string capacity, string floor, string building, List<string> features)
        {
            Name = name;
            Capacity = capacity;
            Floor = floor;
            Building = building;
            Features = features;
        }
        public string Name { get; }
        public string Capacity { get; }
        public string Floor { get; }
        public string Building { get; }
        public List<string> Features { get; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Reservation
    {
        public Reservation(string userID, string roomName, DateTime startTime, DateTime endTime, string id, bool confirmed)
        {
            UserID = userID;
            RoomName = roomName;
            StartTime = startTime;
            EndTime = endTime;
            ID = id;
            Confirmed = confirmed;
        }
        public string UserID { get; }
        public string RoomName { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public string ID { get; }
        public bool Confirmed { get; set; }
    }
    
    internal class RoomList
    {
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public List<Room> Rooms { get; set; }
    }
}