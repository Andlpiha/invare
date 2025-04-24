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

    public MainWindow()
    {
        _viewModel = new MainWindowViewModel();
        this.DataContext = _viewModel;

        InitializeComponent();

        this.FindControl<TabControl>("TabControl")!.DataContext = _viewModel.Tabs;
        this.FindControl<MenuBar>("MenuBar")!.DataContext = _viewModel;
        this.FindControl<Toolbar>("Toolbar")!.DataContext = _viewModel;

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
        var content = tabControl
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

        if (tabControl == null || _viewModel == null) return;
        _viewModel.SelectedTabIndex = tabControl.SelectedIndex;

        this.FindControl<Toolbar>("Toolbar")!.setButtonEnabled(_viewModel.SelectedTabID);
    }
}