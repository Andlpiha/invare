using Avalonia.Controls;
using Avalonia.Input;
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

namespace Inv.ViewModels.MainWindow
{
    public class TableRow
    {
        public int id { get; set; }
        public int? icon { get; set; } = null;
        public int? code_op { get; set; } = null;
        public int? sklad { get; set; } = null;
        public int? compl_num { get; set; } = null;
        public int? mat_num { get; set; } = null;
        public int? vnutr_num { get; set; } = null;
        public string? inv_num { get; set; } = null;
        public int? ser_num { get; set; } = null;
        public DateTime? date_do { get; set; } = null;
        public DateTime? user_do { get; set; } = null;
        public DateTime? date_in { get; set; } = null;
        public DateTime? date_done { get; set; } = null;
        public DateTime? date_out { get; set; } = null;
        public DateTime? date_prof { get; set; } = null;
        public DateTime? date_create { get; set; } = null;
        public string? name { get; set; } = null;
        public string? login_name { get; set; } = null;
        public string? user_name { get; set; } = null;
        public string? dep_name { get; set; } = null;
        public string? pribor_name { get; set; } = null;
        public string? site_name { get; set; } = null;
        public string? jaloba { get; set; } = null;
        public string? diagnos { get; set; } = null;
        public string? repair { get; set; } = null;
        public string? MOL_name { get; set; } = null;
        public string? MOLpl_name { get; set; } = null;
        public string? Description { get; set; } = null;
    }

    public class TableViewModel : ReactiveObject
    {
        // Когда пользователь переходит на вкладку, все значения достаются из базы данных
        // и сохраняются в оперативной памяти
        public Dictionary<string, ObservableCollection<TableRow>> cachedCollections = new();
        public ObservableCollection<TableRow> tempCollection = new();

        public TableRow? selectedRow { get; set; }
        public bool bottomLevel = false;

        private TableModel _model;
        public TableViewModel()
        {
            _model = new TableModel();
        }

        public void onSelectedRowChange(object? sender, SelectionChangedEventArgs args)
        {
            var _selected = ((DataGrid)sender).SelectedItem as TableRow;

            if (_selected != null)
                selectedRow = _selected;
            else
                selectedRow = null;
        }

        public void updateTableRows(string new_tabID)
        {
            // Ремонты
            if (new_tabID == Global.RepairTab)
            {
                TableModel.TransactionReader transaction = _model.getRepairsReader();
                if (!transaction.isOpen()) return;

                FbDataReader reader = transaction.getReader();

                while (reader.Read())
                {
                    var new_row = TableRowForRepair(reader);

                    // Все обновления графики должны выполнятся только из основного потока
                    // так что передаем управление ему
                    Dispatcher.UIThread.InvokeAsync(
                        () => cachedCollections[new_tabID].Add(new_row),
                        DispatcherPriority.Render
                    );
                }

                // Завершаем транзакцию
                transaction.closeConnection();
            }
            // Журнал
            else if (new_tabID == Global.JournalTab)
            {
                TableModel.TransactionReader transaction = _model.getLogReader();
                if (!transaction.isOpen()) return;

                FbDataReader reader = transaction.getReader();

                while (reader.Read())
                {
                    var new_row = TableRowForJournal(reader);

                    // Все обновления графики должны выполнятся только из основного потока
                    // так что передаем управление ему
                    Dispatcher.UIThread.InvokeAsync(
                        () => cachedCollections[new_tabID].Add(new_row),
                        DispatcherPriority.ApplicationIdle
                    );
                }

                // Завершаем транзакцию
                transaction.closeConnection();
            }
            //Склад
            else
            {
                TableModel.TransactionReader transaction = _model.getSkladReader(new_tabID);
                if (!transaction.isOpen()) return;

                FbDataReader reader = transaction.getReader();

                while (reader.Read())
                {
                    var new_row = TableRowForSklad(reader);

                    // Все обновления графики должны выполнятся только из основного потока
                    // так что передаем управление ему
                    Dispatcher.UIThread.InvokeAsync(
                        () => cachedCollections[new_tabID].Add(new_row),
                        DispatcherPriority.Render
                    );
                }

                // Завершаем транзакцию
                transaction.closeConnection();
            }
        }

