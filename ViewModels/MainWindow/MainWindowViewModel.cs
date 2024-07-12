using Avalonia.Controls;
using Avalonia.LogicalTree;
using Inv.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Inv.ViewModels.MainWindow;

public class TabItem
{
    public string Header { get; set; }
    public string ID { get; set; }

    public TabItem(string header, string tabID)
    {
        Header = header;
        ID = tabID;
    }
}

public class MainWindowViewModel : ViewModelBase
{
    public TabItem[] Tabs { get; set; }
    public static TableViewModel tableVM { get; } = new();

    // Индекс выделенной вкладки
    private int _selected_index = -1;
    public int SelectedTabIndex
    {
        get
        {
            return _selected_index;
        }
        set
        {
            _selected_index = value;

            // При обновлении индекса, также обновляем остальные значение,
            // индекс равен -1, если не выбрана ни одна вкладка
            SelectedTabHeader = value != -1 ? Tabs[value].Header : "";
            SelectedTabID = value != -1 ? Tabs[value].ID : "";
        }
    }
    // Название выделенной вкладки
    public string SelectedTabHeader { get; set; }
    public string SelectedTabID { get; set; }

    public MainWindowViewModel()
    {
        SelectedTabIndex = -1;
        var tabs_table = TableModel.getTabs();

        Tabs = new TabItem[tabs_table.Rows.Count];

        for (int i = 0; i < tabs_table.Rows.Count; i++)
        {
            var _row = tabs_table.Rows[i];

            var _name = _row.Field<string>("Name");
            var _id = _row.Field<int>("ID").ToString();

            if (_name == null || _id == null) continue;

            Tabs[i] = new TabItem(_name, _id);
        }

        // Добавить еще две вкладки в конец
        Tabs[^2] = new TabItem(Global.RepairTab, Global.RepairTab);
        Tabs[^1] = new TabItem(Global.JournalTab, Global.JournalTab);
    }

    public bool ExitApp()
    {
        Environment.Exit(0);
        return true;
    }
}
