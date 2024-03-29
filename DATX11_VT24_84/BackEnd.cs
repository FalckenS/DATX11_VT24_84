using System;
using System.Collections.Generic;
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
    public static class BackEnd
    {
        private const string CalendarID = "datx11.vt24.84@gmail.com";
        private const string TimeZone = "Europe/Stockholm";
        
        // FRÅN OSS TILL API: EN TIMMA + vi måste ta bort en innan vi skickar för det ska bli rätt!
        // FRÅN API TILL OSS: EN TIMMA - vi måste lägga till en för det ska bli rätt!
        // DateTime.Now ger 1-timma-minus tid
        
        // Ska ha 1-timma-minus tid som input, använd CreateReservationTime()
        public static async Task<bool> CreateReservation(string userID, string roomName, DateTime startTime, DateTime endTime)
        {
            if (endTime < startTime)
            {
                Console.WriteLine("Invalid start and end time!");
                return false;
            }
            
            List<string> allRoomNames = await GetAllRoomNames();
            if (!allRoomNames.Contains(roomName))
            {
                Console.WriteLine("Invalid room name!");
                return false;
            }
            
            bool isRoomAvailable = await IsRoomAvailable(roomName, startTime, endTime);
            if (!isRoomAvailable)
            {
                Console.WriteLine("Room not available during that time!");
                return false;
            }
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
                Console.Write("Something went wrong with creating reservation!");
                return false;
            }
            return true;
        }
        
        // TODO GÖR EN KLASS RESERVATIONTIME
        // Ska ha vanlig tid som input, retunerar 1-timma-minus tid. Används för att skapa reservations
        public static DateTime CreateReservationTime(int month, int day, int hour, int minute)
        {
            return new DateTime(GetYearOfMonth(month), month, day, hour, minute, 0).AddHours(-1);
        }
        
        public static DateTime CreateReservationTimeForNow()
        {
            return DateTime.Now;
        }
        
        // Retunerar riktiga tiden
        private static DateTime GetRealCurrentTime()
        {
            return DateTime.Now.AddHours(1);
        }

        private static int GetYearOfMonth(int month)
        {
            if (GetRealCurrentTime().Month <= month)
            {
                // Samma år
                return GetRealCurrentTime().Year;
            }
            // Nästa år
            return GetRealCurrentTime().Year + 1;
        }
        
        // ------------------------------------------- Available rooms methods -------------------------------------------

        // Ska ha 1-timma-minus tid som input
        public static async Task<bool> IsRoomAvailable(string roomName, DateTime startTime, DateTime endTime)
        {
            List<Reservation> reservations = await GetReservationsDuring(startTime, endTime);
            return reservations.All(reservation => reservation.RoomName != roomName);
        }
        
        public static async Task<bool> IsRoomAvailableNow(string roomName)
        {
            List<Reservation> ongoingReservations = await GetReservationsDuring(DateTime.Now.AddSeconds(-15), DateTime.Now.AddSeconds(15));
            return ongoingReservations.All(reservation => reservation.RoomName != roomName);
        }

        public static async Task<List<Room>> GetAllRoomsAvailableNow()
        {
            List<Room> allRoomsAvailableNow = new List<Room>();
            List<string> roomNames = await GetAllRoomNamesAvailableNow();
            foreach (string roomName in roomNames)
            {
                Room room = await GetRoomInfo(roomName);
                allRoomsAvailableNow.Add(room);
            }
            return allRoomsAvailableNow;
        }
        
        public static async Task<List<string>> GetAllRoomNamesAvailableNow()
        {
            List<Reservation> ongoingReservations = await GetReservationsDuring(DateTime.Now.AddSeconds(-15), DateTime.Now.AddSeconds(15));
            List<string> roomsNotAvailableNow = ongoingReservations.Select(ongoingReservation => ongoingReservation.RoomName).ToList();
            List<string> allRooms = await GetAllRoomNames();
            // Retunerar alla element i allRooms som INTE finns med i roomsNotAvailableNow
            return allRooms.Where(roomName => !roomsNotAvailableNow.Contains(roomName)).ToList();
        }
     
        // Ska ha 1-timma-minus tid som input
        public static async Task<List<Reservation>> GetReservationsDuring(DateTime startTime, DateTime endTime)
        {
            CalendarService calendarService = GetCalendarService();
            EventsResource.ListRequest request = calendarService.Events.List(CalendarID);
            
            request.TimeMinDateTimeOffset = startTime; // Filtrera bort alla som slutar före startTime
            request.TimeMaxDateTimeOffset = endTime;   // Filtrera bort alla som börjar efter endTime
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 2500;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = await request.ExecuteAsync();
            List<Reservation> reservations = GetReservationList(events);
            
            return reservations;
        }
        
        //  ---------------------------------------- Reservation helper methods ----------------------------------------
        
        private static List<Reservation> GetReservationList(Events events)
        {
            // Lägger till en timma för tiden från APIn är en-timma-minus
            return events.Items.Select(e => new Reservation
            {
                UserID = e.Summary, RoomName = e.Location, StartTime = e.Start.DateTime.Value.AddHours(1), EndTime = e.End.DateTime.Value.AddHours(1)
            }).ToList();
        }
        
        //  ----------------------------------------------- Calender API -----------------------------------------------
        
        private static CalendarService GetCalendarService()
        {
            Assembly assembly = typeof(MainPage).GetTypeInfo().Assembly;
            const string filePath = "DATX11_VT24_84.disco-catcher-418315-d913f85a8669.json";
            Stream stream = assembly.GetManifestResourceStream(filePath);
            if (stream == null)
            {
                throw new FileNotFoundException("Could not find json file.", filePath);
            }
            GoogleCredential credential = GoogleCredential.FromStream(stream).CreateScoped(CalendarService.Scope.Calendar);
            CalendarService calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DATX11_VT24_84",
            });
            return calendarService;
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
                return null;
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
        public string Name { get; set; }
        public string Capacity { get; set; }
        public string Floor { get; set; }
        public string Building { get; set; }
        public List<string> Features { get; set; }
    }

    internal class RoomList
    {
        public List<Room> Rooms { get; set; }
    }

    public class Reservation
    {
        public string UserID { get; set; }
        public string RoomName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}