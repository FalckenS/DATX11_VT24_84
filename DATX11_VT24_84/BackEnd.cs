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

#pragma warning disable CS0618 // Type or member is obsolete

// DateTime.Now ÄR 1 TIMMA EFTER (London tid)!
// EventDateTime GER OCKSÅ EN TIMMA EFTER (London tid)!

namespace DATX11_VT24_84
{
    public static class BackEnd
    {
        public static async Task<bool> IsRoomAvailableNow(string roomName)
        {
            List<Reservation> allOngoingReservations = await GetAllOngoingReservations();
            return allOngoingReservations.All(reservation => reservation.RoomName != roomName);
        }

        public static async Task<List<Room>> GetAllRoomsAvailableNow()
        {
            List<Room> allRoomsAvailableNow = new List<Room>();
            List<string> roomNames = await GetAllRoomNamesAvailableNow();
            foreach (string roomName in roomNames)
            {
                allRoomsAvailableNow.Add(await GetRoomInfo(roomName));
            }
            return allRoomsAvailableNow;
        }
        
        private static async Task<List<string>> GetAllRoomNamesAvailableNow()
        {
            List<string> notAvailableNow = await GetAllRoomNamesNotAvailableNow();
            List<string> allRoomNames = await GetAllRoomNames();
            return allRoomNames.Where(roomName => !notAvailableNow.Contains(roomName)).ToList();
        }
        
        private static async Task<List<string>> GetAllRoomNamesNotAvailableNow()
        {
            List<Room> roomsNotAvailableNow = await GetAllRoomsNotAvailableNow();
            return roomsNotAvailableNow.Select(room => room.Name).ToList();
        }
        
        private static async Task<List<Room>> GetAllRoomsNotAvailableNow()
        {
            List<Room> allRoomsNotAvailableNow = new List<Room>();
            
            List<Reservation> allOngoingReservations = await GetAllOngoingReservations();
            foreach (Reservation ongoingReservation in allOngoingReservations)
            {
                allRoomsNotAvailableNow.Add(await GetRoomInfo(ongoingReservation.RoomName));
            }
            return allRoomsNotAvailableNow;
        }
        
        //  --------------------------------------------- Helper methods ---------------------------------------------
        
        private static List<Reservation> GetReservationList(Events events)
        {
            // Lägger till en timma för tiden från APIn går på London tid
            return events.Items.Select(e => new Reservation
            {
                UserID = e.Summary, RoomName = e.Location, StartTime = e.Start.DateTime.Value.AddHours(1), EndTime = e.End.DateTime.Value.AddHours(1)
            }).ToList();
        }
        
        // Retunerar Sverige tid
        private static DateTime GetCurrentTime()
        {
            return DateTime.Now.AddHours(1);
        }
        
        //  ------------------------------------------- Calender API methods -------------------------------------------
        
        private static async Task<List<Reservation>> GetAllOngoingReservations()
        {
            CalendarService calendarService = GetCalendarService();
            EventsResource.ListRequest request = calendarService.Events.List("datx11.vt24.84@gmail.com");
            
            request.TimeMinDateTimeOffset = DateTime.Now.AddSeconds(-30);
            request.TimeMaxDateTimeOffset = DateTime.Now.AddSeconds(30);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 2500;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            
            List<Reservation> reservations = GetReservationList(await request.ExecuteAsync());
            
            // Rom reservations är tom pågår inga events för roomName just nu, alltså är rummet ledigt
            return reservations;
        }
        
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