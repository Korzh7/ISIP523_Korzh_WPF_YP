using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Main.Models;
using System.Windows;

namespace Main.ViewModels
{
    partial class FrozenViewModel : ObservableObject
    {
        public string Message => "Ваш аккаунт заморожен. Обратитесь к администратору.";
        public bool CanAppeal => !Core.Context.UnfreezeApplications
            .Any(u => u.UserId == Core.CurrentUser.ID &&
                      u.TargetType == "Account" &&
                      u.Status == "Pending");

        [RelayCommand]
        private void AppealFreeze()
        {
            var dialog = new InputDialogWindow("Причина оспаривания заморозки:");
            if (dialog.ShowDialog() != true) return;

            Core.Context.UnfreezeApplications.Add(new UnfreezeApplication
            {
                UserId = Core.CurrentUser.ID,
                TargetType = "Account",
                Reason = dialog.Result
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Заявка отправлена!");
        }
    }
}