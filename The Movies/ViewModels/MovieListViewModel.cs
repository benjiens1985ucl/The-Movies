using System.Collections.ObjectModel;
using System.Windows.Input;
using The_Movies.Models;
using The_Movies.Services;

namespace The_Movies.ViewModels
{
    public class MovieListViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly MovieRepository _movieRepository = new MovieRepository();

        public ObservableCollection<Movie> Movies { get; }

        public ICommand BackCommand { get; }

        public MovieListViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            var loadedMovies = _movieRepository.LoadAll();
            Movies = new ObservableCollection<Movie>(loadedMovies);

            BackCommand = new RelayCommand(_ => Back());
        }

        private void Back()
        {
            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }
    }
}