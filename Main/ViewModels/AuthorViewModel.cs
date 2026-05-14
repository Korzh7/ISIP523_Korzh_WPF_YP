using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Main.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.ViewModels
{
    partial class AuthorViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Book> _books = new();
        [ObservableProperty] private ObservableCollection<Book> _frozenBooks = new();
        [ObservableProperty] private bool _showAddForm;
        [ObservableProperty] private Book? _editingBook;

        [ObservableProperty] private string _bookName = "";
        [ObservableProperty] private string _description = "";
        [ObservableProperty] private string _imageUrl = "";
        [ObservableProperty] private string _text = "";

        public AuthorViewModel()
        {
            LoadBooks();
        }
        
        /// <summary>
        /// Загружает список книг из БД с учётом текущего поиска, жанра и сортировки
        /// </summary>
        private void LoadBooks()
        {
            var all = Core.Context.Books
                .Include(b => b.BooksGenres).ThenInclude(bg => bg.Genre)
                .Include(b => b.Reviews)
                .Where(b => b.UserId == Core.CurrentUser.ID)
                .ToList();

            Books = new ObservableCollection<Book>(all.Where(b => !b.IsFrozen));
            FrozenBooks = new ObservableCollection<Book>(all.Where(b => b.IsFrozen));
        }

        [RelayCommand]
        private void ShowAdd()
        {
            EditingBook = null;
            BookName = ""; Description = ""; ImageUrl = ""; Text = "";
            ShowAddForm = true;
        }

        [RelayCommand]
        private void EditBook(Book book)
        {
            EditingBook = book;
            BookName = book.BookName;
            Description = book.Description;
            ImageUrl = book.ImageURL;
            Text = book.Text;
            ShowAddForm = true;
        }

        [RelayCommand]
        private void SaveBook()
        {
            if (string.IsNullOrWhiteSpace(BookName) || string.IsNullOrWhiteSpace(Description)
                || string.IsNullOrWhiteSpace(ImageUrl) || string.IsNullOrWhiteSpace(Text))
            { MessageBox.Show("Заполните все поля"); return; }

            if (EditingBook == null)
            {
                Core.Context.Books.Add(new Book
                {
                    BookName = BookName,
                    Description = Description,
                    ImageURL = ImageUrl,
                    Text = Text,
                    UserId = Core.CurrentUser.ID
                });
            }
            else
            {
                EditingBook.BookName = BookName;
                EditingBook.Description = Description;
                EditingBook.ImageURL = ImageUrl;
                EditingBook.Text = Text;
            }

            Core.Context.SaveChanges();
            ShowAddForm = false;
            LoadBooks();
            MessageBox.Show(EditingBook == null ? "Книга добавлена!" : "Книга обновлена!");
        }

        [RelayCommand]
        private void CancelEdit() => ShowAddForm = false;

        [RelayCommand]
        private void AppealFreeze(Book book)
        {
            var dialog = new InputDialogWindow("Причина оспаривания заморозки книги:");
            if (dialog.ShowDialog() != true) return;

            Core.Context.UnfreezeApplications.Add(new UnfreezeApplication
            {
                UserId = Core.CurrentUser.ID,
                TargetType = "Book",
                TargetBookId = book.ID,
                Reason = dialog.Result
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Заявка отправлена!");
        }
    }
}