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
        this.DataContext = viewModel;
        _ = viewModel.dbConnect(); // Не ждем результат
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

        if (!viewModel.log_In())
        {
            await MessageBox.Show(this, "Логин не существует", "Ошибка", MessageBox.MessageBoxButtons.Ok);
            return;
        }

        // Сохраняем значения полей при успешном входе
        if (DataContext != null)
        {
            Properties.Login.Default.ServerLocation = viewModel.ServerLocation;
            Properties.Login.Default.DatabaseFile = viewModel.DatabaseFile;
            Properties.Login.Default.UserLogin = viewModel.Login;
            Properties.Login.Default.Save();
        }

        MainWindow mainWindow = new();
        mainWindow.Show();

        this.Close();
    }
}