        //Отображение комплекта
        public void updateRowsFromComplect(string tabID, int compl_id)
        {
            DataTable table = _model.getComplectTable(tabID, compl_id.ToString());
            DataTableReader reader = table.CreateDataReader();

            while (reader.Read())
            {
                var new_row = new TableRow();

                new_row.id = (int)reader["id"];
                new_row.icon = reader["icon"] as int?;
                new_row.compl_num = reader["compl_num"] as int?;
                new_row.vnutr_num = reader["vnutr_num"] as int?;
                new_row.inv_num = reader["inv_num"] as string;

                new_row.name = reader["name"] as string;
                new_row.dep_name = reader["dep_name"] as string;
                new_row.user_name = reader["user_name"] as string;
                //new_row.login_name = reader["login_name"] as string;
                //new_row.site_name = reader["site_name"] as string;

                new_row.date_prof = reader["Date_prof"] as DateTime?;
                new_row.date_create = reader["Date_create"] as DateTime?;

                new_row.MOL_name = reader["MOL_name"] as string;
                new_row.MOLpl_name = reader["MOLpl_name"] as string;
                new_row.Description = reader["Description"] as string;

                // Все обновления графики должны выполнятся только из основного потока
                // так что передаем управление ему
                Dispatcher.UIThread.InvokeAsync(
                    () => tempCollection.Add(new_row),
                    DispatcherPriority.Render
                );
            }
        }

        private TableRow TableRowForRepair(FbDataReader reader)
        {
            var new_row = new TableRow();

            new_row.id = (int)reader["id"];
            new_row.icon = reader["icon"] as int?;
            new_row.compl_num = reader["compl_num"] as int?;
            new_row.vnutr_num = reader["vnutr_num"] as int?;
            new_row.inv_num = reader["inv_num"] as string;

            new_row.name = reader["name"] as string;
            new_row.dep_name = reader["dep_name"] as string;
            new_row.user_name = reader["user_name"] as string;
            new_row.pribor_name = reader["pribor_name"] as string;

            new_row.jaloba = reader["jaloba"] as string;
            new_row.diagnos = reader["diagnos"] as string;
            new_row.repair = reader["repair"] as string;

            new_row.date_in = reader["date_in"] as DateTime?;
            new_row.date_done = reader["date_done"] as DateTime?;
            new_row.date_out = reader["date_out"] as DateTime?;

            return new_row;
        }

        private TableRow TableRowForJournal(FbDataReader reader)
        {
            var new_row = new TableRow();

            new_row.id = (int)reader["id"];
            new_row.icon = reader["icon"] as int?;
            new_row.compl_num = reader["compl_num"] as int?;
            new_row.inv_num = reader["inv_num"] as string;
            new_row.ser_num = reader["ser_num"] as int?;
            new_row.vnutr_num = reader["vnutr_num"] as int?;
            new_row.sklad = reader["sklad"] as int?;

            new_row.name = reader["name"] as string;
            new_row.user_name = reader["user_name"] as string;

            new_row.date_do = reader["date_do"] as DateTime?;
            new_row.user_do = reader["user_do"] as DateTime?;
            new_row.date_prof = reader["date_prof"] as DateTime?;
            new_row.date_create = reader["date_create"] as DateTime?;

            new_row.code_op = reader["code_op"] as int?;

            new_row.MOL_name = reader["MOL_name"] as string;
            new_row.Description = reader["Description"] as string;

            return new_row;
        }

        private TableRow TableRowForSklad(FbDataReader reader)
        {
            var new_row = new TableRow();

            new_row.id = (int)reader["id"];
            new_row.icon = reader["icon"] as int?;
            new_row.compl_num = reader["compl_num"] as int?;
            new_row.vnutr_num = reader["vnutr_num"] as int?;
            new_row.inv_num = reader["inv_num"] as string;

            new_row.name = reader["name"] as string;
            new_row.dep_name = reader["dep_name"] as string;
            new_row.user_name = reader["user_name"] as string;
            new_row.login_name = reader["login_name"] as string;
            new_row.site_name = reader["site_name"] as string;

            new_row.date_prof = reader["Date_prof"] as DateTime?;
            new_row.date_create = reader["Date_create"] as DateTime?;

            new_row.MOL_name = reader["MOL_name"] as string;
            new_row.MOLpl_name = reader["MOLpl_name"] as string;
            new_row.Description = reader["Description"] as string;

            return new_row;
        }

