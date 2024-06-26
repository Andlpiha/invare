using Avalonia;
using Avalonia.Controls;
using System.Diagnostics;

namespace Inv.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
            
        //using (FbConnection _con = new FbConnection(connectionString))
        //{
        //    //string queryStatement = "SELECT TOP 5 * FROM COMPL ORDER BY SKLAD";
        //    string queryStatement = "SELECT first 5 * FROM COMPL";

        //    using (FbCommand _cmd = new FbCommand(queryStatement, _con))
        //    {
        //        DataTable customerTable = new DataTable();
        //        FbDataAdapter _dap = new FbDataAdapter(_cmd);

        //        _con.Open();
        //        _dap.Fill(customerTable);
        //        _con.Close();

        //    }
        //}
    }
}