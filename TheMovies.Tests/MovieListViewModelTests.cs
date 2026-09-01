using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using The_Movies.Models;
using The_Movies.Services;
using The_Movies.ViewModels;

namespace TheMovies.Tests
{
    [TestClass]
    public class MovieListViewModelTests
    {
        private MovieListViewModel CreateViewModel(MainWindowViewModel mainWindowViewModel)
        {
            string testFile = $"test_movies_{Guid.NewGuid()}.json";
            var movieRepository = new MovieRepository(testFile);

            return new MovieListViewModel(mainWindowViewModel, movieRepository);
        }

        [TestMethod]
        public void RemoveMovie_RemovesTheMovieFromTheList()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var viewModel = CreateViewModel(mainWindowViewModel);

            var movie = new Movie { Title = "Test Film", Duration = 90 };
            viewModel.Movies.Add(movie);

            viewModel.RemoveMovie(movie);

            Assert.IsFalse(viewModel.Movies.Contains(movie));
        }

        [TestMethod]
        public void RemoveMovie_DoesNotAffectOtherMovies()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var viewModel = CreateViewModel(mainWindowViewModel);

            var movieToKeep = new Movie { Title = "Behold Denne", Duration = 90 };
            var movieToRemove = new Movie { Title = "Slet Denne", Duration = 100 };
            viewModel.Movies.Add(movieToKeep);
            viewModel.Movies.Add(movieToRemove);

            viewModel.RemoveMovie(movieToRemove);

            Assert.IsTrue(viewModel.Movies.Contains(movieToKeep));
            Assert.IsFalse(viewModel.Movies.Contains(movieToRemove));
        }
    }
}