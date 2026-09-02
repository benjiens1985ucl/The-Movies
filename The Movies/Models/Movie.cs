using System;

namespace The_Movies.Models
{
    public class Movie
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public Genre Genre { get; set; }
    }
}