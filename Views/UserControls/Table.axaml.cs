using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace Inv;

public partial class Table : UserControl
{
    private static readonly string pageTypeDefault = "Получено";
    public static readonly StyledProperty<string> pageTypeProperty =
        AvaloniaProperty.Register<Table, string>(
            nameof(PageType),
            defaultValue: pageTypeDefault,
            defaultBindingMode: Avalonia.Data.BindingMode.OneWay
        );
    public string PageType
    {
        get => GetValue(pageTypeProperty);
        set 
        {
            SetValue(pageTypeProperty, value);
            onPageTypeChange(value);
        }
    }

    public Table()
    {
        InitializeComponent();
        PageType = pageTypeDefault;
    }

    private void onPageTypeChange(string newValue)
    {
        this.FindControl<TextBox>("Box").Text = newValue;
    }
}