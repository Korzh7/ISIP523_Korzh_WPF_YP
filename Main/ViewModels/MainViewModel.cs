using CommunityToolkit.Mvvm.ComponentModel;

namespace Main.ViewModels
{
    partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject? _currentPage;

        public MainViewModel()
        {
            CurrentPage = new AuthViewModel(Navigate);
        }

        public void Navigate(ObservableObject page)
        {
            CurrentPage = page;
        }
    }
}