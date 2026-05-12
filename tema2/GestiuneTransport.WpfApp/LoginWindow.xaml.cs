using System.Windows;
using System.Windows.Input;
using GestiuneTransport.StocareDate;

namespace GestiuneTransport.WpfApp;

public partial class LoginWindow : Window
{
    private readonly UtilizatorFileRepository _utilizatorRepository = new();

    public LoginWindow()
    {
        InitializeComponent();
        LoginButton.Focus();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        Autentifica();
    }

    private void LoginField_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Autentifica();
        }
    }

    private void Autentifica()
    {
        string username = UsernameTextBox.Text.Trim();
        string parola = PasswordBox.Password;

        if (_utilizatorRepository.ValideazaAutentificare(username, parola))
        {
            var mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Closed += (_, _) => Close();
            mainWindow.Show();
            Hide();
            return;
        }

        LoginMessageTextBlock.Text = "Datele de autentificare nu sunt valide. Cont demo: admin / admin123.";
        LoginMessagePanel.Visibility = Visibility.Visible;
    }
}
