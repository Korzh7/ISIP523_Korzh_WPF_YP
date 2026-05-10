using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Main.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.ViewModels
{
    partial class AuthViewModel : ObservableObject
    {
        private readonly Action<ObservableObject> _navigate;

        public AuthViewModel(Action<ObservableObject> navigate)
        {
            _navigate = navigate;
        }
        [ObservableProperty] private bool _isLoginMode = true;
        [ObservableProperty] private string _login = "";
        [ObservableProperty] private string _password = "";
        [ObservableProperty] private string _email = "";
        [ObservableProperty] private string _nickname = "";

        [RelayCommand]
        private void SwitchMode(string isLogin) => IsLoginMode = isLogin == "True";

        [RelayCommand]
        private void Submit()
        {
            if (IsLoginMode) TryLogin();
            else TryRegister();
        }

        private void TryLogin()
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            { MessageBox.Show("Заполните все поля"); return; }

            var user = Core.Context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == Login && u.Password == Password);

            if (user == null) { MessageBox.Show("Неверный логин или пароль"); return; }

            Core.CurrentUser = user;
            _navigate(new MainPageViewModel());
        }

        private void TryRegister()
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password)
                || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Nickname))
            { MessageBox.Show("Заполните все поля"); return; }

            if (!Email.Contains("@"))
            { MessageBox.Show("Некорректный email"); return; }

            if (Password.Length < 6)
            { MessageBox.Show("Пароль минимум 6 символов"); return; }

            if (Core.Context.Users.Any(u => u.Login == Login))
            { MessageBox.Show("Логин занят"); return; }

            if (Core.Context.Users.Any(u => u.Email == Email))
            { MessageBox.Show("Email занят"); return; }

            if (Core.Context.Users.Any(u => u.Nickname == Nickname))
            { MessageBox.Show("Никнейм занят"); return; }

            var user = new User
            {
                Login = Login,
                Password = Password,
                Email = Email,
                Nickname = Nickname,
                RoleId = 1
            };

            Core.Context.Users.Add(user);
            Core.Context.SaveChanges();
            MessageBox.Show("Регистрация успешна!");
            IsLoginMode = true;
        }
    }
}
