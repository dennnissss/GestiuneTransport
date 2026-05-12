using System.Windows;
using GestiuneTransport.Models;

namespace GestiuneTransport.WpfApp;

public partial class CursaDetailsWindow : Window
{
    public CursaDetailsWindow(Cursa cursa)
    {
        InitializeComponent();
        DataContext = cursa;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
