using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Inv.Models;
using Inv.ViewModels;
using static Inv.Models.SprModel;

namespace Inv;

public partial class SprEditWindow : Window
{
    private SpRowData _viewModel;

    // Вызываем этот конструктор при создании нового элемента
    public SprEditWindow(int ID_UP) : this(
        new SpRowData{ ID = SprModel.getSprGen() + 1, ID_up = ID_UP, Name = "" }
    ) {}

    public SprEditWindow(SpRowData rowToEdit)
    {
        InitializeComponent();

        _viewModel = rowToEdit;
        this.DataContext = _viewModel;

        this.FindControl<NumericUpDown>("Cap")!.IsEnabled = _viewModel.Posit > 1;
        this.FindControl<NumericUpDown>("Posit")!.ValueChanged += (sender, e) =>
        {
            this.FindControl<NumericUpDown>("Cap")!.IsEnabled = e.NewValue > 1;
        };
    }

    private void DragWindow(object? sender, PointerPressedEventArgs e)
    {
        this.BeginMoveDrag(e);
    }

    public void okBtnCLick(object sender, RoutedEventArgs args)
    {
        // Возвращаем измененное значение
        Close(_viewModel);
    }

    public void cancelBtnCLick(object sender, RoutedEventArgs args)
    {
        Close(null);
    }
}