using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            LoadVisibility();
            ContentFrame.Navigate(new CatalogPage()); 
        }

        private void LoadVisibility()
        {
            var user = Core.CurrentUser;
            if (user == null) return;
            if (user.Role == null)
                user = Core.Context.Users.Include(u => u.Role).First(u => u.ID == user.ID);

            BtnAuthor.Visibility = user.Role?.RoleName == "Author" ? Visibility.Visible : Visibility.Collapsed;
            BtnAdmin.Visibility = user.Role?.RoleName == "Admin" ? Visibility.Visible : Visibility.Collapsed;
            BtnFrozen.Visibility = user.IsFrozen ? Visibility.Visible : Visibility.Collapsed;
        }

        
        private void Catalog_Click(object sender, RoutedEventArgs e) => ContentFrame.Navigate(new CatalogPage());
        private void Lists_Click(object sender, RoutedEventArgs e) => ContentFrame.Navigate(new ListsPage());
        private void Profile_Click(object sender, RoutedEventArgs e) => ContentFrame.Navigate(new ProfilePage());
        private void Author_Click(object sender, RoutedEventArgs e) => ContentFrame.Navigate(new AuthorPage());
        private void Admin_Click(object sender, RoutedEventArgs e) => ContentFrame.Navigate(new AdminPage());
        private void Frozen_Click(object sender, RoutedEventArgs e) => ContentFrame.Navigate(new FrozenPage());
    }
}
