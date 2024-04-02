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

// ReSharper disable PossibleInvalidOperationException
#pragma warning disable CS0618 // Type or member is obsolete

namespace DATX11_VT24_84
{
    /*
    Kan vara så att skiten slutar fungera när det blir sommartid, löser det då isåfall
    Vetefan om TimeZone faktiskt gör något, verkar som den är broken, verkar alltid köra på London tid
    Inte den snyggaste koden, bryter mot lite principer här och där men den funkar så fuck it we ball
    Allt skriven av Samuel Falck
    */
    
    public static class BackEnd
    {
        // FRÅN OSS TILL API: En timma + så vi måste TA BORT EN innan vi skickar för det ska bli rätt!
        // FRÅN API TILL OSS: En timma - så vi måste LÄGGA TILL EN för det ska bli rätt!
        // DateTime.Now ger 1-timma-minus tid, GetRealTime() ger vanlig tid
        
        private const string CalendarID = "datx11.vt24.84@gmail.com";
        private const string TimeZone = "Europe/Stockholm";
        
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
                throw new Exception("Something went wrong with deleting reservation!");
            }
        }
        
        // -------------------------------------------- Create reservations --------------------------------------------
        
        // Ska ha vanlig tid som input
        public static async Task CreateReservationStartsNow(string userID, string roomName, DateTime endTime)
        {
            await CreateReservation(userID, roomName, GetRealTime().AddMinutes(1), endTime);
        }
        
        // Ska ha vanlig tid som input, kastar olika exception beroende på vad som går fel
        public static async Task CreateReservation(string userID, string roomName, DateTime startTime, DateTime endTime)
        {
            if (endTime < startTime)
            {
                throw new Exception("Start time is after end time!");
            }
            
            if (startTime < GetRealTime())
            {
                throw new Exception("Start time before now!");
            }
            
            if ((startTime - DateTime.Now).TotalDays > 7)
            {
                throw new Exception("Cant reserve group room this far in the future!");
            }
            
            if ((endTime - startTime).TotalHours > 2)
            {
                throw new Exception("Reservation to long!");
            }
            
            if (4 < (await GetReservationsForUser(userID)).Count)
            {
                throw new Exception("Already to many reservations for user!");
            }
            
            List<string> allRoomNames = await GetAllRoomNames();
            if (!allRoomNames.Contains(roomName))
            {
                throw new Exception("Invalid room name!");
            }
            
            bool isRoomAvailable = await IsRoomAvailable(roomName, startTime, endTime);
            if (!isRoomAvailable)
            {
                throw new Exception("Room not available during that time!");
            }
            
            // Gör vanlig tid till 1-timma-minus tid
            startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 
                0).AddHours(-1);
            endTime =   new DateTime(endTime.Year,   endTime.Month,   endTime.Day,   endTime.Hour,   endTime.Minute,   
                0).AddHours(-1);
            
            try
            {
                Event newReservation = new Event()
                {
                    Summary = userID,
                    Location = roomName,
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
                throw new Exception("Something went wrong with creating reservation!");
            }
        }
        
        // ------------------------------------------ Available rooms methods ------------------------------------------

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
        
        public static async Task<bool> IsRoomAvailableNow(string roomName)
        {
            return await IsRoomAvailable(roomName, 
                GetRealTime().AddSeconds(-15), 
                GetRealTime().AddSeconds(15));
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static async Task<List<Room>> GetAllRoomsAvailableNow()
        {
            List<Room> roomsAvailableNow = new List<Room>();
            
            List<Reservation> allReservations = await GetAllReservations();
            List<Reservation> ongoingReservations = allReservations.Where(reservation =>
                reservation.StartTime    <= GetRealTime() && GetRealTime() <  reservation.EndTime ||
                reservation.StartTime    <  GetRealTime() && GetRealTime() <= reservation.EndTime ||
                GetRealTime() <  reservation.StartTime    && reservation.EndTime      <  GetRealTime()
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
        
        //  --------------------------------------------- Get reservations ---------------------------------------------
        
        // Returnera lista för bokningar pågående och 7 dagar framåt, sorterad efter start time
        public static async Task<List<Reservation>> GetReservationsForUser(string userID)
        {
            List<Reservation> allReservations = await GetAllReservations();
            List<Reservation> ongoingAndUpcomingReservations = allReservations.Where(reservation =>
                (reservation.StartTime <= GetRealTime() && GetRealTime() < reservation.EndTime) ||
                (GetRealTime() < reservation.StartTime && reservation.EndTime <= GetRealTime().AddDays(7))).ToList();
            return ongoingAndUpcomingReservations.Where(reservation => reservation.UserID == userID).ToList();
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
            // Lägger till en timma för tiden från APIn är en-timma-minus
            return events.Items.Select(e => new Reservation(
                e.Summary,
                e.Location,
                e.Start.DateTime.Value.AddHours(1),
                e.End.DateTime.Value.AddHours(1),
                e.Id)).ToList();
        }
        
        public static async Task<List<Reservation>> GetReservationsForRoom(string roomName, DateTime bookingDate)
        {
            try
            {
                List<Reservation> allReservations = await GetAllReservations();

                List<Reservation> reservationsForRoom = allReservations.Where(reservation =>
                    reservation.RoomName == roomName &&
                    reservation.StartTime.Date == bookingDate.Date).ToList();

                return reservationsForRoom;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching reservations for room {roomName}: {ex.Message}");
                throw; 
            }
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
            return DateTime.Now.AddHours(1);
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
                throw new Exception("Invalid room name!");
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

    internal class RoomList
    {
        public List<Room> Rooms { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Reservation
    {
        public Reservation(string userID, string roomName, DateTime startTime, DateTime endTime, string id)
        {
            UserID = userID;
            RoomName = roomName;
            StartTime = startTime;
            EndTime = endTime;
            ID = id;
        }
        public string UserID { get; }
        public string RoomName { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public string ID { get; }
    }
}