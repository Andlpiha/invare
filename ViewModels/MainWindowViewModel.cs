using Avalonia.Controls;
using FirebirdSql.Data.FirebirdClient;
using Inv.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Inv.ViewModels;

public class TabItemViewModel : ViewModelBase
{
    public string Header { get; }
    public string Content { get; }

    public TabItemViewModel(string header, string content)
    {
        Header = header;
        Content = content;
    }
}

public class MainWindowViewModel : ViewModelBase
{
    public string[] TabHeaders { get; }

    // Индекс выделенной вкладки
    private int _selected_index = -1;
    public int SelectedIndex
    { 
        get
        {
            return _selected_index;
        }
        set 
        { 
            _selected_index = value;
            SelectedTitle = value != -1 ? TabHeaders[value] : "";
        }
    }
    // Название выделенной вкладки
    public string SelectedTitle { get; set; }

    public MainWindowViewModel() 
    {
        SelectedIndex = -1;

        FbConnection _con = SQLConn.Instance.GetConnection();
        List<string> tabHeaders = TableModel.getTabs(_con);

        TabHeaders = new string[tabHeaders.Count + 2];

        for (int i = 0; i < tabHeaders.Count; i++)
            TabHeaders[i] = tabHeaders[i];

        // Добавить еще две вкладки
        TabHeaders[^2] = "Ремонты";
        TabHeaders[^1] = "Журнал";
    }  
}
