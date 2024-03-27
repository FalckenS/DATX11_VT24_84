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

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

// DateTime.Now  ÄR 1 TIMMA EFTER (går på london tid)!
// EventDateTime GER OCKSÅ EN TIMMA EFTER (går på london tid)!

namespace DATX11_VT24_84
{
    public static class BackEnd
    {
        // Retunerar den RIKTIGA tiden (Sverige tid)
        public static DateTime GetCurrentTime()
        {
            return DateTime.Now.AddHours(1);
        }
        
        public static async Task<bool> IsRoomAvailableNow(string roomName)
        {
            CalendarService calendarService = await GetCalendarService();
            EventsResource.ListRequest request = calendarService.Events.List("datx11.vt24.84@gmail.com");
            
            request.TimeMinDateTimeOffset = DateTime.Now.AddSeconds(-30);
            request.TimeMaxDateTimeOffset = DateTime.Now.AddSeconds(30);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.Q = roomName;
            request.MaxResults = 1;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            
            Events events = await request.ExecuteAsync();
            List<Reservation> reservations = GetReservationList(events);
            
            // Rom reservations är tom pågår inga events för roomName just nu, alltså är rummet ledigt
            return reservations.Count == 0;
        }

        /*
        private static async Task<List<Reservation>> GetAllRoomsAvailableNow()
        {
            CalendarService calendarService = await GetCalendarService();
            EventsResource.ListRequest request = calendarService.Events.List("datx11.vt24.84@gmail.com");
            
            request.TimeMinDateTimeOffset = DateTime.Now.AddSeconds(-30);
            request.TimeMaxDateTimeOffset = DateTime.Now.AddSeconds(30);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 2500;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            
            Events events = await request.ExecuteAsync();
            
            return GetReservationList(events);
        }
        
        private static async Task<List<Reservation>> GetAllRoomsNotAvailableNow()
        {
            CalendarService calendarService = await GetCalendarService();
            EventsResource.ListRequest request = calendarService.Events.List("datx11.vt24.84@gmail.com");
            
            request.TimeMinDateTimeOffset = DateTime.Now.AddSeconds(-30);
            request.TimeMaxDateTimeOffset = DateTime.Now.AddSeconds(30);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 2500;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            
            Events events = await request.ExecuteAsync();
            
            return GetReservationList(events);
        }
        */
        
        private static List<Reservation> GetReservationList(Events events)
        {
            // Lägger till en timma för tiden från APIn går på London tid
            return events.Items.Select(e => new Reservation
            {
                UserID = e.Summary, RoomID = e.Location, StartTime = e.Start.DateTime.Value.AddHours(1), EndTime = e.End.DateTime.Value.AddHours(1)
            }).ToList();
        }
        
        private static async Task<CalendarService> GetCalendarService()
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
        
//  ------------------------------------------------ Room data back end ------------------------------------------------
        
        public static async Task<Room> GetRoomInfo(string roomName)
        {
            List<Room> allRooms = await GetAllRooms();
            Room room = allRooms.First(r => r.Name.Equals(roomName, StringComparison.OrdinalIgnoreCase));
            return room;
        }

        private static async Task<List<string>> GetAllRoomNames(string building)
        {
            List<Room> allRooms = await GetAllRooms();
            return (from room in allRooms where room.Building == building select room.Name).ToList();
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
        public string RoomID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}