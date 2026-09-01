using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using The_Movies.Models;
using The_Movies.Services;
using The_Movies.ViewModels;

namespace TheMovies.Tests
{
    [TestClass]
    public class ProgramViewModelTests
    {
        private ProgramViewModel CreateViewModel(MainWindowViewModel mainWindowViewModel)
        {
            string suffix = Guid.NewGuid().ToString();

            var cinemaRepository = new CinemaRepository($"test_cinemas_{suffix}.json");
            var movieRepository = new MovieRepository($"test_movies_{suffix}.json");
            var screeningRepository = new ScreeningRepository($"test_screenings_{suffix}.json");

            return new ProgramViewModel(mainWindowViewModel, cinemaRepository, movieRepository, screeningRepository);
        }

        [TestMethod]
        public void CanSave_ReturnsFalse_WhenNothingIsSelected()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var viewModel = CreateViewModel(mainWindowViewModel);

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsFalse(canSave);
        }

        [TestMethod]
        public void CanSave_ReturnsFalse_WhenMovieIsMissing()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var viewModel = CreateViewModel(mainWindowViewModel);

            viewModel.SelectedCinema = new Cinema { Name = "Test Biograf" };
            viewModel.SelectedHall = new Hall { Number = 1 };
            viewModel.SelectedDate = DateTime.Today;
            viewModel.SelectedStartTime = "18:00";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsFalse(canSave);
        }

        [TestMethod]
        public void CanSave_ReturnsTrue_WhenAllFieldsAreSelected()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var viewModel = CreateViewModel(mainWindowViewModel);

            viewModel.SelectedCinema = new Cinema { Name = "Test Biograf" };
            viewModel.SelectedHall = new Hall { Number = 1 };
            viewModel.SelectedMovie = new Movie { Title = "Test Film", Duration = 90 };
            viewModel.SelectedDate = DateTime.Today;
            viewModel.SelectedStartTime = "18:00";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsTrue(canSave);
        }
    }
}