using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Main.Models;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace Main.ViewModels
{
    partial class CatalogViewModel : ObservableObject
    {
        private readonly Action<ObservableObject> _navigate;

        public CatalogViewModel(Action<ObservableObject> navigate)
        {
            _navigate = navigate;
            Genres = new ObservableCollection<Genre>(Core.Context.Genres.ToList());
            LoadBooks();
        }

        [RelayCommand]
        private void OpenBook(Book book) => _navigate(new BookViewModel(book, _navigate));

        [ObservableProperty] private ObservableCollection<Book> _books = new();
        [ObservableProperty] private ObservableCollection<Genre> _genres = new();
        [ObservableProperty] private Genre? _selectedGenre;
        [ObservableProperty] private string _search = "";
        [ObservableProperty] private string _sortBy = "Название";

        public List<string> SortOptions => new() { "Название", "Рейтинг" };

        public CatalogViewModel()
        {
            Genres = new ObservableCollection<Genre>(Core.Context.Genres.ToList());
            LoadBooks();
        }
        [RelayCommand]
        private void ResetGenre()
        {
            _selectedGenre = null;
            OnPropertyChanged(nameof(SelectedGenre));
            LoadBooks();
        }

       
       
        partial void OnSearchChanged(string value) => LoadBooks();
        partial void OnSelectedGenreChanged(Genre? value) => LoadBooks();
        partial void OnSortByChanged(string value) => LoadBooks();

        private void LoadBooks()
        {
            var query = Core.Context.Books
                .Include(b => b.User)
                .Include(b => b.BooksGenres).ThenInclude(bg => bg.Genre)
                .Include(b => b.Reviews)
                .Where(b => !b.IsFrozen)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(_search))
                query = query.Where(b => b.BookName.Contains(_search) ||
                                         b.User.Nickname.Contains(_search));

            if (_selectedGenre != null)
                query = query.Where(b => b.BooksGenres.Any(bg => bg.GenreId == _selectedGenre.ID));

            var list = query.ToList();

            list = _sortBy == "Рейтинг"
                ? list.OrderByDescending(b => b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0).ToList()
                : list.OrderBy(b => b.BookName).ToList();

            Books = new ObservableCollection<Book>(list);
        }
        [RelayCommand]
        private void AddToList(Book book)
        {
            var exists = Core.Context.ReadingLists
                .Any(rl => rl.UserId == Core.CurrentUser.ID && rl.BookId == book.ID);

            if (exists) { MessageBox.Show("Книга уже в списке"); return; }

            Core.Context.ReadingLists.Add(new ReadingList
            {
                UserId = Core.CurrentUser.ID,
                BookId = book.ID,
                Status = "Planned"
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Книга добавлена в «В планах»");
        }
    }
}