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
using System.Data.Entity.Core.Mapping;

namespace Inv;

public enum RepairWindowType
{
    Create,
    Edit,
    Prof
}

public partial class RepairWindow : ReactiveWindow<RepairForm>
{
    private RepairForm? _old_value { get; set; }
    public RepairWindowType type { get; }

    public RepairWindow(RepairWindowType type = RepairWindowType.Create)
    {
        InitializeComponent();

        ComboBox combo = this.FindControl<ComboBox>("executorsBox")!;
        combo.SelectionChanged += (object? sender, SelectionChangedEventArgs args) =>
        {
            if (args.AddedItems == null)
                this.ViewModel!.executor = null;
            this.ViewModel!.executor = args.AddedItems[0].ToString();
        };
        this.type = type;

        this.WhenActivated(disposables =>
        {
            if (this.ViewModel is null)
                return;

            if (type == RepairWindowType.Edit)
                this._old_value = new RepairForm(this.ViewModel as RepairForm);
            else
            {
                this._old_value = null;

                this.FindControl<TextBox>("ComplNum")!.IsEnabled = false;
                this.FindControl<TextBox>("VnutrNum")!.IsEnabled = false;
                this.FindControl<TextBox>("InvNum")!.IsEnabled = false;
            }

            if (this.type == RepairWindowType.Prof)
            {
                this.FindControl<TextBox>("Complaint")!.IsEnabled = false;
                this.FindControl<TextBox>("Complaint")!.Text = "Профилактика";

                this.FindControl<TextBox>("Diagnosis")!.IsEnabled = false;
                this.FindControl<CalendarDatePicker>("AcceptedTime")!.IsEnabled = false;
                this.FindControl<CalendarDatePicker>("ReturnedTime")!.IsEnabled = false;

                this.FindControl<CalendarDatePicker>("DoneTime")!.SelectedDate = DateTime.Now;
            }
        });
    }

    public void okBtnClick(object sender, RoutedEventArgs args)
    {
        string message = string.Empty;

        if (type == RepairWindowType.Prof)
            this.ViewModel.returnedTime = this.ViewModel.doneTime;

        if (type != RepairWindowType.Edit && RepairModel.CheckDuplicate(
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
        if (this.type == RepairWindowType.Edit)
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