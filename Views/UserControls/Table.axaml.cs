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

        if (this.DataContext == null)
            _viewModel = MainWindowViewModel.tableVM;
        else
            _viewModel = this.DataContext as TableViewModel;

        InitializeComponent();

        _grid = this.FindControl<DataGrid>("MainGrid");
        if (_grid == null)
            throw new Exception("DataGrid#MainGrid not found");
        
        foreach (var column in _grid.Columns)
        {
            column.Width = _viewModel!.column_width[column.Tag as string];
        }

        _grid.SelectionChanged += _viewModel!.onSelectedRowChange;
        _grid.DoubleTapped += onDoubleClick;
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

        Global.TopLevel = true;
        Global.CurCompl = null;
        deselectRows();

        // »змен€ем отображаемые столбцы в зависимости от выбранной вкладки
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

            // »сполн€ем метод асинхронно
            Task.Run(() =>
                _viewModel.updateTableRows(new_id)
            );
        }
    }

    private void onDoubleClick(object? sender, TappedEventArgs args)
    {
        // ѕровер€ем в какой части DataGrid было произведено нажатие
        Control element = args.Source as Control;
        while (element is not DataGrid)
        {
            element = element.Parent as Control;

            // ≈сли элемент или один из его родителей €вл€етс€ DataGridRow, значит пользователь тыкнул на таблицу
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
            if (Global.RW)
                Toolbar.editItem(row);
        if (TabID == Global.RepairTab)
            Toolbar.editItem(row);
        else if (TabID == Global.JournalTab)
        {
            if (row.icon == Global.ComplectIcon)
            {
                //TODO
                return;
            }
            else 
            {
                //TODO
                return;
            }
        }
        else
        {
            if (row.icon == Global.ComplectIcon)
            {
                _viewModel.tempCollection.Clear();
                _grid.DataContext = _viewModel.tempCollection;

                // ѕотому что данные можно извлечь только из главного(UI) потока
                var tabID = TabID;
                Task.Run(() =>
                    _viewModel.updateRowsFromComplect(tabID, (int)row.id)
                );
            }
            else
                Toolbar.editItem(row);
        }

        Global.TopLevel = false;
        Global.CurCompl = row;
    }

    private TableViewModel _viewModel;
    private DataGrid _grid;
}

public class IconIndexConverter : IValueConverter
{
    public static readonly IconIndexConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter,
                                                            CultureInfo culture)
    {
        if (value is int)
        {
            switch ((int)value)
            {
                case Global.ComplectIcon:
                    return "/Assets/toolbar-icons/desktop-tower.svg";
                default:
                    return "/Assets/toolbar-icons/expansion-card-variant.svg";
            }
        }
        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(),
                                                BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType,
                                object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}