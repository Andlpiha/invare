using Avalonia.Controls;
using Avalonia.Interactivity;
using Inv.ViewModels;

namespace Inv;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        this.DataContext = new LoginViewModel();
        InitializeComponent();
    }

    public void Exit(object sender, RoutedEventArgs args)
    {
        System.Environment.Exit(0);
    }
    
    public void Ok_btnClick(object sender, RoutedEventArgs args)
    {
        // Сохраняем значения полей при успешном входе
        if (DataContext != null)
        {
            Properties.Login.Default.DBLocation = ((LoginViewModel)this.DataContext).DBLocation;
            Properties.Login.Default.Name = ((LoginViewModel)this.DataContext).Login;
            Properties.Login.Default.Save();
        }
    }
}