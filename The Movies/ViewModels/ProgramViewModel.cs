using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using The_Movies.Models;
using The_Movies.Services;

namespace The_Movies.ViewModels
{
    public class ProgramViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly CinemaRepository _cinemaRepository;
        private readonly MovieRepository _movieRepository;
        private readonly ScreeningRepository _screeningRepository;

        public ObservableCollection<Cinema> Cinemas { get; }
        public ObservableCollection<Movie> Movies { get; }
        public ObservableCollection<Hall> AvailableHalls { get; } = new ObservableCollection<Hall>();

        public ObservableCollection<string> AvailableStartTimes { get; }

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

        private DateTime? _selectedDate = DateTime.Today;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        private string? _selectedStartTime;
        public string? SelectedStartTime
        {
            get => _selectedStartTime;
            set => SetProperty(ref _selectedStartTime, value);
        }

        private bool _isPremiere;
        public bool IsPremiere
        {
            get => _isPremiere;
            set => SetProperty(ref _isPremiere, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public ProgramViewModel(
            MainWindowViewModel mainWindowViewModel,
            CinemaRepository? cinemaRepository = null,
            MovieRepository? movieRepository = null,
            ScreeningRepository? screeningRepository = null)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _cinemaRepository = cinemaRepository ?? new CinemaRepository();
            _movieRepository = movieRepository ?? new MovieRepository();
            _screeningRepository = screeningRepository ?? new ScreeningRepository();

            Cinemas = new ObservableCollection<Cinema>(_cinemaRepository.LoadAll());
            Movies = new ObservableCollection<Movie>(_movieRepository.LoadAll());

            AvailableStartTimes = new ObservableCollection<string>();
            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute < 60; minute += 15)
                {
                    AvailableStartTimes.Add($"{hour:00}:{minute:00}");
                }
            }

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
                && SelectedMovie != null
                && SelectedDate != null
                && SelectedStartTime != null;
        }

        private void Save()
        {
            TimeSpan startTime = 
                TimeSpan.Parse(SelectedStartTime!);

            DateTime screeningDateTime =
                SelectedDate!.Value.Date.Add(startTime);

            if (screeningDateTime < DateTime.Now)
            {
                MessageBox.Show(
                    "Du kan ikke oprette en visning i fortiden.",
                    "Ugyldigt Tidspunkt",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var screening = new Screening
            {
                Movie = SelectedMovie!,
                Hall = SelectedHall!,
                DateTime = screeningDateTime,
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