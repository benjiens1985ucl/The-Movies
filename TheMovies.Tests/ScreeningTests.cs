using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using The_Movies.Models;

namespace TheMovies.Tests
{
    [TestClass]
    public class ScreeningTests
    {
        [TestMethod]
        public void GetMovieStartTime_AddsTwentyMinutesToScreeningTime()
        {
            var screening = new Screening
            {
                DateTime = new DateTime(2026, 1, 1, 18, 0, 0),
                Movie = new Movie { Duration = 90 }
            };

            var result = screening.GetMovieStartTime();

            Assert.AreEqual(new DateTime(2026, 1, 1, 18, 20, 0), result);
        }

        [TestMethod]
        public void GetMovieEndTime_AddsMovieDurationAfterStartTime()
        {
            var screening = new Screening
            {
                DateTime = new DateTime(2026, 1, 1, 18, 0, 0),
                Movie = new Movie { Duration = 90 }
            };

            var result = screening.GetMovieEndTime();

            // 18:00 + 20 min reklamer + 90 min film = 19:50
            Assert.AreEqual(new DateTime(2026, 1, 1, 19, 50, 0), result);
        }

        [TestMethod]
        public void GetHallAvailableTime_AddsTwentyMinutesCleanupAfterMovieEnds()
        {
            var screening = new Screening
            {
                DateTime = new DateTime(2026, 1, 1, 18, 0, 0),
                Movie = new Movie { Duration = 90 }
            };

            var result = screening.GetHallAvailableTime();

            // 19:50 + 20 min oprydning = 20:10
            Assert.AreEqual(new DateTime(2026, 1, 1, 20, 10, 0), result);
        }
    }
}