using System;
using System.Collections.Generic;
using System.Windows.Input;
using The_Movies.Models;
using The_Movies.Services;

namespace The_Movies.ViewModels
{
    public class RegisterMovieViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly MovieRepository _movieRepository = new MovieRepository();

        private bool CanSave(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Title) && Duration > 0;
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private int _duration;
        public int Duration
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

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public RegisterMovieViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            SaveCommand = new RelayCommand(_ => Save(), CanSave);
            BackCommand = new RelayCommand(_ => Back());
        }

        private void Save()
        {
            var movie = new Movie
            {
                Title = Title,
                Duration = Duration,
                Genre = Genre
            };

            var movies = _movieRepository.LoadAll();
            movies.Add(movie);
            _movieRepository.Save(movies);

            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }

        private void Back()
        {
            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }
    }
}