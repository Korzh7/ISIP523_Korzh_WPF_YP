using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Main.ViewModels
{
    partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableObject? _currentSection;
        [ObservableProperty] private Visibility _isAdmin = Visibility.Collapsed;
        [ObservableProperty] private Visibility _isAuthor = Visibility.Collapsed;
        [ObservableProperty] private Visibility _isFrozen = Visibility.Collapsed;

        public MainPageViewModel()
        {
            LoadUserState();
            CurrentSection = new CatalogViewModel(NavigateSection);
        }

        private void LoadUserState()
        {
            var user = Core.CurrentUser;
            IsAdmin = user.Role?.RoleName == "Admin" ? Visibility.Visible : Visibility.Collapsed;
            IsAuthor = user.Role?.RoleName == "Author" ? Visibility.Visible : Visibility.Collapsed;
            IsFrozen = user.IsFrozen ? Visibility.Visible : Visibility.Collapsed;
        }

        [RelayCommand]
        private void Navigate(string section)
        {
            CurrentSection = section switch
            {
                "Catalog" => new CatalogViewModel(NavigateSection),
                "Lists" => new ListsViewModel(),
                "Profile" => new ProfileViewModel(),
                "Admin" => new AdminViewModel(),
                "Author" => new AuthorViewModel(),
                "Frozen" => new FrozenViewModel(),
                _ => CurrentSection
            };
        }

        private void NavigateSection(ObservableObject page) => CurrentSection = page;
    }
}