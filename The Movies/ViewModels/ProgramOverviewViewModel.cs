using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using The_Movies.Services;
using The_Movies.Models;

namespace The_Movies.ViewModels
{
    public class ProgramOverviewViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly ScreeningRepository _screeningRepository =
            new ScreeningRepository();

        public ObservableCollection<Screening> Screenings { get; }

        public ICommand BackCommand { get; }

        public ICollectionView FilteredScreenings { get; }
        public ObservableCollection<string> CinemaFilters { get; }
        
        private string _selectedCinemaFilter = "Alle";

        public string SelectedCinemaFilter
        {
            get => _selectedCinemaFilter;
            set
            {
                if (SetProperty(ref _selectedCinemaFilter, value))
                {
                    FilteredScreenings.Refresh();
                }
            }
        }

        public ObservableCollection<string> MovieFilters { get; }
        public ObservableCollection<string> GenreFilters { get; }

        public ObservableCollection<string> PeriodFilters { get; } =
            new ObservableCollection<string>
            {
                "Alle",
                "Dato",
                "Uge",
                "Maaned"
            };

        private string _selectedMovieFilter = "Alle";
        public string SelectedMovieFilter
        {
            get => _selectedMovieFilter;
            set
            {
                if (SetProperty(ref _selectedMovieFilter, value))
                {
                    FilteredScreenings.Refresh();
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
                    FilteredScreenings.Refresh();
                }
            }
        }

        private string _selectedPeriodFilter = "Alle";
        public string SelectedPeriodFilter
        {
            get => _selectedPeriodFilter;
            set
            {
                if (SetProperty(ref _selectedPeriodFilter, value))
                {
                    FilteredScreenings.Refresh();
                }
            }
        }

        private DateTime? _selectedFilterDate = DateTime.Today;
        public DateTime? SelectedFilterDate
        {
            get => _selectedFilterDate;
            set
            {
                if (SetProperty(ref _selectedFilterDate, value))
                {
                    FilteredScreenings.Refresh();
                }
            }
        }

        public ProgramOverviewViewModel(
            MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            var screenings = _screeningRepository
                .LoadAll()
                .OrderBy(s => s.DateTime);

            Screenings = new ObservableCollection<Screening>(screenings);

            CinemaFilters = new ObservableCollection<string>
            {
                "Alle"
            };

            foreach (var cinemaName in Screenings
                .Select(s => s.CinemaName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .OrderBy(name => name))
            {
                CinemaFilters.Add(cinemaName);
            }

            MovieFilters = new ObservableCollection<string>
            {
                "Alle"
            };
            
            foreach (var movieTitle in Screenings
                    .Select(s => s.Movie.Title)
                    .Where(title => !string.IsNullOrWhiteSpace(title))
                    .Distinct()
                    .OrderBy(title => title))
            {
                MovieFilters.Add(movieTitle);
            }

            GenreFilters = new ObservableCollection<string>
            {
                "Alle"
            };

            foreach (var genre in Screenings
                .Select(s => s.Movie.Genre.ToString())
                .Distinct()
                .OrderBy(genre => genre))
            {
                GenreFilters.Add(genre);
            }

            FilteredScreenings =
                CollectionViewSource.GetDefaultView(Screenings);

            FilteredScreenings.Filter = FilterScreening;


            BackCommand = new RelayCommand(_ => Back());

        }

        private bool FilterScreening(object item)
        {
            if (item is not Screening screening)
            {
                return false;
            }

            bool matchesCinema =
                SelectedCinemaFilter == "Alle"
                || screening.CinemaName == SelectedCinemaFilter;

            bool matchesMovie =
                SelectedMovieFilter == "Alle"
                || screening.Movie.Title == SelectedMovieFilter;

            bool matchesGenre =
                SelectedGenreFilter == "Alle"
                || screening.Movie.Genre.ToString() == SelectedGenreFilter;

            bool matchesDate =
                MatchesDateFilter(screening);

            return matchesCinema
                && matchesMovie
                && matchesGenre
                && matchesDate;
        }

        private bool MatchesDateFilter(Screening screening)
        {
            if (SelectedPeriodFilter == "Alle")
            {
                return true;
            }

            if (SelectedFilterDate == null)
            {
                return true;
            }

            DateTime screeningDate = screening.DateTime.Date;
            DateTime selectedDate = SelectedFilterDate.Value.Date;

            if (SelectedPeriodFilter == "Dato")
            {
                return screeningDate == selectedDate;
            }

            if (SelectedPeriodFilter == "Uge")
            {
                DateTime startOfWeek = GetStartOfWeek(selectedDate);
                DateTime endOfWeek = startOfWeek.AddDays(7);

                return screeningDate >= startOfWeek
                    && screeningDate < endOfWeek;
            }

            if (SelectedPeriodFilter == "Maaned")
            {
                return screeningDate.Year == selectedDate.Year
                && screeningDate.Month == selectedDate.Month;
            }

            return true;
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            int difference =
                (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;

            return date.AddDays(-difference).Date;
        }

        private void Back()
        {
            _mainWindowViewModel.CurrentView =
                new MainMenuViewModel(_mainWindowViewModel);
        }
    }
}
