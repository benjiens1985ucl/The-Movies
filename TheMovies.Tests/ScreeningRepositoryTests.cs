using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using The_Movies.Models;
using The_Movies.Services;

namespace TheMovies.Tests
{
    [TestClass]
    public class ScreeningRepositoryTests
    {
        [TestMethod]
        public void SaveAndLoadAll_ReturnsTheSameScreenings()
        {
            string testFile = $"test_screenings_{Guid.NewGuid()}.json";
            var repository = new ScreeningRepository(testFile);

            var screening = new Screening
            {
                Movie = new Movie { Title = "Test Film", Duration = 90 },
                Hall = new Hall { Number = 1 },
                DateTime = new DateTime(2026, 1, 1, 18, 0, 0),
                IsPremiere = true
            };

            repository.Save(new List<Screening> { screening });
            var loaded = repository.LoadAll();

            Assert.AreEqual(1, loaded.Count);
            Assert.AreEqual("Test Film", loaded[0].Movie.Title);
            Assert.AreEqual(1, loaded[0].Hall.Number);
            Assert.IsTrue(loaded[0].IsPremiere);
        }

        [TestMethod]
        public void LoadAll_ReturnsEmptyList_WhenFileDoesNotExist()
        {
            string testFile = $"test_screenings_{Guid.NewGuid()}.json";
            var repository = new ScreeningRepository(testFile);

            var loaded = repository.LoadAll();

            Assert.AreEqual(0, loaded.Count);
        }
    }
}