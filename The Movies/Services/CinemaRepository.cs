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
                CreateCinema("Hjerm", 120, 90, 60),
                CreateCinema("Videbæk", 180, 140, 110, 80, 60),
                CreateCinema("Thorsminde", 100, 70),
                CreateCinema("Ræhr", 150, 110, 80, 60)
            };
        }

        private Cinema CreateCinema(string name, params int[] capacities)
        {
            var cinema = new Cinema 
            { 
                Name = name 
            };

            for (int i = 0; i < capacities.Length; i++)
            {
                cinema.Halls.Add(new Hall
                {
                    Number = i + 1,
                    Capacity = capacities[i]
                });
            }

            return cinema;
        }
    }
}