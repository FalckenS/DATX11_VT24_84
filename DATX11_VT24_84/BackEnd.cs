using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DATX11_VT24_84
{
    public static class BackEnd
    {
        public static async Task<Room> GetRoomByName(string roomName)
        {
            List<Room> allRooms = await ReadRoomDataAsync();
            Room room = allRooms.FirstOrDefault(r => r.Name.Equals(roomName, StringComparison.OrdinalIgnoreCase));
            return room;
        }
        
        private static async Task<List<Room>> ReadRoomDataAsync()
        {
            Assembly assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("DATX11_VT24_84.rooms.json");
            if (stream == null)
            {
                throw new FileNotFoundException("Could not find json file.", "DATX11_VT24_84.rooms.json");
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
    
    public class RoomList
    {
        public List<Room> Rooms { get; set; }
    }
}