using System.Windows;

namespace Main.Views
{
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
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}