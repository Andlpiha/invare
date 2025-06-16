using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Inv.Views;
using Avalonia.Interactivity;
using Inv.ViewModels.MainWindow;
using Avalonia.Input;
using Avalonia.LogicalTree;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Data.Converters;
using Avalonia.Data;
using System.Globalization;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using DynamicData;
using Avalonia.Controls.Metadata;

namespace Inv;

public partial class Table : UserControl
{
    public static readonly StyledProperty<string> TabIDProperty =
        AvaloniaProperty.Register<Table, string>(
            nameof(TabID),
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay
        );
    public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<Table, SelectionChangedEventArgs>(
            nameof(SelectionChanged),
            RoutingStrategies.Bubble
        );

    private TableViewModel _viewModel;
    private DataGrid _grid;

    public string TabID
    {
        get => GetValue(TabIDProperty);
        set => SetValue(TabIDProperty, value);
    }

    public event EventHandler<SelectionChangedEventArgs> SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }
    protected virtual void OnGridSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        SelectionChangedEventArgs new_args = new(SelectionChangedEvent, args.RemovedItems, args.AddedItems);
        RaiseEvent(new_args);
    }   

    public Table()
    {
        TabIDProperty.Changed.AddClassHandler<Table>(onPageTypeChange);

        if (this.DataContext == null)
            _viewModel = MainWindowViewModel.tableVM;
        else
            _viewModel = this.DataContext as TableViewModel;

        InitializeComponent();

        _grid = this.FindControl<DataGrid>("MainGrid")!;
        if (_grid == null)
            throw new Exception("DataGrid#MainGrid not found");
        _grid.DataContext = _viewModel!.item_source;

        foreach (var column in _grid.Columns)
        {
            column.Width = _viewModel!.column_width[column.Tag as string];
        }

        _grid.SelectionChanged += _viewModel!.onSelectedRowChange;
        _grid.DoubleTapped += onDoubleClick;
        _grid.SelectionChanged += OnGridSelectionChanged;
    }
    public void deselectRows()
    {
        _grid.SelectedIndex = -1;
        _grid.SelectedItem = null;
    }

    private void onPageTypeChange(Table sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue == null)
            throw new Exception("PageType is null");

        var new_id = (string)args.NewValue;
        RenderNewPage(new_id);
    }

    public void RenderNewPage(string new_id)
    {
        var _grid = this.FindControl<DataGrid>("MainGrid");
        if (_grid == null)
            throw new Exception("DataGrid#MainGrid not found");

        if (!Global.TopLevel)
        {
            _grid.DataContext = _viewModel!.item_source;
            Global.TopLevel = true;
            Global.ComplUp = null;
        }

        deselectRows();

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

        _viewModel.CurrentTabId = new_id;
        if (!_viewModel.addedIds.Contains(new_id))
        {
            _viewModel.addedIds.Add(new_id);

            // Исполняем метод асинхронно
            Task.Run(() =>
                _viewModel.updateTableRows(new_id)
            );
        }
    }

    private void onDoubleClick(object? sender, TappedEventArgs args)
    {
        // Проверяем в какой части DataGrid было произведено нажатие
        Control element = args.Source as Control;
        while (element is not DataGrid)
        {
            element = element.Parent as Control;

            // Если элемент или один из его родителей является DataGridRow, значит пользователь тыкнул на таблицу
            // в противном случае пользователь тыкнул на header
            if (element is DataGridRow)
            {
                var row = (DataGridRow)element;
                handleRowDoubleClick(row.DataContext as TableRow);

                break;
            }
        }
    }

    private void handleRowDoubleClick(TableRow row)
    {
        if (!Global.TopLevel)
        {
            if (Global.RW)
                Toolbar.editItem(row, TabID);
        }
        else if (TabID == Global.RepairTab || TabID == Global.JournalTab)
        {
            Toolbar.editItem(row, TabID);
        }
        else
        {
            if (row.icon == Global.ComplectIcon)
            {
                // Потому что данные можно извлечь только из главного(UI) потока
                var tabID = TabID;
                Task.Run(() =>
                    _viewModel.updateRowsFromComplect(tabID, (int)row.id)
                );
                _grid.DataContext = _viewModel.temp_collection;

                Global.TopLevel = false;
                Global.ComplUp = row;
            }
            else
                Toolbar.editItem(row, TabID);
        }
    }
}