        public Dictionary<string, DataGridLength> column_width = new Dictionary<string, DataGridLength>()
        {
            { "number",         new DataGridLength(35, DataGridLengthUnitType.Pixel) },
            { "icon",           new DataGridLength(35, DataGridLengthUnitType.Pixel) },
            { "code_op",        new DataGridLength(35, DataGridLengthUnitType.Pixel) },
            { "sklad",          new DataGridLength(100, DataGridLengthUnitType.Pixel) },
            { "compl_num",      new DataGridLength(60, DataGridLengthUnitType.Pixel) },
            { "vnutr_num",      new DataGridLength(60, DataGridLengthUnitType.Pixel) },
            { "inv_num",        new DataGridLength(90, DataGridLengthUnitType.Pixel) },
            { "ser_num",        new DataGridLength(45, DataGridLengthUnitType.Pixel) },
            { "date_do",        new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "date_in",        new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "date_done",      new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "date_out",       new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "Date_prof",      new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "Date_create",    new DataGridLength(105, DataGridLengthUnitType.Pixel) },
            { "name",           new DataGridLength(220, DataGridLengthUnitType.Pixel) },
            { "login_name",     new DataGridLength(60, DataGridLengthUnitType.Pixel) },
            { "user_name",      new DataGridLength(240, DataGridLengthUnitType.Pixel) },
            { "dep_name",       new DataGridLength(200, DataGridLengthUnitType.Pixel) },
            { "pribor_name",    new DataGridLength(100, DataGridLengthUnitType.Pixel) },
            { "site_name",      new DataGridLength(90, DataGridLengthUnitType.Pixel) },
            { "jaloba",         new DataGridLength(200, DataGridLengthUnitType.Pixel) },
            { "diagnos",        new DataGridLength(200, DataGridLengthUnitType.Pixel) },
            { "repair",         new DataGridLength(300, DataGridLengthUnitType.Pixel) },
            { "MOL_name",       new DataGridLength(175, DataGridLengthUnitType.Pixel) },
            { "MOLpl_name",     new DataGridLength(100, DataGridLengthUnitType.Pixel) },
            { "Description",    new DataGridLength(320, DataGridLengthUnitType.Pixel) },
        };

        public Dictionary<string, bool> journal_column_visibility = new Dictionary<string, bool>()
        {
            { "number",         true },
            { "icon",           true },
            { "code_op",        true },
            { "sklad",          true },
            { "compl_num",      true },
            { "vnutr_num",      true },
            { "inv_num",        true },
            { "ser_num",        true },
            { "date_do",        true },
            { "date_in",        false },
            { "date_done",      false },
            { "date_out",       false },
            { "Date_prof",      true },
            { "Date_create",    true },
            { "name",           true },
            { "login_name",     true },
            { "user_name",      true },
            { "dep_name",       false },
            { "pribor_name",    false },
            { "site_name",      true },
            { "jaloba",         false },
            { "diagnos",        false },
            { "repair",         false },
            { "MOL_name",       true },
            { "MOLpl_name",     false },
            { "Description",    true },
        };

        public Dictionary<string, bool> remont_column_visibility = new Dictionary<string, bool>()
        {
            { "number",         true },
            { "icon",           true },
            { "code_op",        false },
            { "sklad",          false },
            { "compl_num",      false },
            { "vnutr_num",      false },
            { "inv_num",        true },
            { "ser_num",        false },
            { "date_do",        false },
            { "date_in",        true },
            { "date_done",      true },
            { "date_out",       true },
            { "Date_prof",      false },
            { "Date_create",    false },
            { "name",           true },
            { "login_name",     false },
            { "user_name",      true },
            { "dep_name",       true },
            { "pribor_name",    true },
            { "site_name",      false },
            { "jaloba",         true },
            { "diagnos",        true },
            { "repair",         true },
            { "MOL_name",       false },
            { "MOLpl_name",     false },
            { "Description",    false },
        };

        public Dictionary<string, bool> sklad_column_visibility = new Dictionary<string, bool>()
        {
            { "number",         true },
            { "icon",           true },
            { "code_op",        false },
            { "sklad",          false },
            { "compl_num",      true },
            { "vnutr_num",      true },
            { "inv_num",        true },
            { "ser_num",        false },
            { "date_do",        false },
            { "date_in",        false },
            { "date_done",      false },
            { "date_out",       false },
            { "Date_prof",      false },
            { "Date_create",    false },
            { "name",           true },
            { "login_name",     true },
            { "user_name",      true },
            { "dep_name",       true },
            { "pribor_name",    false },
            { "site_name",      true },
            { "jaloba",         false },
            { "diagnos",        false },
            { "repair",         false },
            { "MOL_name",       true },
            { "MOLpl_name",     true },
            { "Description",    true },
        };
    }
}
