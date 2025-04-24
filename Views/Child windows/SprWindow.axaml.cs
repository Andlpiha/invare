using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Inv.ViewModels;
using Inv.ViewModels.MainWindow;
using ReactiveUI;

namespace Inv;

public partial class SprWindow : Window
{
    // Edit - справочник был вызван из основного окна, не возврщает значение
    // Get - справочник был вызван из окна создания/редактирования, возвращает выбранное в таблице значение
    private string _spr_type;

    private TreeDataGrid _dataGrid;
    private static SprViewModel _sprViewModel;
    
    private object? _selectedRow;

    public SprWindow(string spr_type = "Edit") : this()
    {
        this._spr_type = spr_type;
    }
    public SprWindow()
    {
        InitializeComponent();

        this._spr_type = "Edit";

        _dataGrid = this.FindControl<TreeDataGrid>("SprTree")!;
        _dataGrid.DataContext = new SprViewModel();
        _sprViewModel = (SprViewModel)_dataGrid.DataContext;

        // Если не выбрано ни одной строки, то выключаем кнопки редактирования и удаления
        _dataGrid.RowSelection!.SelectionChanged += (sender, e) =>
        {
            _selectedRow = e.SelectedItems[0];
            bool btnEnabled = _selectedRow != null;

            this.FindControl<Button>("CreateButton")!.IsEnabled = btnEnabled;
            this.FindControl<Button>("CreateInButton")!.IsEnabled = btnEnabled;
            this.FindControl<Button>("EditButton")!.IsEnabled = btnEnabled;
            this.FindControl<Button>("DeleteButton")!.IsEnabled = btnEnabled;
        };

        _dataGrid.DoubleTapped += (sender, e) =>
        {
            Control element = e.Source as Control;
            while (element is not DataGrid)
            {
                element = element.Parent as Control;

                // Если элемент или один из его родителей является DataGridRow, значит пользователь тыкнул на таблицу
                // в противном случае пользователь тыкнул на header
                if (element is TreeDataGridRow)
                {
                    var row = (TreeDataGridRow)element;
                    var window = new SprEditWindow(row.DataContext as SpRowData);
                    window.Show();

                    break;
                }
            }
        };
    }
}