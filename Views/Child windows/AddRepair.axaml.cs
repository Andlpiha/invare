using Avalonia.Controls;
using Inv.ViewModels.Forms;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using MsgBox;
using System;
using System.ComponentModel;
using Inv.Models;
using DynamicData.Binding;
using ReactiveUI;

namespace Inv;

public partial class AddRepair : ReactiveWindow<RepairForm>
{
    private RepairForm? _old_value { get; }
    public bool edit { get; }

    public AddRepair(bool edit = false)
    {
        InitializeComponent();

        ComboBox combo = this.FindControl<ComboBox>("executorsBox")!;
        combo.SelectionChanged += (object? sender, SelectionChangedEventArgs args) =>
        {
            if (args.AddedItems == null)
                this.ViewModel!.executor = null;
            this.ViewModel!.executor = args.AddedItems[0].ToString();
        };

        this.edit = edit;
        if (edit)
            this._old_value = new RepairForm(this.ViewModel as RepairForm);
        else
            this._old_value = null;
    }

    public void okBtnClick(object sender, RoutedEventArgs args)
    {
        string message = string.Empty;

        if (!edit && RepairModel.CheckDuplicate(
            this.ViewModel as RepairForm, SQLConn.Instance.GetConnection()
        ))
            message = "Этот предмет уже в ремонте";
        else
            message = this.ViewModel!.validate();

        if (message != String.Empty)
        {
            MessageBox.Show(null, message, "Ошибка", MessageBox.MessageBoxButtons.Ok);
            return;
        }

        var conn = SQLConn.Instance.GetConnection();
        if (edit)
            RepairModel.EditRepair(this.ViewModel as RepairForm, _old_value, conn);
        else
            RepairModel.AddRepair(this.ViewModel as RepairForm, conn);

        this.Close(this.ViewModel);
    }
    public void cancelBtnClick(object sender, RoutedEventArgs args)
    {
        this.Close();
    }

    public async void sprBtnClick(object sender, RoutedEventArgs args)
    {
        var btn = args.Source as Button;

        var spr = new SprWindow("");
        var sprValue = await spr.ShowDialog<string>(this);

        if(sprValue == String.Empty)
                return;

        if(btn!.Name == "depBtn")
        {
            this.ViewModel!.Department = sprValue;
        }
        else if(btn!.Name == "usrBtn")
        {
            this.ViewModel!.User = sprValue;
        }
    }
}