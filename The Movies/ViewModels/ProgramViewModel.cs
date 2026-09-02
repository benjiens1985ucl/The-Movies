using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using The_Movies.Models;
using The_Movies.Services;

namespace The_Movies.ViewModels
{
    public class DayColumn
    {
        public DateTime Date { get; set; }
        public ObservableCollection<Screening> Screenings { get; set; } = new ObservableCollection<Screening>();
    }

    public class ProgramViewModel : ViewModelBase
    {
        private static readonly TimeSpan OpeningTime = new TimeSpan(10, 0, 0);

        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly CinemaRepository _cinemaRepository;
        private readonly MovieRepository _movieRepository;
        private readonly ScreeningRepository _screeningRepository;

        public ObservableCollection<Cinema> Cinemas { get; }
        public ObservableCollection<Movie> Movies { get; }
        public ObservableCollection<Hall> AvailableHalls { get; } = new ObservableCollection<Hall>();

        public ObservableCollection<DayColumn> WeekDays { get; } = new ObservableCollection<DayColumn>();

        private DateTime _selectedWeekStart = GetStartOfWeek(DateTime.Today);
        public DateTime SelectedWeekStart
        {
            get => _selectedWeekStart;
            set
            {
                if (SetProperty(ref _selectedWeekStart, value))
                {
                    UpdateWeekDays();
                }
            }
        }

        public string WeekLabel => $"{SelectedWeekStart:dd-MM-yyyy} - {SelectedWeekStart.AddDays(6):dd-MM-yyyy}";

        private Cinema? _selectedCinema;
        public Cinema? SelectedCinema
        {
            get => _selectedCinema;
            set
            {
                if (SetProperty(ref _selectedCinema, value))
                {
                    UpdateAvailableHalls();
                    UpdateWeekDays();
                }
            }
        }

        private Hall? _selectedHall;
        public Hall? SelectedHall
        {
            get => _selectedHall;
            set
            {
                if (SetProperty(ref _selectedHall, value))
                {
                    UpdateWeekDays();
                }
            }
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

        private bool _isPremiere;
        public bool IsPremiere
        {
            get => _isPremiere;
            set => SetProperty(ref _isPremiere, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand PreviousWeekCommand { get; }
        public ICommand NextWeekCommand { get; }

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

            SaveCommand = new RelayCommand(_ => Save(), CanSave);
            BackCommand = new RelayCommand(_ => Back());
            PreviousWeekCommand = new RelayCommand(_ => SelectedWeekStart = SelectedWeekStart.AddDays(-7));
            NextWeekCommand = new RelayCommand(_ => SelectedWeekStart = SelectedWeekStart.AddDays(7));

            UpdateWeekDays();
        }

        private static DateTime GetStartOfWeek(DateTime date)
        {
            int difference = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-difference).Date;
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

        private void UpdateWeekDays()
        {
            WeekDays.Clear();
            OnPropertyChanged(nameof(WeekLabel));

            if (SelectedCinema == null || SelectedHall == null)
            {
                return;
            }

            var screenings = _screeningRepository.LoadAll()
                .Where(s => s.CinemaName == SelectedCinema.Name && s.Hall.Number == SelectedHall.Number)
                .ToList();

            for (int i = 0; i < 7; i++)
            {
                var date = SelectedWeekStart.AddDays(i);
                var day = new DayColumn { Date = date };

                foreach (var screening in screenings.Where(s => s.DateTime.Date == date).OrderBy(s => s.DateTime))
                {
                    day.Screenings.Add(screening);
                }

                WeekDays.Add(day);
            }
        }

        private DateTime GetNextAvailableStartTime(DateTime date)
        {
            var screenings = _screeningRepository.LoadAll()
                .Where(s => s.CinemaName == SelectedCinema!.Name
                    && s.Hall.Number == SelectedHall!.Number
                    && s.DateTime.Date == date.Date)
                .ToList();

            if (screenings.Count == 0)
            {
                return date.Date.Add(OpeningTime);
            }

            var lastScreening = screenings.OrderBy(s => s.DateTime).Last();
            return lastScreening.GetHallAvailableTime();
        }

        private bool CanSave(object? parameter)
        {
            return SelectedCinema != null
                && SelectedHall != null
                && SelectedMovie != null
                && SelectedDate != null;
        }

        private void Save()
        {
            DateTime screeningDateTime = GetNextAvailableStartTime(SelectedDate!.Value);

            var screening = new Screening
            {
                Movie = SelectedMovie!,
                CinemaName = SelectedCinema!.Name,
                Hall = SelectedHall!,
                DateTime = screeningDateTime,
                IsPremiere = IsPremiere
            };

            var screenings = _screeningRepository.LoadAll();
            screenings.Add(screening);
            _screeningRepository.Save(screenings);

            StatusMessage = $"\"{screening.Movie.Title}\" er tilføjet kl. {screeningDateTime:HH:mm}.";

            SelectedMovie = null;
            IsPremiere = false;

            UpdateWeekDays();
        }

        private void Back()
        {
            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }
    }
}