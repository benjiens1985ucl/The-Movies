using System;

namespace The_Movies.Models
{
    public class Screening
    {
        private const int CommercialMinutes = 20;
        private const int CleanupMinutes = 20; 
        
        public Movie Movie { get; set; } = new Movie();
        public Hall Hall { get; set; } = new Hall();
        public DateTime DateTime { get; set; }
        public bool IsPremiere { get; set; }

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