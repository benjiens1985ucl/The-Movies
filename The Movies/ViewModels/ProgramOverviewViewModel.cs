using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
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

        public ProgramOverviewViewModel(
            MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            var screenings = _screeningRepository
                .LoadAll()
                .OrderBy(s => s.DateTime);

            Screenings = new ObservableCollection<Screening>(screenings);

            BackCommand = new RelayCommand(_ => Back());

        }
        
        private void Back()
        {
            _mainWindowViewModel.CurrentView =
                new MainMenuViewModel(_mainWindowViewModel);
        }
    }
}
