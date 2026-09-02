using System;

namespace The_Movies.Models
{
    public class Booking
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ScreeningId { get; set; }
        public int TicketCount { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }
}