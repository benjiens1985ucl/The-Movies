using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using The_Movies.Models;
using The_Movies.Services;

namespace The_Movies.ViewModels
{
    public class BookingViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly ScreeningRepository _screeningRepository;
        private readonly BookingRepository _bookingRepository;

        public ObservableCollection<Screening> Screenings { get; }

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

        public ICommand SaveCommand { get; }
        public ICommand BackCommand { get; }

        public BookingViewModel(
            MainWindowViewModel mainWindowViewModel,
            ScreeningRepository? screeningRepository = null,
            BookingRepository? bookingRepository = null)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _screeningRepository = screeningRepository ?? new ScreeningRepository();
            _bookingRepository = bookingRepository ?? new BookingRepository();

            Screenings = new ObservableCollection<Screening>(_screeningRepository.LoadAll());

            SaveCommand = new RelayCommand(_ => Save(), CanSave);
            BackCommand = new RelayCommand(_ => Back());
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
            var booking = new Booking
            {
                ScreeningId = SelectedScreening!.Id,
                TicketCount = int.Parse(TicketCount),
                CustomerName = CustomerName
            };

            var bookings = _bookingRepository.LoadAll();
            bookings.Add(booking);
            _bookingRepository.Save(bookings);

            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }

        private void Back()
        {
            _mainWindowViewModel.CurrentView = new MainMenuViewModel(_mainWindowViewModel);
        }
    }
}