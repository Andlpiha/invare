using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Inv.ViewModels.MainWindow;

namespace Inv;

public partial class ChatSidebar : UserControl
{
    private readonly ChatSidebarViewModel _viewModel;

    public ChatSidebar()
    {
        _viewModel = new ChatSidebarViewModel();    
        this.DataContext = _viewModel;

        _viewModel.Messages.Add(new("Рубцов", "Дерьмо полное!"));
        _viewModel.Messages.Add(new("Рубцов", "Вообще капец!"));
        _viewModel.Messages.Add(new("Иванов", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus id tellus nulla. Nullam neque purus, imperdiet eget nisl in, eleifend sollicitudin eros. Sed condimentum massa velit, vel posuere nisl consequat non. Integer sit amet libero suscipit, convallis risus sed, vulputate orci.", false));

        InitializeComponent();
    }
}