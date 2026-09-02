using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using The_Movies.Models;

namespace The_Movies.Services
{
    public class ScreeningRepository
    {
        private readonly string _filePath;

        public ScreeningRepository(string filePath = "screenings.json")
        {
            _filePath = filePath;
        }

        public void Save(List<Screening> screenings)
        {
            string json = JsonSerializer.Serialize(screenings);
            File.WriteAllText(_filePath, json);
        }

        public List<Screening> LoadAll(List<Movie>? movies = null)
        {
            if (!File.Exists(_filePath))
            {
                return new List<Screening>();
            }

            string json = File.ReadAllText(_filePath);
            var screenings = JsonSerializer.Deserialize<List<Screening>>(json) ?? new List<Screening>();

            if (movies != null)
            {
                foreach (var screening in screenings)
                {
                    screening.Movie = movies.FirstOrDefault(m => m.Id == screening.MovieId)
                        ?? new Movie { Title = "Ukendt film" };
                }
            }

            return screenings;
        }
    }
}