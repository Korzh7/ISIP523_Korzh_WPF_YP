using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Main.Models;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace Main.ViewModels
{
    partial class ListsViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<ReadingList> _books = new();
        [ObservableProperty] private ObservableCollection<Genre> _genres = new();
        [ObservableProperty] private Genre? _selectedGenre;
        [ObservableProperty] private string _search = "";
        [ObservableProperty] private string _sortBy = "Name";
        [ObservableProperty] private string _currentStatus = "Reading";

        public List<string> SortOptions => new() { "Name", "Rating" };
        public List<string> Statuses => new() { "Reading", "Planned", "Abandoned", "Read" };
        public Dictionary<string, string> StatusNames => new()
        {
            { "Reading", "Читаю" },
            { "Planned", "В планах" },
            { "Abandoned", "Заброшено" },
            { "Read", "Прочитано" }
        };

        public ListsViewModel()
        {
            Genres = new ObservableCollection<Genre>(Core.Context.Genres.ToList());
            LoadBooks();
        }

        partial void OnSearchChanged(string value) => LoadBooks();
        partial void OnSelectedGenreChanged(Genre? value) => LoadBooks();
        partial void OnSortByChanged(string value) => LoadBooks();
        partial void OnCurrentStatusChanged(string value) => LoadBooks();

        private void LoadBooks()
        {
            var query = Core.Context.ReadingLists
                .Include(rl => rl.Book).ThenInclude(b => b.User)
                .Include(rl => rl.Book).ThenInclude(b => b.Reviews)
                .Include(rl => rl.Book).ThenInclude(b => b.BooksGenres).ThenInclude(bg => bg.Genre)
                .Where(rl => rl.UserId == Core.CurrentUser.ID && rl.Status == _currentStatus)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(_search))
                query = query.Where(rl => rl.Book.BookName.Contains(_search) ||
                                          rl.Book.User.Nickname.Contains(_search));

            if (_selectedGenre != null)
                query = query.Where(rl => rl.Book.BooksGenres.Any(bg => bg.GenreId == _selectedGenre.ID));

            var list = query.ToList();

            list = _sortBy == "Rating"
                ? list.OrderByDescending(rl => rl.Book.Reviews.Any() ? rl.Book.Reviews.Average(r => r.Rating) : 0).ToList()
                : list.OrderBy(rl => rl.Book.BookName).ToList();

            Books = new ObservableCollection<ReadingList>(list);
        }

        [RelayCommand]
        private void ChangeStatus(ReadingList item)
        {
        }

        [RelayCommand]
        private void ResetGenre()
        {
            _selectedGenre = null;
            OnPropertyChanged(nameof(SelectedGenre));
            LoadBooks();
        }

        [RelayCommand]
        private void MoveBook(ReadingList item)
        {
            Core.Context.SaveChanges();
            LoadBooks();
        }

        [RelayCommand]
        private void SwitchStatus(string status)
        {
            CurrentStatus = status;
        }
    }
}