using Avalonia.Controls;
using Avalonia.Interactivity;
using Inv.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Inv.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowViewModel();

        var viewModel = this.DataContext as MainWindowViewModel;
        var tabControl = this.FindControl<TabControl>("Tabs");

        // —оздаем массив значений дл€ отрисовки табов
        if (tabControl != null && viewModel != null)
        {
            var tabItems = new TabItemViewModel[viewModel.TabHeaders.Length];

            for (int i = 0; i < viewModel.TabHeaders.Length; i++)
            {
                tabItems[i] = new TabItemViewModel(
                    viewModel.TabHeaders[i],
                    viewModel.TabHeaders[i]
                );
            }

            tabControl.DataContext = tabItems;
        }
    }

    // ≈сли пользователь кликает на другой таб
    public void TabChangeHandler(object sender, SelectionChangedEventArgs e)
    {
        var viewModel = this.DataContext as MainWindowViewModel;
        var tabControl = sender as TabControl;

        if (tabControl == null || viewModel == null) return;
        viewModel.SelectedIndex = tabControl.SelectedIndex;

        //tabControl.FindControl<>

    }
}