using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Inv.ViewModels;
using System;
using System.Diagnostics;
using System.IO;

namespace Inv;

public partial class Table : UserControl
{
    public static readonly StyledProperty<string> TabIDProperty =
        AvaloniaProperty.Register<Table, string>(
            nameof(TabID),
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay
        );

    public string TabID
    {
        get => GetValue(TabIDProperty);
        set => SetValue(TabIDProperty, value);
    }

    public Table()
    {
        TabIDProperty.Changed.AddClassHandler<Table>(onPageTypeChange);

        InitializeComponent();
        this.DataContext = new TableViewModel();
    }

    private void onPageTypeChange(Table sender, AvaloniaPropertyChangedEventArgs e)
    {
        var viewModel = this.DataContext as TableViewModel;

        return;

        viewModel.updateTableRows((string)e.NewValue);
    }
}