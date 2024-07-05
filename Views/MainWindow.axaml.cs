using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Inv.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Inv.Views;

public partial class MainWindow : Window
{
    public static int? SelectedItemID { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowViewModel();

        var viewModel = this.DataContext as MainWindowViewModel;
        this.FindControl<TabControl>("TabControl").DataContext = viewModel.Tabs;
    }

    // ≈сли пользователь кликает на другую вкладку
    public void TabChangeHandler(object sender, SelectionChangedEventArgs e)
    {
        var viewModel = this.DataContext as MainWindowViewModel;
        var tabControl = sender as TabControl;

        if (tabControl == null || viewModel == null) return;
        viewModel.SelectedIndex = tabControl.SelectedIndex;
    }
}