using Microsoft.VisualStudio.TestTools.UnitTesting;
using The_Movies.ViewModels;

namespace TheMovies.Tests
{
    [TestClass]
    public class RegisterMovieViewModelTests
    {
        [TestMethod]
        public void CanSave_ReturnsFalse_WhenTitleIsEmpty()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var viewModel = new RegisterMovieViewModel(mainWindowViewModel);

            viewModel.Title = "";
            viewModel.Duration = "90";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsFalse(canSave);
        }

        [TestMethod]
        public void CanSave_ReturnsFalse_WhenDurationIsNotANumber()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var viewModel = new RegisterMovieViewModel(mainWindowViewModel);

            viewModel.Title = "Test Film";
            viewModel.Duration = "abc";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsFalse(canSave);
        }

        [TestMethod]
        public void CanSave_ReturnsTrue_WhenTitleAndDurationAreValid()
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var viewModel = new RegisterMovieViewModel(mainWindowViewModel);

            viewModel.Title = "Test Film";
            viewModel.Duration = "90";

            bool canSave = viewModel.SaveCommand.CanExecute(null);

            Assert.IsTrue(canSave);
        }
    }
}