using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Inv.Models;
using Inv.ViewModels;
using Inv.ViewModels.Forms;
using Inv.ViewModels.MainWindow;
using Inv.Views;
using MsgBox;
using ReactiveUI;
using Svg;
using System;

namespace Inv;

public partial class Toolbar : UserControl
{
    public Toolbar()
    {
        InitializeComponent();
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
                if (Global.CurCompl != null) return;
                window.DataContext = new CreateComplectViewModel(tabID);
                break;
            case "mat":
                if (Global.CurCompl != null)
                    window.DataContext = new CreateComponentViewModel(tabID, Global.CurCompl.vnutr_num);
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
            MainWindowViewModel.tableVM.cachedCollections[_viewModel.SelectedTabID].Add(result);
    }

    public async void openSpr(object sender, RoutedEventArgs args)
    {
        SprWindow window = new SprWindow("Regular");
        window.Show();
    }

    public async void deleteItem(object sender, RoutedEventArgs args)
    {
        var _viewModel = this.DataContext as MainWindowViewModel;
        var _selectedRow = MainWindowViewModel.tableVM.selectedRow;

        if (!Global.RW)
        {
            _ = MessageBox.Show(this.VisualRoot as Window, "У вас недостаточно прав для удаления", "Ошибка доступа", MessageBox.MessageBoxButtons.Ok);
            return;
        }
        if (MainWindowViewModel.tableVM.selectedRow == null)
        {
            _ = MessageBox.Show(this.VisualRoot as Window, "Выберите строку для удаления", "Сообщение", MessageBox.MessageBoxButtons.Ok);
            return;
        }

        MessageBox.MessageBoxResult user_responce;
        switch (_viewModel!.SelectedTabID)
        {
            case Global.RepairTab:
                user_responce = await MessageBox.Show(this.VisualRoot as Window, "Вы уверены, что хотите удалить этот ремонт?", "Сообщение", MessageBox.MessageBoxButtons.YesNo);
                if (user_responce != MessageBox.MessageBoxResult.Yes) break;

                ItemModel.DeleteRemontItem(_selectedRow!.id);
                MainWindowViewModel.tableVM.cachedCollections[_viewModel.SelectedTabID].Remove(_selectedRow);
                break;
            case Global.JournalTab:
                throw new ApplicationException("Cannot delete journal entry");
            default:
                if (_selectedRow!.icon == Global.ComplectIcon)
                {
                    user_responce = await MessageBox.Show(this.VisualRoot as Window, "Вы уверены, что хотите удалить этот комплект?", "Сообщение", MessageBox.MessageBoxButtons.YesNo);
                    if (user_responce != MessageBox.MessageBoxResult.Yes) break;

                    ItemModel.DeleteComplectItem(_selectedRow!.id);
                    MainWindowViewModel.tableVM.cachedCollections[_viewModel.SelectedTabID].Remove(_selectedRow);
                }
                else
                {
                    user_responce = await MessageBox.Show(this.VisualRoot as Window, "Вы уверены, что хотите удалить эту единицу?", "Сообщение", MessageBox.MessageBoxButtons.YesNo);
                    if (user_responce != MessageBox.MessageBoxResult.Yes) break;

                    ItemModel.DeleteMatItem(_selectedRow!.id);
                    MainWindowViewModel.tableVM.cachedCollections[_viewModel.SelectedTabID].Remove(_selectedRow);
                }
                break;
        }
    }

    public static void editItem(TableRow? row = null)
    {
        if (row == null)
        {
            row = MainWindowViewModel.tableVM.selectedRow;
            if (row == null) return;
        }
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