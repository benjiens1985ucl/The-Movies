namespace The_Movies.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public MainWindowViewModel()
        {
            CurrentView = new MainMenuViewModel();
        }
    }
}