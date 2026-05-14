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
using System.Windows.Shapes;

namespace Main
{
    /// <summary>
    /// Логика взаимодействия для InputDialogWindow.xaml
    /// </summary>
    public partial class InputDialogWindow : Window
    {
        public string Result { get; private set; } = "";

        public InputDialogWindow(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            Result = InputText.Text;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
