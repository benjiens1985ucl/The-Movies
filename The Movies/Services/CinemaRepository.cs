using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using The_Movies.Models;

namespace The_Movies.Services
{
    public class CinemaRepository
    {
        private readonly string _filePath;

        public CinemaRepository(string filePath = "cinemas.json")
        {
            _filePath = filePath;
        }

        public void Save(List<Cinema> cinemas)
        {
            string json = JsonSerializer.Serialize(cinemas);
            File.WriteAllText(_filePath, json);
        }

        public List<Cinema> LoadAll()
        {
            if (!File.Exists(_filePath))
            {
                var seeded = CreateDefaultCinemas();
                Save(seeded);
                return seeded;
            }

            string json = File.ReadAllText(_filePath);
            var cinemas = JsonSerializer.Deserialize<List<Cinema>>(json);

            return cinemas ?? new List<Cinema>();
        }

        private List<Cinema> CreateDefaultCinemas()
        {
            return new List<Cinema>
            {
                CreateCinema("Hjerm", 3),
                CreateCinema("Videbæk", 5),
                CreateCinema("Thorsminde", 2),
                CreateCinema("Ræhr", 4)
            };
        }

        private Cinema CreateCinema(string name, int hallCount)
        {
            var cinema = new Cinema { Name = name };

            for (int i = 1; i <= hallCount; i++)
            {
                cinema.Halls.Add(new Hall { Number = i });
            }

            return cinema;
        }
    }
}