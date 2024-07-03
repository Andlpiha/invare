using Avalonia.Controls;
using Avalonia.Threading;
using FirebirdSql.Data.FirebirdClient;
using Inv.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inv.ViewModels
{
    public class TableRow
    {
        public int? number { get; set; }            = null;
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

    public class TableViewModel : ReactiveObject
    {
        // Когда пользователь переходит на вкладку, все значения достаются из базы данных
        // и сохраняются в оперативной памяти
        public Dictionary<string, ObservableCollection<TableRow>> cachedCollections = new();

        public int? Compl_id { get; set; }

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
                TableModel.TransactionReader transaction = _model.getSkladReader(new_tabID);
                if (!transaction.isOpen()) return;

                FbDataReader reader = transaction.getReader();

                int it = 0;
                while(reader.Read())
                { 
                    var new_row = new TableRow();

                    new_row.number = it;
                    new_row.icon = reader["icon"] as int?;
                    new_row.compl_num = reader["compl_num"] as int?;
                    new_row.vnutr_num = reader["vnutr_num"] as int?;
                    new_row.inv_num = reader["inv_num"] as string;

                    new_row.name = reader["name"] as string;
                    new_row.dep_name = reader["dep_name"] as string;
                    new_row.user_name = reader["user_name"] as string;
                    new_row.login_name = reader["login_name"] as string;
                    new_row.site_name = reader["site_name"] as string;

                    new_row.Date_prof = reader["Date_prof"] as DateTime?;
                    new_row.Date_create = reader["Date_create"] as DateTime?;

                    new_row.MOL_name = reader["MOL_name"] as string;
                    new_row.MOLpl_name = reader["MOLpl_name"] as string;
                    new_row.Description = reader["Description"] as string;

                    // Все обновления графики должны выполнятся только из основного потока
                    // так что передаем управление ему
                    Dispatcher.UIThread.InvokeAsync(
                        () => cachedCollections[new_tabID].Add(new_row),
                        DispatcherPriority.Render
                    );

                    it++;
                }

                // Завершаем транзакцию
                transaction.closeConnection();
            }
        }

        public Dictionary<string, DataGridLength> column_width = new Dictionary<string, DataGridLength>()
        {
            { "icon", new DataGridLength(20, DataGridLengthUnitType.Pixel) },
            { "code_op", new DataGridLength(20, DataGridLengthUnitType.Pixel) },
            { "sklad", new DataGridLength(100, DataGridLengthUnitType.Pixel) },
            { "compl_num", new DataGridLength(40, DataGridLengthUnitType.Pixel) },
            { "vnutr_num", new DataGridLength(40, DataGridLengthUnitType.Pixel) },
            { "inv_num", new DataGridLength(90, DataGridLengthUnitType.Pixel) },
            { "ser_num", new DataGridLength(45, DataGridLengthUnitType.Pixel) },
            { "date_do", new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "date_in", new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "date_done", new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "date_out", new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "Date_prof", new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "Date_create", new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "name", new DataGridLength(220, DataGridLengthUnitType.Pixel) },
            { "login_name", new DataGridLength(60, DataGridLengthUnitType.Pixel) },
            { "user_name", new DataGridLength(240, DataGridLengthUnitType.Pixel) },
            { "dep_name", new DataGridLength(200, DataGridLengthUnitType.Pixel) },
            { "pribor_name", new DataGridLength(100, DataGridLengthUnitType.Pixel) },
            { "site_name", new DataGridLength(90, DataGridLengthUnitType.Pixel) },
            { "jaloba", new DataGridLength(200, DataGridLengthUnitType.Pixel) },
            { "diagnos", new DataGridLength(200, DataGridLengthUnitType.Pixel) },
            { "repair", new DataGridLength(300, DataGridLengthUnitType.Pixel) },
            { "MOL_name", new DataGridLength(100, DataGridLengthUnitType.Pixel) },
            { "MOLpl_name", new DataGridLength(100, DataGridLengthUnitType.Pixel) },
            { "Description", new DataGridLength(320, DataGridLengthUnitType.Pixel) },
        };

        public Dictionary<string, bool> journal_column_visibility = new Dictionary<string, bool>()
        {
            { "icon", true },
            { "code_op", true },
            { "sklad", true },
            { "compl_num", true },
            { "vnutr_num", true },
            { "inv_num", true },
            { "ser_num", true },
            { "date_do", true },
            { "date_in", false },
            { "date_done", false },
            { "date_out", false },
            { "Date_prof", true },
            { "Date_create", true },
            { "name", true },
            { "login_name", true },
            { "user_name", true },
            { "dep_name", false },
            { "pribor_name", false },
            { "site_name", true },
            { "jaloba", false },
            { "diagnos", false },
            { "repair", false },
            { "MOL_name", true },
            { "MOLpl_name", false },
            { "Description", true },
        };

        public Dictionary<string, bool> remont_column_visibility = new Dictionary<string, bool>()
        {
            { "icon", true },
            { "code_op", false },
            { "sklad", false },
            { "compl_num", true },
            { "vnutr_num", true },
            { "inv_num", true },
            { "ser_num", false },
            { "date_do", false },
            { "date_in", true },
            { "date_done", true },
            { "date_out", true },
            { "Date_prof", false },
            { "Date_create", false },
            { "name", true },
            { "login_name", false },
            { "user_name", true },
            { "dep_name", true },
            { "pribor_name", true },
            { "site_name", false },
            { "jaloba", true },
            { "diagnos", true },
            { "repair", true },
            { "MOL_name", false },
            { "MOLpl_name", false },
            { "Description", false },
        };

        public Dictionary<string, bool> sklad_column_visibility = new Dictionary<string, bool>()
        {
            { "icon", true },
            { "code_op", false },
            { "sklad", false },
            { "compl_num", true },
            { "vnutr_num", true },
            { "inv_num", true },
            { "ser_num", false },
            { "date_do", false },
            { "date_in", false },
            { "date_done", false },
            { "date_out", false },
            { "Date_prof", false },
            { "Date_create", false },
            { "name", true },
            { "login_name", true },
            { "user_name", true },
            { "dep_name", true },
            { "pribor_name", false },
            { "site_name", true },
            { "jaloba", false },
            { "diagnos", false },
            { "repair", false },
            { "MOL_name", true },
            { "MOLpl_name", true },
            { "Description", true },
        };
    }
}
