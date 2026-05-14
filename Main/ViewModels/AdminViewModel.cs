using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Main.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Main.ViewModels
{
    partial class AdminViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Complaint> _complaints = new();
        [ObservableProperty] private ObservableCollection<UnfreezeApplication> _unfreezeApplications = new();
        [ObservableProperty] private ObservableCollection<RoleApplication> _roleApplications = new();
        [ObservableProperty] private ObservableCollection<Book> _frozenBooks = new();
        [ObservableProperty] private ObservableCollection<User> _frozenUsers = new();
        [ObservableProperty] private ObservableCollection<Review> _frozenReviews = new();
        [ObservableProperty] private ObservableCollection<User> _users = new();
        [ObservableProperty] private string _currentTab = "Complaints";

        public AdminViewModel()
        {
            LoadAll();
        }

        private void LoadAll()
        {
            Complaints = new ObservableCollection<Complaint>(
                Core.Context.Complaints
                    .Include(c => c.User)
                    .Include(c => c.Book)
                    .Include(c => c.Review)
                    .Include(c => c.TargetAuthor)
                    .ToList());

            UnfreezeApplications = new ObservableCollection<UnfreezeApplication>(
                Core.Context.UnfreezeApplications
                    .Include(u => u.User)
                    .Include(u => u.TargetBook)
                    .Where(u => u.Status == "Pending")
                    .ToList());

            RoleApplications = new ObservableCollection<RoleApplication>(
                Core.Context.RoleApplications
                    .Include(r => r.User)
                    .Where(r => r.Status == "Pending")
                    .ToList());

            FrozenBooks = new ObservableCollection<Book>(
                Core.Context.Books.Where(b => b.IsFrozen).ToList());

            FrozenUsers = new ObservableCollection<User>(
                Core.Context.Users.Where(u => u.IsFrozen).ToList());

            FrozenReviews = new ObservableCollection<Review>(
                Core.Context.Reviews
                    .Include(r => r.User)
                    .Include(r => r.Book)
                    .Where(r => r.IsFrozen).ToList());

            Users = new ObservableCollection<User>(
                Core.Context.Users.Include(u => u.Role).ToList());
        }

        [RelayCommand]
        private void SwitchTab(string tab) => CurrentTab = tab;

        [RelayCommand]
        private void AcceptComplaint(Complaint c)
        {
            if (c.BookId != null)
            {
                var book = Core.Context.Books.Find(c.BookId);
                if (book != null) book.IsFrozen = true;
            }
            else if (c.ReviewId != null)
            {
                var review = Core.Context.Reviews.Find(c.ReviewId);
                if (review != null) review.IsFrozen = true;
            }
            else if (c.TargetAuthorId != null)
            {
                var user = Core.Context.Users.Find(c.TargetAuthorId);
                if (user != null) user.IsFrozen = true;
            }
            Core.Context.Complaints.Remove(c);
            Core.Context.SaveChanges();
            LoadAll();
        }

        [RelayCommand]
        private void RejectComplaint(Complaint c)
        {
            Core.Context.Complaints.Remove(c);
            Core.Context.SaveChanges();
            LoadAll();
        }

        [RelayCommand]
        private void AcceptUnfreeze(UnfreezeApplication app)
        {
            if (app.TargetType == "Account")
            {
                var user = Core.Context.Users.Find(app.UserId);
                if (user != null) user.IsFrozen = false;
            }
            else if (app.TargetType == "Book" && app.TargetBookId != null)
            {
                var book = Core.Context.Books.Find(app.TargetBookId);
                if (book != null) book.IsFrozen = false;
            }
            app.Status = "Accepted";
            Core.Context.SaveChanges();
            LoadAll();
        }

        [RelayCommand]
        private void RejectUnfreeze(UnfreezeApplication app)
        {
            app.Status = "Rejected";
            Core.Context.SaveChanges();
            LoadAll();
        }

        [RelayCommand]
        private void AcceptRoleApplication(RoleApplication app)
        {
            var user = Core.Context.Users.Find(app.UserId);
            if (user != null)
            {
                var authorRole = Core.Context.Roles.FirstOrDefault(r => r.RoleName == "Author");
                if (authorRole != null) user.RoleId = authorRole.ID;
            }
            app.Status = "Accepted";
            Core.Context.SaveChanges();
            LoadAll();
        }

        [RelayCommand]
        private void RejectRoleApplication(RoleApplication app)
        {
            app.Status = "Rejected";
            Core.Context.SaveChanges();
            LoadAll();
        }

        [RelayCommand]
        private void ChangePassword(User user)
        {
            var dialog = new InputDialogWindow("Новый пароль:");
            if (dialog.ShowDialog() != true) return;
            user.Password = dialog.Result;
            Core.Context.SaveChanges();
            MessageBox.Show("Пароль изменён!");
        }

        [RelayCommand]
        private void AssignRole(User user)
        {
            var roles = Core.Context.Roles.ToList();
            var current = roles.FindIndex(r => r.ID == user.RoleId);
            var next = roles[(current + 1) % roles.Count];
            user.RoleId = next.ID;
            Core.Context.SaveChanges();
            LoadAll();
            MessageBox.Show($"Роль изменена на {next.RoleName}");
        }
    }
}