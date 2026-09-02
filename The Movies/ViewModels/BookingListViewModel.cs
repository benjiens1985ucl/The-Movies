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
    public class BookingDisplayItem
    {
        public Booking Booking { get; set; } = new Booking();
        public Screening Screening { get; set; } = new Screening();
    }

    public class BookingListViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly CinemaRepository _cinemaRepository;
        private readonly MovieRepository _movieRepository;
        private readonly ScreeningRepository _screeningRepository;
        private readonly BookingRepository _bookingRepository;

        public ObservableCollection<Cinema> Cinemas { get; }
        public ObservableCollection<Hall> AvailableHalls { get; } = new ObservableCollection<Hall>();
        public ObservableCollection<BookingDisplayItem> Bookings { get; } = new ObservableCollection<BookingDisplayItem>();

        private Cinema? _selectedCinema;
        public Cinema? SelectedCinema
        {
            get => _selectedCinema;
            set
            {
                if (SetProperty(ref _selectedCinema, value))
                {
                    UpdateAvailableHalls();
                    UpdateBookings();
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
                    UpdateBookings();
                }
            }
        }

        public BookingDisplayItem? SelectedBooking { get; set; }

        public ICommand BackCommand { get; }
        public ICommand DeleteBookingCommand { get; }
        public ICommand EditBookingCommand { get; }

        public BookingListViewModel(
            MainWindowViewModel mainWindowViewModel,
            CinemaRepository? cinemaRepository = null,
            MovieRepository? movieRepository = null,
            ScreeningRepository? screeningRepository = null,
            BookingRepository? bookingRepository = null)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _cinemaRepository = cinemaRepository ?? new CinemaRepository();
            _movieRepository = movieRepository ?? new MovieRepository();
            _screeningRepository = screeningRepository ?? new ScreeningRepository();
            _bookingRepository = bookingRepository ?? new BookingRepository();

            Cinemas = new ObservableCollection<Cinema>(_cinemaRepository.LoadAll());

            BackCommand = new RelayCommand(_ => Back());
            DeleteBookingCommand = new RelayCommand(_ => DeleteBooking());
            EditBookingCommand = new RelayCommand(_ => EditBooking());

            UpdateBookings();
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

        public void UpdateBookings()
        {
            Bookings.Clear();

            var movies = _movieRepository.LoadAll();
            var screenings = _screeningRepository.LoadAll(movies);
            var bookings = _bookingRepository.LoadAll();

            var items = bookings
                .Select(b => new BookingDisplayItem
                {
                    Booking = b,
                    Screening = screenings.FirstOrDefault(s => s.Id == b.ScreeningId) ?? new Screening()
                })
                .AsEnumerable();

            if (SelectedCinema != null)
            {
                items = items.Where(i => i.Screening.CinemaName == SelectedCinema.Name);
            }

            if (SelectedHall != null)
            {
                items = items.Where(i => i.Screening.Hall.Number == SelectedHall.Number);
            }

            foreach (var item in items.OrderBy(i => i.Screening.DateTime))
            {
                Bookings.Add(item);
            }
        }

        private void EditBooking()
        {
            if (SelectedBooking == null)
            {
                MessageBox.Show(
                    "Vaelg den booking du oensker at redigere.",
                    "Ingen booking valgt.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            _mainWindowViewModel.CurrentView = new EditBookingViewModel(
                _mainWindowViewModel,
                SelectedBooking.Booking,
                SelectedBooking.Screening);
        }

        private void DeleteBooking()
        {
            if (SelectedBooking == null)
            {
                MessageBox.Show(
                    "Vaelg den booking du oensker at slette.",
                    "Ingen booking valgt.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var result = MessageBox.Show(
                $"Er du sikker paa, at du vil slette bookingen paa {SelectedBooking.Booking.TicketCount} billet(ter)?",
                "Slet booking",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            _bookingRepository.Delete(SelectedBooking.Booking.Id);
            UpdateBookings();
        }

        private void Back()
        {
            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }
    }
}