using System;

namespace The_Movies.Models
{
    public class Screening
    {
        public Movie Movie { get; set; } = new Movie();
        public Hall Hall { get; set; } = new Hall();
        public DateTime DateTime { get; set; }
        public bool IsPremiere { get; set; }
    }
}