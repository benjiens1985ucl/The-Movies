using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using The_Movies.Models;
using The_Movies.Services;

namespace The_Movies.ViewModels
{
    public class MovieListViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly MovieRepository _movieRepository;

        public ObservableCollection<Movie> Movies { get; }

        public Movie? SelectedMovie { get; set; }

        public ICommand BackCommand { get; }
        public ICommand DeleteMovieCommand { get; }

        public MovieListViewModel(MainWindowViewModel mainWindowViewModel, MovieRepository? movieRepository = null)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _movieRepository = movieRepository ?? new MovieRepository();

            var loadedMovies = _movieRepository.LoadAll();
            Movies = new ObservableCollection<Movie>(loadedMovies);

            BackCommand = new RelayCommand(_ => Back());
            DeleteMovieCommand = new RelayCommand(_ => DeleteMovie());
        }

        private void DeleteMovie()
        {
            if (SelectedMovie == null)
            {
                MessageBox.Show(
                    "Vaelg den film du oensker at slette.",
                    "Ingen film valgt.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var result = MessageBox.Show(
                $"Er du sikker paa, at du vil slette \"{SelectedMovie.Title}\"?",
                "Slet film",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            RemoveMovie(SelectedMovie);
            SelectedMovie = null;
        }

        public void RemoveMovie(Movie movie)
        {
            Movies.Remove(movie);
            _movieRepository.Save(new List<Movie>(Movies));
        }

        private void Back()
        {
            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }
    }
}