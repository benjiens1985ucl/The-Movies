using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;


namespace The_Movies.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        public ICommand RegisterMovieCommand { get; }
        public ICommand ShowMovieListCommand { get; }

        public MainMenuViewModel()
        {
            RegisterMovieCommand = new RelayCommand(_ => RegisterMovie());
            ShowMovieListCommand = new RelayCommand(_ => ShowMovieList());
        }

        private void RegisterMovie()
        {
            // Skal senere skifte "aktiv side" til registreringsformularen
        }

        private void ShowMovieList()
        {
            // Skal senere skifte "aktiv side" til filmoversigten
        }
    }
}