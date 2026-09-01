using System.Collections.Generic;
using System.IO;
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

        public List<Screening> LoadAll()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Screening>();
            }

            string json = File.ReadAllText(_filePath);
            var screenings = JsonSerializer.Deserialize<List<Screening>>(json);

            return screenings ?? new List<Screening>();
        }
    }
}