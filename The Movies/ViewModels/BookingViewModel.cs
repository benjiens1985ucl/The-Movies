using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using The_Movies.Models;
using The_Movies.Services;

namespace The_Movies.ViewModels
{
    public class BookingViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly CinemaRepository _cinemaRepository;
        private readonly ScreeningRepository _screeningRepository;
        private readonly MovieRepository _movieRepository;
        private readonly BookingRepository _bookingRepository;

        private List<Screening> _allScreenings = new List<Screening>();

        public ObservableCollection<Cinema> Cinemas { get; }
        public ObservableCollection<Screening> Screenings { get; } = new ObservableCollection<Screening>();

        private Cinema? _selectedCinema;
        public Cinema? SelectedCinema
        {
            get => _selectedCinema;
            set
            {
                if (SetProperty(ref _selectedCinema, value))
                {
                    UpdateScreenings();
                }
            }
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    UpdateScreenings();
                }
            }
        }

        private Screening? _selectedScreening;
        public Screening? SelectedScreening
        {
            get => _selectedScreening;
            set
            {
                if (SetProperty(ref _selectedScreening, value))
                {
                    UpdateAvailableSeats();
                }
            }
        }

        private int _availableSeats;
        public int AvailableSeats
        {
            get => _availableSeats;
            set => SetProperty(ref _availableSeats, value);
        }

        private string _ticketCount = string.Empty;
        public string TicketCount
        {
            get => _ticketCount;
            set => SetProperty(ref _ticketCount, value);
        }

        private string _customerName = string.Empty;
        public string CustomerName
        {
            get => _customerName;
            set => SetProperty(ref _customerName, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public BookingViewModel(
            MainWindowViewModel mainWindowViewModel,
            CinemaRepository? cinemaRepository = null,
            ScreeningRepository? screeningRepository = null,
            MovieRepository? movieRepository = null,
            BookingRepository? bookingRepository = null)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _cinemaRepository = cinemaRepository ?? new CinemaRepository();
            _screeningRepository = screeningRepository ?? new ScreeningRepository();
            _movieRepository = movieRepository ?? new MovieRepository();
            _bookingRepository = bookingRepository ?? new BookingRepository();

            Cinemas = new ObservableCollection<Cinema>(_cinemaRepository.LoadAll());

            var movies = _movieRepository.LoadAll();
            _allScreenings = _screeningRepository.LoadAll(movies);

            SaveCommand = new RelayCommand(_ => Save(), CanSave);
            BackCommand = new RelayCommand(_ => Back());

            UpdateScreenings();
        }

        private void UpdateScreenings()
        {
            Screenings.Clear();
            SelectedScreening = null;

            var filtered = _allScreenings.AsEnumerable();

            if (SelectedCinema != null)
            {
                filtered = filtered.Where(s => s.CinemaName == SelectedCinema.Name);
            }

            if (SelectedDate != null)
            {
                filtered = filtered.Where(s => s.DateTime.Date == SelectedDate.Value.Date);
            }

            foreach (var screening in filtered.OrderBy(s => s.DateTime))
            {
                Screenings.Add(screening);
            }
        }

        private void UpdateAvailableSeats()
        {
            if (SelectedScreening == null)
            {
                AvailableSeats = 0;
                return;
            }

            int ticketsSold = _bookingRepository.GetTicketsSold(SelectedScreening.Id);
            AvailableSeats = SelectedScreening.Hall.Capacity - ticketsSold;
        }

        private bool CanSave(object? parameter)
        {
            return SelectedScreening != null
                && int.TryParse(TicketCount, out int count)
                && count > 0
                && count <= AvailableSeats;
        }

        private void Save()
        {
            int ticketCount = int.Parse(TicketCount);

            var booking = new Booking
            {
                ScreeningId = SelectedScreening!.Id,
                TicketCount = ticketCount,
                CustomerName = CustomerName
            };

            var bookings = _bookingRepository.LoadAll();
            bookings.Add(booking);
            _bookingRepository.Save(bookings);

            StatusMessage = $"Du har booket {ticketCount} billet(ter).";

            TicketCount = string.Empty;
            CustomerName = string.Empty;

            UpdateAvailableSeats();
        }

        private void Back()
        {
            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }
    }
}