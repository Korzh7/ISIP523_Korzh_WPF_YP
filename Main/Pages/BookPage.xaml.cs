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
using Main.Models;
using Main.ViewModels;

namespace Main.Pages
{
    /// <summary>
    /// Логика взаимодействия для BookPage.xaml
    /// </summary>
    public partial class BookPage : Page
    {
        public BookPage(Book book)
        {
            InitializeComponent();
            var vm = new BookViewModel(book);
            vm.GoBackRequested += () => NavigationService.GoBack(); 
            DataContext = vm;
        }
        void BackButton_Click(object sender, RoutedEventArgs e) => NavigationService?.GoBack();
        
    }
}
