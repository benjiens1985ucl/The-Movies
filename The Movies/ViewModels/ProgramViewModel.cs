using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using The_Movies.Models;
using The_Movies.Services;

namespace The_Movies.ViewModels
{
    public class ProgramViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly CinemaRepository _cinemaRepository = new CinemaRepository();
        private readonly MovieRepository _movieRepository = new MovieRepository();
        private readonly ScreeningRepository _screeningRepository = new ScreeningRepository();

        public ObservableCollection<Cinema> Cinemas { get; }
        public ObservableCollection<Movie> Movies { get; }
        public ObservableCollection<Hall> AvailableHalls { get; } = new ObservableCollection<Hall>();

        private Cinema? _selectedCinema;
        public Cinema? SelectedCinema
        {
            get => _selectedCinema;
            set
            {
                if (SetProperty(ref _selectedCinema, value))
                {
                    UpdateAvailableHalls();
                }
            }
        }

        private Hall? _selectedHall;
        public Hall? SelectedHall
        {
            get => _selectedHall;
            set => SetProperty(ref _selectedHall, value);
        }

        private Movie? _selectedMovie;
        public Movie? SelectedMovie
        {
            get => _selectedMovie;
            set => SetProperty(ref _selectedMovie, value);
        }

        private DateTime _selectedDateTime = DateTime.Now;
        public DateTime SelectedDateTime
        {
            get => _selectedDateTime;
            set => SetProperty(ref _selectedDateTime, value);
        }

        private bool _isPremiere;
        public bool IsPremiere
        {
            get => _isPremiere;
            set => SetProperty(ref _isPremiere, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public ProgramViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            Cinemas = new ObservableCollection<Cinema>(_cinemaRepository.LoadAll());
            Movies = new ObservableCollection<Movie>(_movieRepository.LoadAll());

            SaveCommand = new RelayCommand(_ => Save(), CanSave);
            BackCommand = new RelayCommand(_ => Back());
        }

        private void UpdateAvailableHalls()
        {
            AvailableHalls.Clear();
            SelectedHall = null;

            if (SelectedCinema == null)
            {
                return;
            }

            foreach (var hall in SelectedCinema.Halls)
            {
                AvailableHalls.Add(hall);
            }
        }

        private bool CanSave(object? parameter)
        {
            return SelectedCinema != null
                && SelectedHall != null
                && SelectedMovie != null;
        }

        private void Save()
        {
            var screening = new Screening
            {
                Movie = SelectedMovie!,
                Hall = SelectedHall!,
                DateTime = SelectedDateTime,
                IsPremiere = IsPremiere
            };

            var screenings = _screeningRepository.LoadAll();
            screenings.Add(screening);
            _screeningRepository.Save(screenings);

            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }

        private void Back()
        {
            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }
    }
}