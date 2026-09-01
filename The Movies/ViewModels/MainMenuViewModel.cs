using System.Windows.Input;

namespace The_Movies.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public ICommand RegisterMovieCommand { get; }
        public ICommand ShowMovieListCommand { get; }
        public ICommand ShowProgramCommand { get; }
        public ICommand ShowProgramOverviewCommand { get; }

        public MainMenuViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            RegisterMovieCommand = new RelayCommand(_ => RegisterMovie());
            ShowMovieListCommand = new RelayCommand(_ => ShowMovieList());
            ShowProgramCommand = new RelayCommand(_ => ShowProgram());
            ShowProgramOverviewCommand =
                new RelayCommand(_ => ShowProgramOverview());
        }

        private void RegisterMovie()
        {
            System.Diagnostics.Debug.WriteLine("RegisterMovie blev kaldt!");
            _mainWindowViewModel.CurrentView = new RegisterMovieViewModel(_mainWindowViewModel);
        }

        private void ShowMovieList()
        {
            _mainWindowViewModel.CurrentView = new MovieListViewModel(_mainWindowViewModel);
        }
        private void ShowProgram()
        {
            _mainWindowViewModel.CurrentView = new ProgramViewModel(_mainWindowViewModel);
        }

        private void ShowProgramOverview()
        {
            _mainWindowViewModel.CurrentView =
                new ProgramOverviewViewModel(_mainWindowViewModel);
        }
    }
}