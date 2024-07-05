using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Inv.Models;
using Inv.ViewModels.Forms;
using System;

namespace Inv;

public partial class AddItemWindow : Window
{
    private bool c_ma;
    public AddItemWindow(bool c_ma)
    {
        this.c_ma = c_ma;
        if (c_ma)
        {

        }
        else
        {

        }

        InitializeComponent();

        this.DataContext = new ItemForm();
    }

    public void onCancelClick(object sender, RoutedEventArgs args)
    {
        var _form = this.DataContext as ItemForm;

        if (_form != null)
            ToolbarModel.CreateItem(_form, c_ma);
        else
            throw new Exception("DataContext is null");

        this.Close();
    }

    public void onOkClick(object sender, RoutedEventArgs args)
    {
        this.Close();
    }
}