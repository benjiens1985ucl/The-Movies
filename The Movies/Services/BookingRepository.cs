using System;
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

        public int GetTicketsSold(Guid screeningId)
        {
            var bookings = LoadAll();
            return bookings
                .Where(b => b.ScreeningId == screeningId)
                .Sum(b => b.TicketCount);
        }

        public void Update(Booking updatedBooking)
        {
            var bookings = LoadAll();
            var existing = bookings.FirstOrDefault(b => b.Id == updatedBooking.Id);

            if (existing != null)
            {
                existing.TicketCount = updatedBooking.TicketCount;
                existing.CustomerName = updatedBooking.CustomerName;
            }

            Save(bookings);
        }

        public void Delete(Guid bookingId)
        {
            var bookings = LoadAll();
            bookings.RemoveAll(b => b.Id == bookingId);
            Save(bookings);
        }
    }
}