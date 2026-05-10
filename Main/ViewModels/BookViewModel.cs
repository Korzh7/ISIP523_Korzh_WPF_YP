using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Main.Models;
using Main.Views;
using Microsoft.EntityFrameworkCore;

namespace Main.ViewModels
{
    partial class BookViewModel : ObservableObject
    {
        private readonly Action<ObservableObject> _navigate;

        [ObservableProperty] private Book _book;
        [ObservableProperty] private string _reviewText = "";
        [ObservableProperty] private int _reviewRating = 5;

        public string Genres => string.Join(", ", _book.BooksGenres.Select(bg => bg.Genre.GenreName));
        public string Author => _book.User.Nickname;

        public BookViewModel(Book book, Action<ObservableObject> navigate)
        {
            _navigate = navigate;
            _book = Core.Context.Books
                .Include(b => b.User)
                .Include(b => b.BooksGenres).ThenInclude(bg => bg.Genre)
                .Include(b => b.Reviews).ThenInclude(r => r.User)
                .First(b => b.ID == book.ID);

            Core.CurrentUser = Core.Context.Users
                .Include(u => u.Role)
                .First(u => u.ID == Core.CurrentUser.ID);
        }
        [RelayCommand]
        private void GoBack() => _navigate(new CatalogViewModel(_navigate));

        [RelayCommand]
        private void AddReview()
        {
            if (string.IsNullOrWhiteSpace(ReviewText))
            { MessageBox.Show("Введите текст отзыва"); return; }

            if (Core.Context.Reviews.Any(r => r.UserId == Core.CurrentUser.ID && r.BookId == Book.ID))
            { MessageBox.Show("Вы уже оставили отзыв на эту книгу"); return; }

            var review = new Review
            {
                Text = ReviewText,
                Rating = ReviewRating,
                UserId = Core.CurrentUser.ID,
                BookId = Book.ID
            };

            Core.Context.Reviews.Add(review);
            Core.Context.SaveChanges();

            Book = Core.Context.Books
                .Include(b => b.User)
                .Include(b => b.BooksGenres).ThenInclude(bg => bg.Genre)
                .Include(b => b.Reviews).ThenInclude(r => r.User)
                .First(b => b.ID == Book.ID);

            ReviewText = "";
            ReviewRating = 5;
        }
        private string? ShowInputDialog(string message)
        {
            var dialog = new InputDialogWindow(message);
            return dialog.ShowDialog() == true ? dialog.Result : null;
        }

        [RelayCommand]
        private void ComplainBook()
        {
            var reason = ShowInputDialog("Причина жалобы на книгу:");
            if (reason == null) return;
            Core.Context.Complaints.Add(new Complaint { UserId = Core.CurrentUser.ID, BookId = Book.ID, Reason = reason });
            Core.Context.SaveChanges();
            MessageBox.Show("Жалоба отправлена");
        }

        [RelayCommand]
        private void ComplainAuthor()
        {
            var reason = ShowInputDialog("Причина жалобы на автора:");
            if (reason == null) return;
            Core.Context.Complaints.Add(new Complaint { UserId = Core.CurrentUser.ID, TargetAuthorId = Book.UserId, Reason = reason });
            Core.Context.SaveChanges();
            MessageBox.Show("Жалоба отправлена");
        }

        [RelayCommand]
        private void ComplainReview(Review review)
        {
            var reason = ShowInputDialog("Причина жалобы на отзыв:");
            if (reason == null) return;
            Core.Context.Complaints.Add(new Complaint { UserId = Core.CurrentUser.ID, ReviewId = review.ID, Reason = reason });
            Core.Context.SaveChanges();
            MessageBox.Show("Жалоба отправлена");
        }
        public bool IsAdmin => Core.CurrentUser.Role?.RoleName == "Admin";

        [RelayCommand]
        private void FreezeBook()
        {
            Book.IsFrozen = true;
            Core.Context.SaveChanges();
            MessageBox.Show("Книга заморожена");
            GoBack();
        }

        [RelayCommand]
        private void FreezeReview(Review review)
        {
            review.IsFrozen = true;
            Core.Context.SaveChanges();
            MessageBox.Show("Отзыв заморожен");

            Book = Core.Context.Books
                .Include(b => b.User)
                .Include(b => b.BooksGenres).ThenInclude(bg => bg.Genre)
                .Include(b => b.Reviews).ThenInclude(r => r.User)
                .First(b => b.ID == Book.ID);
        }
    }
}