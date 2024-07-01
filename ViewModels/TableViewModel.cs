using Inv.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inv.ViewModels
{
    public class TableRow
    {
        public int icon { get; set; } = -1;
        public int code_op { get; set; } = -1;
        public int sklad { get; set; } = -1;
        public int compl_num { get; set; } = -1;
        public int vnutr_num { get; set; } = -1;
        public int inv_num { get; set; } = -1;
        public int ser_num { get; set; } = -1;
        public DateTime date_do { get; set; } = DateTime.MinValue;
        public DateTime date_in { get; set; } = DateTime.MinValue;
        public DateTime date_done { get; set; } = DateTime.MinValue;
        public DateTime date_out { get; set; } = DateTime.MinValue;
        public DateTime Date_prof { get; set; } = DateTime.MinValue;
        public DateTime Date_create { get; set; } = DateTime.MinValue;
        public string name { get; set; } = String.Empty; 
        public string login_name { get; set; } = String.Empty;
        public string user_name { get; set; } = String.Empty;
        public string dep_name { get; set; } = String.Empty;
        public string pribor_name { get; set; } = String.Empty;
        public string jaloba { get; set; } = String.Empty;
        public string diagnos { get; set; } = String.Empty;
        public string repair { get; set; } = String.Empty;
        public string MOL_name { get; set; } = String.Empty;
        public string MOLpl_name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
    }

    public class TableViewModel
    {
        public ObservableCollection<TableRow> TableRows { get; } = new();

        private TableModel _model;
        public TableViewModel() 
        {
            _model = new TableModel();
        }

        public void updateTableRows(string new_tabID)
        {
            // Ремонты
            if (new_tabID == Global.RepairTab)
            {

            }
            // Журнал
            else if (new_tabID == Global.JournalTab)
            {

            }
            //Склад
            else
            {
                var sklad = _model.getSkladItems(new_tabID);
            }
        }
    }
}
