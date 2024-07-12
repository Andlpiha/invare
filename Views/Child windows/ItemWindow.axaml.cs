using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Inv.Models;
using Inv.ViewModels.Forms;
using Inv.ViewModels.MainWindow;
using ReactiveUI;
using System;

namespace Inv;

public partial class ItemWindow : ReactiveWindow<ItemForm>
{
    public ItemWindow()
    {
        InitializeComponent();

        this.WhenActivated(d => d(ViewModel!.okBtnClickCommand.Subscribe(arg => { if (arg != null) Close(arg); })));
        this.WhenActivated(d => d(ViewModel!.cancelBtnClickCommand.Subscribe(Close)));
    }
}