using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private readonly ScreeningRepository _screeningRepository;

        public ObservableCollection<Movie> Movies { get; }

        public Movie? SelectedMovie { get; set; }

        public ICommand BackCommand { get; }
        public ICommand DeleteMovieCommand { get; }
        public ICommand EditMovieCommand { get; }

        public MovieListViewModel(
            MainWindowViewModel mainWindowViewModel,
            MovieRepository? movieRepository = null,
            ScreeningRepository? screeningRepository = null)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _movieRepository = movieRepository ?? new MovieRepository();
            _screeningRepository = screeningRepository ?? new ScreeningRepository();

            var loadedMovies = _movieRepository.LoadAll();
            Movies = new ObservableCollection<Movie>(loadedMovies);

            BackCommand = new RelayCommand(_ => Back());
            DeleteMovieCommand = new RelayCommand(_ => DeleteMovie());
            EditMovieCommand = new RelayCommand(_ => EditMovie());
        }

        public bool IsMovieOnProgram(Movie movie)
        {
            var screenings = _screeningRepository.LoadAll();
            return screenings.Any(s => s.MovieId == movie.Id);
        }

        private void EditMovie()
        {
            if (SelectedMovie == null)
            {
                MessageBox.Show(
                    "Vælg en film, du vil redigere.",
                    "Ingen film valgt.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            _mainWindowViewModel.CurrentView =
                new RegisterMovieViewModel(
                    _mainWindowViewModel,
                    SelectedMovie,
                    Movies);
        }

        private void DeleteMovie()
        {
            if (SelectedMovie == null)
            {
                MessageBox.Show(
                    "Vælg den film du ønsker at slette.",
                    "Ingen film valgt.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            if (IsMovieOnProgram(SelectedMovie))
            {
                MessageBox.Show(
                    $"\"{SelectedMovie.Title}\" er på programmet og kan ikke slettes.",
                    "Kan ikke slette film",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var result = MessageBox.Show(
                $"Er du sikker på, at du vil slette \"{SelectedMovie.Title}\"?",
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