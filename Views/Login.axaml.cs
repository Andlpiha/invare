using Avalonia.Controls;
using Avalonia.Interactivity;
using Inv.ViewModels;
using FirebirdSql.Data.FirebirdClient;
using Inv.Views;
using MsgBox;
using System.Threading.Tasks;
using System;

namespace Inv;

public partial class LoginWindow : Window
{
    private LoginViewModel viewModel;
    public LoginWindow()
    {
        InitializeComponent();

        viewModel = new LoginViewModel();
        DataContext = viewModel;
        viewModel.dbConnect();
    }

    public void Exit(object sender, RoutedEventArgs args)
    {
        System.Environment.Exit(0);
    }
    
    public async void Ok_btnClick(object sender, RoutedEventArgs args)
    {
        if (DataContext == null) return;

        if (SQLConn.Instance.GetConnection().State != System.Data.ConnectionState.Open)
        {
            bool connectionSuccess = await viewModel.dbConnect();
            if (!connectionSuccess) return;
        }

        // Сохраняем значения полей при успешном входе
        if (DataContext != null)
        {
            Properties.Login.Default.DBLocation = viewModel.DBLocation;
            Properties.Login.Default.Name = viewModel.Login;
            Properties.Login.Default.Save();
        }

        MainWindow mainWindow = new();
        mainWindow.Show();

        this.Close();
    }
}