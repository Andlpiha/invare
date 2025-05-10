using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using DynamicData;
using Inv.Models;
using Inv.ViewModels;
using Inv.ViewModels.Forms;
using Inv.ViewModels.MainWindow;
using Inv.Views;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MsgBox;
using ReactiveUI;
using Svg;
using System;
using System.ComponentModel;

namespace Inv;

public partial class Toolbar : UserControl
{
    public Toolbar()
    {
        InitializeComponent();
    }
    public void openSpr(object sender, RoutedEventArgs args)
    {
        SprWindow window = new SprWindow("Regular");
        window.Show();
    }

    public async void createOrEdit(object sender, RoutedEventArgs args)
    {
        var _viewModel = this.DataContext as MainWindowViewModel;
        var button = sender as Button;

        if (_viewModel!.SelectedTabID == Global.JournalTab ||
            _viewModel!.SelectedTabID == Global.RepairTab)
            return;
        Int32 tabID = Int32.Parse(_viewModel!.SelectedTabID);

        ItemWindow window = new ItemWindow();
        switch(button!.Name)
        {
            case "compl":
                if (Global.ComplUp != null) return;
                window.DataContext = new CreateComplectViewModel(tabID);
                break;
            case "mat":
                if (Global.ComplUp != null)
                    window.DataContext = new CreateComponentViewModel(tabID, Global.ComplUp.vnutr_num);
                else
                    window.DataContext = new CreateComponentViewModel(tabID);
                break;
            case "edit":
                throw new NotImplementedException();
        }

        // Диалог привязан к MainWindow
        var result = await window.ShowDialog<TableRow>(this.VisualRoot as Window);

        // Добавляем созданную строку в таблицу
        if (result != null)
            MainWindowViewModel.tableVM.cachedCollection.AddOrUpdate(result);
    }

    public void addRepair(object sender, RoutedEventArgs args)
    {
        RepairForm form = new RepairForm();

        var _viewModel = this.DataContext as MainWindowViewModel;
        if (_viewModel!.SelectedTabID != Global.RepairTab && _viewModel.SelectedRow != null)
        {
            form.compl_num  = _viewModel.SelectedRow!.compl_num.ToString()  ?? String.Empty;
            form.vnutr_num  = _viewModel.SelectedRow!.vnutr_num.ToString()  ?? String.Empty;
            form.inv_num    = _viewModel.SelectedRow!.inv_num               ?? String.Empty;

            form.name       = _viewModel.SelectedRow!.name                  ?? String.Empty;
            form.Department = _viewModel.SelectedRow!.dep_name              ?? String.Empty;
            form.User       = _viewModel.SelectedRow!.user_name             ?? String.Empty;
        }

        RepairWindow window = new RepairWindow()
        {
            ViewModel = form
        };
        window.Show();
    }

    public async void deleteItem(object sender, RoutedEventArgs args)
    {
        var _viewModel = this.DataContext as MainWindowViewModel;
        var _selectedRow = _viewModel!.SelectedRow;

        if (!Global.RW)
        {
            _ = MessageBox.Show(this.VisualRoot as Window, "У вас недостаточно прав для удаления", "Ошибка доступа", MessageBox.MessageBoxButtons.Ok);
            return;
        }
        if (_viewModel.SelectedRow == null)
        {
            _ = MessageBox.Show(this.VisualRoot as Window, "Выберите строку для удаления", "Сообщение", MessageBox.MessageBoxButtons.Ok);
            return;
        }

        MessageBox.MessageBoxResult user_responce;
        System.Action<long> deleteFunction;
        string message;

        switch (_viewModel!.SelectedTabID)
        {
            case Global.JournalTab:
                throw new ApplicationException("Cannot delete journal entry");
            case Global.RepairTab:
                message = "этот ремонт";
                deleteFunction = ItemModel.DeleteRemontItem;
                break;
            default:
                if (_selectedRow!.icon == Global.ComplectIcon)
                {
                    message = "этот комплект";
                    deleteFunction = ItemModel.DeleteComplectItem;
                }
                else
                {
                    message = "эту единицу";
                    deleteFunction = ItemModel.DeleteMatItem;
                }
                break;
        }
        user_responce = await MessageBox.Show(this.VisualRoot as Window, $"Вы уверены, что хотите удалить {message}?", "Сообщение", MessageBox.MessageBoxButtons.YesNo);
        if (user_responce == MessageBox.MessageBoxResult.Yes)
        {
            deleteFunction(_selectedRow!.id);
            MainWindowViewModel.tableVM.cachedCollection.Remove(_selectedRow);
        }
    }

    public static async void editItem(TableRow row, string tabID)
    {
        if (tabID == Global.RepairTab)
        {
            RepairForm form = new RepairForm(row);
            RepairWindow window = new(true)
            {
                ViewModel = form,
            };
            
            if (Avalonia.Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var updated_row = await window.ShowDialog<RepairForm>(desktop.MainWindow!);
                if (updated_row is null)
                    return; // Ничего не поменяли

                if (updated_row.id is null)
                    throw new ArgumentException("Edited repair ID is null");

                updateTableRowFromRepair(row, updated_row);
                MainWindowViewModel.tableVM.cachedCollection.AddOrUpdate(row);
            }

        }
        else if (tabID == Global.JournalTab)
        {
            //TODO
        }
        else
        {
            // Иначе редактируем комплекты
        }
    }

    private static void updateTableRowFromRepair(TableRow row, RepairForm updated)
    {
        row.compl_num = int.Parse(updated.compl_num);
        row.vnutr_num = int.Parse(updated.vnutr_num);
        row.inv_num = updated.inv_num;
        row.name = updated.name;
        row.pribor_name = updated.executor;
        row.dep_name = updated.Department;
        row.user_name = updated.User;
        row.jaloba = updated.complaint;
        row.diagnos = updated.diagnosis;
        row.repair = updated.actions_taken;
        row.date_in = updated.acceptedTime;
        row.date_done = updated.doneTime;
        row.date_out = updated.returnedTime;
    }

    public void setButtonEnabled(string pageID)
    {
        switch (pageID)
        {
            case Global.JournalTab:
                this.FindControl<Button>("repair")!.IsEnabled = false;
                this.FindControl<Button>("edit")!.IsEnabled = false;
                this.FindControl<Button>("delete")!.IsEnabled = false;
                this.FindControl<Button>("ret")!.IsEnabled = false;
                this.FindControl<Button>("compl")!.IsEnabled = false;
                this.FindControl<Button>("mat")!.IsEnabled = false;
                this.FindControl<Button>("add_compl")!.IsEnabled = false;
                this.FindControl<Button>("move")!.IsEnabled = false;
                this.FindControl<Button>("maintenance")!.IsEnabled = false;
                break;
            case Global.RepairTab:
                this.FindControl<Button>("repair")!.IsEnabled = true;
                this.FindControl<Button>("edit")!.IsEnabled = true;
                this.FindControl<Button>("delete")!.IsEnabled = true;
                this.FindControl<Button>("ret")!.IsEnabled = true;
                this.FindControl<Button>("compl")!.IsEnabled = false;
                this.FindControl<Button>("mat")!.IsEnabled = false;
                this.FindControl<Button>("add_compl")!.IsEnabled = false;
                this.FindControl<Button>("move")!.IsEnabled = false;
                this.FindControl<Button>("maintenance")!.IsEnabled = false;
                break;
            default:
                foreach (var item in this.FindControl<StackPanel>("ToolbarStack")!.Children)
                {
                    if (item is Button)
                        item.IsEnabled = true;
                }
                break;
        }
    }
}