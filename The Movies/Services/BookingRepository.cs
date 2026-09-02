using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using The_Movies.Models;

namespace The_Movies.Services
{
    public class BookingRepository
    {
        private readonly string _filePath;

        public BookingRepository(string filePath = "bookings.json")
        {
            _filePath = filePath;
        }

        public void Save(List<Booking> bookings)
        {
            string json = JsonSerializer.Serialize(bookings);
            File.WriteAllText(_filePath, json);
        }

        public List<Booking> LoadAll()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Booking>();
            }

            string json = File.ReadAllText(_filePath);
            var bookings = JsonSerializer.Deserialize<List<Booking>>(json);

            return bookings ?? new List<Booking>();
        }

        public int GetTicketsSold(System.Guid screeningId)
        {
            var bookings = LoadAll();
            return bookings
                .Where(b => b.ScreeningId == screeningId)
                .Sum(b => b.TicketCount);
        }
    }
}