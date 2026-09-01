using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using The_Movies.Models;
using The_Movies.Services;

namespace The_Movies.ViewModels
{
    public class RegisterMovieViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly MovieRepository _movieRepository = new MovieRepository();

        private Movie? _movieToEdit;
        private ObservableCollection<Movie>? _movies;

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _duration = string.Empty;
        public string Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        private Genre _genre; 
        public Genre Genre
        {
            get => _genre;
            set => SetProperty(ref _genre, value);
        }

        public IEnumerable<Genre> GenreValues => Enum.GetValues<Genre>();

        public string PageTitle =>
            _movieToEdit == null ? "Registrer Film" : "Rediger Film";

        public string SaveButtonText =>
            _movieToEdit == null ? "Gem" : "Gem Aendringer";
        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public RegisterMovieViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            SaveCommand = new RelayCommand(_ => Save(), CanSave);
            BackCommand = new RelayCommand(_ => Back());
        }

        public RegisterMovieViewModel(
            MainWindowViewModel mainWindowViewModel, 
            Movie movie,
            ObservableCollection<Movie> movies)
            : this(mainWindowViewModel)
        {
            _movieToEdit = movie;
            _movies = movies;

            Title = movie.Title;
            Duration = movie.Duration.ToString();
            Genre = movie.Genre;
        }

        private bool CanSave(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Title)
                && int.TryParse(Duration, out int duration)
                && duration > 0;
        }

        private void Save()
        {
            if (_movieToEdit == null)
            {
                var movie = new Movie
                {
                    Title = Title,
                    Duration = int.Parse(Duration),
                    Genre = Genre
                };

                var movies = _movieRepository.LoadAll();
                movies.Add(movie);
                _movieRepository.Save(movies);

                _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);

                return;
            }

            _movieToEdit.Title = Title;
            _movieToEdit.Duration = int.Parse(Duration);
            _movieToEdit.Genre = Genre;

            _movieRepository.Save(
                new List<Movie>(_movies!));

            _mainWindowViewModel.CurrentView =
                new MovieListViewModel(_mainWindowViewModel);

        }

        private void Back()
        {
            if (_movieToEdit != null)
            {
                _mainWindowViewModel.CurrentView = 
                    new MovieListViewModel(_mainWindowViewModel);
            }
            else
            {
                _mainWindowViewModel.CurrentView = 
                    new MainMenuViewModel(_mainWindowViewModel);
            }
        }
    }
}