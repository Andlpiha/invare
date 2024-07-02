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
        public int? icon { get; set; }              = null;
        public int? code_op { get; set; }           = null;
        public int? sklad { get; set; }             = null;
        public int? compl_num { get; set; }         = null;
        public int? vnutr_num { get; set; }         = null;
        public string? inv_num { get; set; }        = null;
        public int? ser_num { get; set; }           = null;
        public DateTime? date_do { get; set; }      = null;
        public DateTime? date_in { get; set; }      = null;
        public DateTime? date_done { get; set; }    = null;
        public DateTime? date_out { get; set; }     = null;
        public DateTime? Date_prof { get; set; }    = null;
        public DateTime? Date_create { get; set; }  = null;
        public string? name { get; set; }           = null; 
        public string? login_name { get; set; }     = null;
        public string? user_name { get; set; }      = null;
        public string? dep_name { get; set; }       = null;
        public string? pribor_name { get; set; }    = null;
        public string? site_name { get; set; }      = null;
        public string? jaloba { get; set; }         = null;
        public string? diagnos { get; set; }        = null;
        public string? repair { get; set; }         = null;
        public string? MOL_name { get; set; }       = null;
        public string? MOLpl_name { get; set; }     = null;
        public string? Description { get; set; }    = null;
    }

    public class TableWidth
    {
        // Setup width
        public int icon_width { get; set; } = 18;
        public int code_op_width { get; set; } = 20;
        public int sklad_width { get; set; } = 100;
        public int compl_num_width { get; set; } = 40;
        public int vnutr_num_width { get; set; } = 40;
        public int inv_num_width { get; set; } = 45;
        public int ser_num_width { get; set; } = 45;
        public int date_do_width { get; set; } = 105;
        public int date_in_width { get; set; } = 105;
        public int date_done_width { get; set; } = 105;
        public int date_out_width { get; set; } = 105;
        public int Date_prof_width { get; set; } = 60;
        public int Date_create_width { get; set; } = 60;
        public int name_width { get; set; } = 220;
        public int user_name_width { get; set; } = 240;
        public int login_name_width { get; set; } = 60;
        public int dep_name_width { get; set; } = 200;
        public int pribor_name_width { get; set; } = 100;
        public int site_name_width { get; set; } = 90;
        public int jaloba_width { get; set; } = 200;
        public int diagnos_width { get; set; } = 200;
        public int repair_width { get; set; } = 300;
        public int MOL_name_width { get; set; } = 100;
        public int MOLpl_name_width { get; set; } = 100;
        public int Description_width { get; set; } = 320;
    }

    public class TableViewModel
    {
        public ObservableCollection<TableRow> TableRows { get; } = new();
        public int Compl_id { get; set; }

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
                TableRows.Clear();

                foreach (DataRow row in sklad.Rows)
                {
                    var new_row = new TableRow();

                    new_row.icon = row.Field<int?>("icon");
                    new_row.compl_num = row.Field<int?>("compl_num");
                    new_row.vnutr_num = row.Field<int?>("vnutr_num");
                    new_row.inv_num = row.Field<string?>("inv_num");

                    new_row.name = row.Field<string?>("name");
                    new_row.dep_name = row.Field<string?>("dep_name");
                    new_row.user_name = row.Field<string?>("user_name");
                    new_row.login_name = row.Field<string?>("login_name");
                    new_row.site_name = row.Field<string?>("site_name");

                    new_row.Date_prof = row.Field<DateTime?>("Date_prof");
                    new_row.Date_create = row.Field<DateTime?>("Date_create");

                    new_row.MOL_name = row.Field<string?>("MOL_name");
                    new_row.MOLpl_name = row.Field<string?>("MOLpl_name");
                    new_row.Description = row.Field<string?>("Description");

                    TableRows.Add(new_row);
                }
            }
        }
    }
}
