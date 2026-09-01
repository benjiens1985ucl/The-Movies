using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using The_Movies.Models;
using The_Movies.Services;

namespace TheMovies.Tests
{
    [TestClass]
    public class CinemaRepositoryTests
    {
        [TestMethod]
        public void SaveAndLoadAll_ReturnsTheSameCinemas()
        {
            string testFile = $"test_cinemas_{Guid.NewGuid()}.json";
            var repository = new CinemaRepository(testFile);

            var cinema = new Cinema { Name = "Test Biograf" };
            cinema.Halls.Add(new Hall { Number = 1 });
            cinema.Halls.Add(new Hall { Number = 2 });

            repository.Save(new List<Cinema> { cinema });
            var loaded = repository.LoadAll();

            Assert.AreEqual(1, loaded.Count);
            Assert.AreEqual("Test Biograf", loaded[0].Name);
            Assert.AreEqual(2, loaded[0].Halls.Count);
        }

        [TestMethod]
        public void LoadAll_SeedsDefaultCinemas_WhenFileDoesNotExist()
        {
            string testFile = $"test_cinemas_{Guid.NewGuid()}.json";
            var repository = new CinemaRepository(testFile);

            var loaded = repository.LoadAll();

            Assert.AreEqual(4, loaded.Count);
        }
    }
}