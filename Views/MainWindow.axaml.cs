using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Inv.ViewModels.MainWindow;
using System.Linq;
using System.Windows.Input;

namespace Inv.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel _viewModel;
    private TabControl _tabControl;
    private MenuBar _menu_bar;
    private Toolbar _toolbar;

    public MainWindow()
    {
        _viewModel = new MainWindowViewModel();
        this.DataContext = _viewModel;

        InitializeComponent();

        _tabControl = this.FindControl<TabControl>("TabControl")!;
        _menu_bar = this.FindControl<MenuBar>("MenuBar")!;
        _toolbar = this.FindControl<Toolbar>("Toolbar")!;

        _tabControl.DataContext = _viewModel.Tabs;
        _menu_bar.DataContext = _viewModel;
        _toolbar.DataContext = _viewModel;

        _toolbar.setButtonEnabled(_viewModel.SelectedTabID);

        this.KeyUp += handleKeyUp;
    }

    public void handleKeyUp(object? sender,  KeyEventArgs args)
    {
        if (args.Key == Key.Escape)
        {
            if (MainWindowViewModel.tableVM.selectedRow == null)
                findMainTable()!.RenderNewPage(_viewModel.SelectedTabID);
            else
                deselectRows();
        }
    }

    public ICommand DeselectRowsCommand { get; }
    public void deselectRows()
    {
        var table = findMainTable();
        if (table != null)
            table.deselectRows();
    }

    private Table? findMainTable()
    {
        var tabControl = this.FindControl<TabControl>("TabControl");
        var content = tabControl!
            .GetLogicalChildren()
            .Where((child) => child is DockPanel)
            .FirstOrDefault();

        if (content != null)
        {
            var table = content
                .GetLogicalChildren()
                .Where((child) => child is Table)
                .FirstOrDefault() as Table;

            return table;
        }
        return null;
    }

    // ≈сли пользователь кликает на другую вкладку
    public void TabChangeHandler(object sender, SelectionChangedEventArgs args)
    {
        var tabControl = sender as TabControl;

        if (tabControl == null || _viewModel == null || _toolbar == null) return;
        _viewModel.SelectedTabIndex = tabControl.SelectedIndex;

        _toolbar.setButtonEnabled(_viewModel.SelectedTabID);
    }

    public void SelectionChangeHandler(object sender, SelectionChangedEventArgs args)
    {
        if (args.AddedItems.Count > 0 && args.AddedItems[0] is TableRow row)
            _viewModel.SelectedRow = row;
        else
            _viewModel.SelectedRow = null;
    }
}