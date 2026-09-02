using System.Windows.Input;
using The_Movies.Models;
using The_Movies.Services;

namespace The_Movies.ViewModels
{
    public class EditBookingViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly BookingRepository _bookingRepository;
        private readonly Booking _bookingToEdit;
        private readonly Screening _screening;

        public string MovieTitle => _screening.Movie.Title;
        public string ScreeningInfo => $"{_screening.CinemaName} - Sal {_screening.Hall.Number} - {_screening.DateTime:dd-MM-yyyy HH:mm}";

        private int _originalTicketCount;

        private string _ticketCount;
        public string TicketCount
        {
            get => _ticketCount;
            set => SetProperty(ref _ticketCount, value);
        }

        private string _customerName;
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

        public EditBookingViewModel(
            MainWindowViewModel mainWindowViewModel,
            Booking booking,
            Screening screening,
            BookingRepository? bookingRepository = null)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _bookingRepository = bookingRepository ?? new BookingRepository();
            _bookingToEdit = booking;
            _screening = screening;

            _originalTicketCount = booking.TicketCount;
            _ticketCount = booking.TicketCount.ToString();
            _customerName = booking.CustomerName;

            SaveCommand = new RelayCommand(_ => Save(), CanSave);
            BackCommand = new RelayCommand(_ => Back());
        }

        private bool CanSave(object? parameter)
        {
            if (!int.TryParse(TicketCount, out int newCount) || newCount <= 0)
            {
                return false;
            }

            int ticketsSoldExcludingThisBooking = _bookingRepository.GetTicketsSold(_screening.Id) - _originalTicketCount;
            int availableSeats = _screening.Hall.Capacity - ticketsSoldExcludingThisBooking;

            return newCount <= availableSeats;
        }

        private void Save()
        {
            _bookingToEdit.TicketCount = int.Parse(TicketCount);
            _bookingToEdit.CustomerName = CustomerName;

            _bookingRepository.Update(_bookingToEdit);

            StatusMessage = "Bookingen er opdateret.";
        }

        private void Back()
        {
            _mainWindowViewModel.CurrentView = new BookingListViewModel(_mainWindowViewModel);
        }
    }
}