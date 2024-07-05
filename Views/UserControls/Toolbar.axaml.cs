using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Inv.ViewModels;

namespace Inv;

public partial class Toolbar : UserControl
{
    public Toolbar()
    {
        InitializeComponent();
    }

    public async void createComponent(object sender, RoutedEventArgs args)
    {
        AddItemWindow window = new AddItemWindow(false);

        await window.ShowDialog(this.VisualRoot as Window);
    }
}