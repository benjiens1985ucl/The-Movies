using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using The_Movies.Models;
using The_Movies.Services;
using The_Movies.ViewModels;

namespace TheMovies.Tests
{
    [TestClass]
    public class RegisterMovieViewModelTests
    {
        private MovieRepository CreateTestRepository()
        {
            return new MovieRepository($"test_movies_{Guid.NewGuid()}.json");
        }

        [TestMethod]
        public void CanSave_ReturnsFalse_WhenTitleIsEmpty()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var viewModel = new RegisterMovieViewModel(mainWindowViewModel, CreateTestRepository());

            viewModel.Title = "";
            viewModel.Duration = "90";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsFalse(canSave);
        }

        [TestMethod]
        public void CanSave_ReturnsFalse_WhenDurationIsNotANumber()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var viewModel = new RegisterMovieViewModel(mainWindowViewModel, CreateTestRepository());

            viewModel.Title = "Test Film";
            viewModel.Duration = "abc";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsFalse(canSave);
        }

        [TestMethod]
        public void CanSave_ReturnsTrue_WhenTitleAndDurationAreValid()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var viewModel = new RegisterMovieViewModel(mainWindowViewModel, CreateTestRepository());

            viewModel.Title = "Test Film";
            viewModel.Duration = "90";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsTrue(canSave);
        }

        [TestMethod]
        public void Save_UpdatesMovieData_WhenEditingExistingMovie()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var movieToEdit = new Movie { Title = "Gammel Titel", Duration = 90, Genre = Genre.Action };
            var movies = new ObservableCollection<Movie> { movieToEdit };

            var viewModel = new RegisterMovieViewModel(mainWindowViewModel, movieToEdit, movies, CreateTestRepository());
            viewModel.Title = "Ny Titel";
            viewModel.Duration = "120";
            viewModel.Genre = Genre.Comedy;

            viewModel.SaveCommand.Execute(null);

            Assert.AreEqual("Ny Titel", movieToEdit.Title);
            Assert.AreEqual(120, movieToEdit.Duration);
            Assert.AreEqual(Genre.Comedy, movieToEdit.Genre);
        }

        [TestMethod]
        public void Save_DoesNotAffectOtherMovies_WhenEditingExistingMovie()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var movieToEdit = new Movie { Title = "Redigeres", Duration = 90, Genre = Genre.Action };
            var otherMovie = new Movie { Title = "Uroert Film", Duration = 100, Genre = Genre.Drama };
            var movies = new ObservableCollection<Movie> { movieToEdit, otherMovie };

            var viewModel = new RegisterMovieViewModel(mainWindowViewModel, movieToEdit, movies, CreateTestRepository());
            viewModel.Title = "Redigeret Titel";
            viewModel.Duration = "150";

            viewModel.SaveCommand.Execute(null);

            Assert.AreEqual("Uroert Film", otherMovie.Title);
            Assert.AreEqual(100, otherMovie.Duration);
            Assert.AreEqual(Genre.Drama, otherMovie.Genre);
        }
    }
}