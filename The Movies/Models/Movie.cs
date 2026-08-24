namespace The_Movies.Models
{
    public class Movie
    {
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public Genre Genre { get; set; }
    }
}