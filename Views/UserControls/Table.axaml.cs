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

    private TableViewModel _viewModel;

    public Table()
    {
        TabIDProperty.Changed.AddClassHandler<Table>(onPageTypeChange);
        _viewModel = new TableViewModel();

        InitializeComponent();

        var _grid = this.FindControl<DataGrid>("MainGrid");
        if (_grid == null)
            throw new Exception("DataGrid#MainGrid not found");

        TableWidth width = new();
        _grid.Resources.Add("TableWidth", width);
        _grid.DataContext = _viewModel.TableRows;
    }

    public void onSelectedRowChange(object sender, SelectionChangedEventArgs e)
    {
        var _selected = ((DataGrid)sender).SelectedItem as TableRow;
        _viewModel.Compl_id = (int)_selected.compl_num;
    }

    private void onPageTypeChange(Table sender, AvaloniaPropertyChangedEventArgs e)
    {
        _viewModel.updateTableRows((string)e.NewValue);
    }
}