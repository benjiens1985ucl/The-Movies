using System;
using System.Text.Json.Serialization;

namespace The_Movies.Models
{
    public class Screening
    {
        private const int CommercialMinutes = 20;
        private const int CleanupMinutes = 20;

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MovieId { get; set; }

        [JsonIgnore]
        public Movie Movie { get; set; } = new Movie();

        public string CinemaName { get; set; } = string.Empty;
        public Hall Hall { get; set; } = new Hall();
        public DateTime DateTime { get; set; }
        public bool IsPremiere { get; set; }
        public int TicketsSold { get; set; }
        public int AvailableSeats => Hall.Capacity - TicketsSold;

        public DateTime GetMovieStartTime()
        {
            return DateTime.AddMinutes(CommercialMinutes);
        }

        public DateTime GetMovieEndTime()
        {
            return GetMovieStartTime().AddMinutes(Movie.Duration);
        }

        public DateTime GetHallAvailableTime()
        {
            return GetMovieEndTime().AddMinutes(CleanupMinutes);
        }
    }
}