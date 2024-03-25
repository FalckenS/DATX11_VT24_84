using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace DATX11_VT24_84
{
    public class BackEnd
    {
        private const string RoomsFilePath = "DATX11_VT24_84.rooms.json";

        //private readonly List<Reservation> _reservations;

        public BackEnd()
        {
            //_reservations = new List<Reservation>();
            Init();
        }
        
        private void Init()
        {
            // TODO BackEnd INIT
        }
        
//  -------------------------------------------------- Singleton code --------------------------------------------------
        private static BackEnd _instance;

        // Lock object for thread safety in multi-threaded applications
        private static readonly object LockObject = new object();
        
        // Public static method to get the instance of the class
        public static BackEnd Instance
        {
            get
            {
                // Double-check locking for thread safety
                if (_instance == null)
                {
                    lock (LockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new BackEnd();
                        }
                    }
                }
                return _instance;
            }
        }
        
//  ------------------------------------------------ Room data back end ------------------------------------------------
        
        public async Task<Room> GetRoomInfo(string roomName)
        {
            List<Room> allRooms = await ReadRoomsFromFile();
            Room room = allRooms.FirstOrDefault(r => r.Name.Equals(roomName, StringComparison.OrdinalIgnoreCase));
            return room;
        }
        
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private async Task<List<Room>> ReadRoomsFromFile()
        {
            Assembly assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(RoomsFilePath);
            if (stream == null)
            {
                throw new FileNotFoundException("Could not find json file.", RoomsFilePath);
            }
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = await reader.ReadToEndAsync();
                RoomList roomList = JsonConvert.DeserializeObject<RoomList>(json);
                return roomList.Rooms;
            }
        }
        
// ----------------------------------------------- Reservations methods -----------------------------------------------
        
        /*public async Task<bool> IsRoomAvailable(string roomId, DateTime startTime, DateTime endTime)
        {
            if (startTime < endTime)
            {
                throw new ArgumentException("Invalid start and end time!");
            }
            List<Reservation> reservationsForRoom = _reservations.Where(reservation => reservation.RoomID == roomId).ToList();
            return !reservationsForRoom.Any(reservation =>
                (reservation.StartTime <= startTime && startTime < reservation.EndTime) ||
                (reservation.StartTime < endTime    && endTime <= reservation.EndTime)  ||
                (startTime <= reservation.StartTime && reservation.EndTime <= endTime));
        }
        
        public async Task<bool> IsRoomAvailableNow(string roomId)
        {
            List<Reservation> reservationsForRoom = _reservations.Where(reservation => reservation.RoomID == roomId).ToList();
            return !reservationsForRoom.Any(reservation => 
                reservation.StartTime <= DateTime.Now && DateTime.Now < reservation.EndTime);
        }

        // Retunrs all current and coming reservations for a user
        public async Task<List<Reservation>> GetCurrentAndComingReservations(string userID)
        {
            List<Reservation> currentAndComingReservation = new List<Reservation>();
            
            // Filter and sort reservations
            List<Reservation> reservationsForUser = _reservations.Where(reservation => reservation.UserID == userID).ToList();
            reservationsForUser = reservationsForUser.OrderBy(reservation => reservation.StartTime).ToList();
            
            foreach (Reservation reservation in reservationsForUser)
            {
                if (reservation.EndTime < DateTime.Now)
                {
                    // Reservation has ended
                    continue;
                }
                currentAndComingReservation.Add(reservation);
            }
            return currentAndComingReservation;
        }

        public async void AddNewReservation(string userID, string roomId, DateTime startTime, DateTime endTime)
        {
            if (startTime < endTime)
            {
                throw new ArgumentException("Invalid start and end time!");
            }
            if (!await IsRoomAvailable(roomId, startTime, endTime))
            {
                throw new ArgumentException("Room is not available during that time!");
            }
            _reservations.Add(new Reservation
            {
                UserID = userID,
                RoomID = roomId,
                StartTime = startTime,
                EndTime = endTime
            });
        }*/
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

    /*public class Reservation
    {
        public string UserID { get; set; }
        public string RoomID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }*/
}