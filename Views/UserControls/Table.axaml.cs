using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Inv.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Inv.Views;
using Avalonia.Interactivity;

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

        foreach (var column in _grid.Columns)
        {
            column.Width = _viewModel.column_width[column.Tag as string];
        }
    }

    public void onSelectedRowChange(object sender, SelectionChangedEventArgs e)
    {
        var _selected = ((DataGrid)sender).SelectedItem as TableRow;

        if (_selected != null)
            MainWindow.SelectedItemID = _selected.compl_num as int?;
        else
            MainWindow.SelectedItemID = null;
    }

    private void onPageTypeChange(Table sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue == null)
            throw new Exception("PageType is null");

        var new_id = (string)e.NewValue;
        var _grid = this.FindControl<DataGrid>("MainGrid");
        if (_grid == null)
            throw new Exception("DataGrid#MainGrid not found");

        _viewModel.Compl_id = null;

        // Изменяем отображаемые столбцы в зависимости от выбранной вкладки
        if (new_id == Global.RepairTab)
            foreach (var column in _grid.Columns)
                column.IsVisible = _viewModel.remont_column_visibility[column.Tag as string];
        else if (new_id == Global.JournalTab)
            foreach (var column in _grid.Columns)
                column.IsVisible = _viewModel.journal_column_visibility[column.Tag as string];
        else
            foreach (var column in _grid.Columns)
                column.IsVisible = _viewModel.sklad_column_visibility[column.Tag as string];

        if (_viewModel.cachedCollections.ContainsKey(new_id))
            _grid.DataContext = _viewModel.cachedCollections[new_id];
        else
        {
            _viewModel.cachedCollections.Add(new_id, new ObservableCollection<TableRow>());
            _grid.DataContext = _viewModel.cachedCollections[new_id];

            // Исполняем метод асинхронно
            Task.Run(() =>
                _viewModel.updateTableRows(new_id)
            );
        }
    }
}