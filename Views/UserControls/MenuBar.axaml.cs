using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Inv.ViewModels;

namespace Inv;

public partial class MenuBar : UserControl
{
    public MenuBar()
    {
        DataContext = new MenuBarViewModel();
        InitializeComponent();
    }
}