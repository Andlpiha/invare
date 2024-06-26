using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Inv.ViewModels;

namespace Inv;

public partial class Toolbar : UserControl
{
    public Toolbar()
    {
        DataContext = new ToolbarViewModel();
        InitializeComponent();
    }
}