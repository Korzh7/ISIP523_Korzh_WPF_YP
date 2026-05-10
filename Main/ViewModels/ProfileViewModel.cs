using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Main.Models;
using Main.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Main.ViewModels
{
    partial class ProfileViewModel : ObservableObject
    {
        [ObservableProperty] private User _user;
        [ObservableProperty] private ObservableCollection<Review> _reviews = new();
        [ObservableProperty] private bool _canApplyForAuthor;

        public bool IsFrozen => _user.IsFrozen;
        public string RoleName => _user.Role?.RoleName ?? "Нет роли";

        public ProfileViewModel()
        {
            User = Core.Context.Users
                .Include(u => u.Role)
                .Include(u => u.Reviews).ThenInclude(r => r.Book)
                .Include(u => u.RoleApplications)
                .First(u => u.ID == Core.CurrentUser.ID);

            Core.CurrentUser = User;

            Reviews = new ObservableCollection<Review>(User.Reviews);

            CanApplyForAuthor = User.Role?.RoleName == "Reader" &&
                !User.RoleApplications.Any(a => a.Status == "Pending");
        }

        [RelayCommand]
        private void ApplyForAuthor()
        {
            Core.Context.RoleApplications.Add(new RoleApplication { UserId = User.ID });
            Core.Context.SaveChanges();
            CanApplyForAuthor = false;
            MessageBox.Show("Заявка отправлена!");
        }

        [RelayCommand]
        private void AppealFreeze()
        {
            var dialog = new InputDialogWindow("Причина оспаривания заморозки:");
            if (dialog.ShowDialog() != true) return;

            Core.Context.UnfreezeApplications.Add(new UnfreezeApplication
            {
                UserId = User.ID,
                TargetType = "Account",
                Reason = dialog.Result
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Заявка на снятие заморозки отправлена!");
        }
    }
}