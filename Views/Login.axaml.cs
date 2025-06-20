using Avalonia.Controls;
using Avalonia.Interactivity;
using Inv.ViewModels;
using FirebirdSql.Data.FirebirdClient;
using Inv.Views;
using MsgBox;
using System.Threading.Tasks;
using System;
using Avalonia.Controls.ApplicationLifetimes;
using Inv.ViewModels.Forms;

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
            if (!connectionSuccess)
            {
                await MessageBox.Show(this, "Неверно набран адрес базы данных, либо база данных недоступна", "Ошибка", MessageBox.MessageBoxButtons.Ok);
                return;
            }
        }

        if (!viewModel.log_In())
        {
            await MessageBox.Show(this, "Неверно набран логин или пароль", "Ошибка", MessageBox.MessageBoxButtons.Ok);
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
        if (Avalonia.Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = mainWindow;
        mainWindow.Show();

        this.Close();
    }
}