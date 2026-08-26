using System.Windows.Input;
using The_Movies.Models;

namespace The_Movies.ViewModels
{
    public class RegisterMovieViewModel : ViewModelBase
    {
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public IEnumerable<Genre> GenreValues => Enum.GetValues<Genre>();

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

        public ICommand SaveCommand { get; }

        public RegisterMovieViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save());
        }

        private void Save()
        {
            var movie = new Movie
            {
                Title = Title,
                Duration = Duration,
                Genre = Genre
            };

            
        }
    }
}