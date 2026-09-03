using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using The_Movies.Models;
using The_Movies.Services;
using The_Movies.ViewModels;

namespace TheMovies.Tests
{
    [TestClass]
    public class BookingViewModelTests
    {
        private (BookingViewModel viewModel, CinemaRepository cinemaRepository, ScreeningRepository screeningRepository, MovieRepository movieRepository, BookingRepository bookingRepository) CreateViewModel(
            MainWindowViewModel mainWindowViewModel)
        {
            string suffix = Guid.NewGuid().ToString();

            var cinemaRepository = new CinemaRepository($"test_cinemas_{suffix}.json");
            var screeningRepository = new ScreeningRepository($"test_screenings_{suffix}.json");
            var movieRepository = new MovieRepository($"test_movies_{suffix}.json");
            var bookingRepository = new BookingRepository($"test_bookings_{suffix}.json");

            var viewModel = new BookingViewModel(mainWindowViewModel, cinemaRepository, screeningRepository, movieRepository, bookingRepository);

            return (viewModel, cinemaRepository, screeningRepository, movieRepository, bookingRepository);
        }

        [TestMethod]
        public void CanSave_ReturnsFalse_WhenNoScreeningIsSelected()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var (viewModel, _, _, _, _) = CreateViewModel(mainWindowViewModel);

            viewModel.TicketCount = "2";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsFalse(canSave);
        }

        [TestMethod]
        public void CanSave_ReturnsFalse_WhenTicketCountExceedsAvailableSeats()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var (viewModel, _, _, _, _) = CreateViewModel(mainWindowViewModel);

            var screening = new Screening
            {
                Movie = new Movie { Title = "Test Film", Duration = 90 },
                Hall = new Hall { Number = 1, Capacity = 10 }
            };

            viewModel.SelectedScreening = screening;
            viewModel.TicketCount = "11";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsFalse(canSave);
        }

        [TestMethod]
        public void CanSave_ReturnsFalse_WhenNoSeatsAreAvailable()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var (viewModel, _, _, _, _) = CreateViewModel(mainWindowViewModel);

            var screening = new Screening
            {
                Movie = new Movie { Title = "Udsolgt Film", Duration = 90 },
                Hall = new Hall { Number = 1, Capacity = 0 }
            };

            viewModel.SelectedScreening = screening;
            viewModel.TicketCount = "1";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsFalse(canSave);
        }

        [TestMethod]
        public void CanSave_ReturnsTrue_WhenTicketCountIsWithinCapacity()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var (viewModel, _, _, _, _) = CreateViewModel(mainWindowViewModel);

            var screening = new Screening
            {
                Movie = new Movie { Title = "Test Film", Duration = 90 },
                Hall = new Hall { Number = 1, Capacity = 10 }
            };

            viewModel.SelectedScreening = screening;
            viewModel.TicketCount = "5";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsTrue(canSave);
        }

        [TestMethod]
        public void Save_ReducesAvailableSeats_AfterBooking()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var (viewModel, _, _, _, _) = CreateViewModel(mainWindowViewModel);

            var screening = new Screening
            {
                Movie = new Movie { Title = "Test Film", Duration = 90 },
                Hall = new Hall { Number = 1, Capacity = 10 }
            };

            viewModel.SelectedScreening = screening;
            viewModel.TicketCount = "4";
            viewModel.SaveCommand.Execute(null);

            Assert.AreEqual(6, viewModel.AvailableSeats);
        }
    }
}