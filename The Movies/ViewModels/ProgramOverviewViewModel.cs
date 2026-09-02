using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using The_Movies.Models;
using The_Movies.Services;

namespace The_Movies.ViewModels
{
    public class ProgramOverviewViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly CinemaRepository _cinemaRepository;
        private readonly MovieRepository _movieRepository;
        private readonly ScreeningRepository _screeningRepository;

        public ObservableCollection<Cinema> Cinemas { get; }
        public ObservableCollection<Hall> AvailableHalls { get; } = new ObservableCollection<Hall>();
        public ObservableCollection<DayColumn> WeekDays { get; } = new ObservableCollection<DayColumn>();

        public ObservableCollection<string> MovieFilters { get; } = new ObservableCollection<string> { "Alle" };
        public ObservableCollection<string> GenreFilters { get; } = new ObservableCollection<string> { "Alle" };

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

        private string _selectedMovieFilter = "Alle";
        public string SelectedMovieFilter
        {
            get => _selectedMovieFilter;
            set
            {
                if (SetProperty(ref _selectedMovieFilter, value))
                {
                    UpdateWeekDays();
                }
            }
        }

        private string _selectedGenreFilter = "Alle";
        public string SelectedGenreFilter
        {
            get => _selectedGenreFilter;
            set
            {
                if (SetProperty(ref _selectedGenreFilter, value))
                {
                    UpdateWeekDays();
                }
            }
        }

        public ICommand BackCommand { get; }
        public ICommand PreviousWeekCommand { get; }
        public ICommand NextWeekCommand { get; }

        public ProgramOverviewViewModel(
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

            var movies = _movieRepository.LoadAll();
            var allScreenings = _screeningRepository.LoadAll(movies);

            foreach (var title in allScreenings.Select(s => s.Movie.Title).Distinct().OrderBy(t => t))
            {
                MovieFilters.Add(title);
            }

            foreach (var genre in allScreenings.Select(s => s.Movie.Genre.ToString()).Distinct().OrderBy(g => g))
            {
                GenreFilters.Add(genre);
            }

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

            var movies = _movieRepository.LoadAll();
            var screenings = _screeningRepository.LoadAll(movies).AsEnumerable();

            if (SelectedCinema != null)
            {
                screenings = screenings.Where(s => s.CinemaName == SelectedCinema.Name);
            }

            if (SelectedHall != null)
            {
                screenings = screenings.Where(s => s.Hall.Number == SelectedHall.Number);
            }

            if (SelectedMovieFilter != "Alle")
            {
                screenings = screenings.Where(s => s.Movie.Title == SelectedMovieFilter);
            }

            if (SelectedGenreFilter != "Alle")
            {
                screenings = screenings.Where(s => s.Movie.Genre.ToString() == SelectedGenreFilter);
            }

            var screeningList = screenings.ToList();

            for (int i = 0; i < 7; i++)
            {
                var date = SelectedWeekStart.AddDays(i);
                var day = new DayColumn { Date = date };

                foreach (var screening in screeningList.Where(s => s.DateTime.Date == date).OrderBy(s => s.DateTime))
                {
                    day.Screenings.Add(screening);
                }

                WeekDays.Add(day);
            }
        }

        private void Back()
        {
            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }
    }
}