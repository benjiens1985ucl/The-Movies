using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using The_Movies.Models;

namespace The_Movies.Services
{
    public class MovieRepository
    {
        private readonly string _filePath;

        public MovieRepository(string filePath = "movies.json")
        {
            _filePath = filePath;
        }

        public void Save(List<Movie> movies)
        {
            string json = JsonSerializer.Serialize(movies);
            File.WriteAllText(_filePath, json);
        }

        public List<Movie> LoadAll()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Movie>();
            }

            string json = File.ReadAllText(_filePath);
            var movies = JsonSerializer.Deserialize<List<Movie>>(json);

            return movies ?? new List<Movie>();
        }
    }
}