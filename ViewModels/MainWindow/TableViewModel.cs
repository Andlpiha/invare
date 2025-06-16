using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using FirebirdSql.Data.FirebirdClient;
using Inv.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Primitives;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inv.ViewModels.MainWindow
{
    public class TableRow
    {
        public int id { get; set; }
        public string tabId { get; set; }

        public int? icon { get; set; } = null;
        public int? code_op { get; set; } = null;
        public int? sklad { get; set; } = null;
        public int? compl_id { get; set; } = null;
        public int? mat_id { get; set; } = null;
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
        public int? prof_interval { get; set; } = null;
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

        public TableRow() { }
        public TableRow(TableRow other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            this.id = other.id;
            this.icon = other.icon;
            this.code_op = other.code_op;
            this.sklad = other.sklad;
            this.compl_num = other.compl_num;
            this.mat_num = other.mat_num;
            this.vnutr_num = other.vnutr_num;
            this.inv_num = other.inv_num;
            this.ser_num = other.ser_num;
            this.date_do = other.date_do;
            this.user_do = other.user_do;
            this.date_in = other.date_in;
            this.date_done = other.date_done;
            this.date_out = other.date_out;
            this.date_prof = other.date_prof;
            this.date_create = other.date_create;
            this.name = other.name;
            this.login_name = other.login_name;
            this.user_name = other.user_name;
            this.dep_name = other.dep_name;
            this.pribor_name = other.pribor_name;
            this.site_name = other.site_name;
            this.jaloba = other.jaloba;
            this.diagnos = other.diagnos;
            this.repair = other.repair;
            this.MOL_name = other.MOL_name;
            this.MOLpl_name = other.MOLpl_name;
            this.Description = other.Description;
        }
    }

    public class TableViewModel : ReactiveObject
    {
        public List<String> addedIds = new();
        private string _currentId = String.Empty;
        public string CurrentTabId
        {
            get => _currentId;
            set => this.RaiseAndSetIfChanged(ref _currentId, value);
        }

        // Данные, хранимые в памяти
        public SourceCache<TableRow, long> cachedCollection = new(row => row.id);
        // Объект, к которому привязана таблица
        public ReadOnlyObservableCollection<TableRow> item_source;
        public IDisposable filterRule;

        // Временная коллекция, в которой храняться данные определенных запросов
        public ObservableCollection<TableRow> temp_collection = new();
        public TableRow? selectedRow { get; set; }
        public bool bottomLevel = false;

        public bool fuzzySearchEnabled = false;
        private string _searchString = string.Empty;
        public string SearchString
        {
            get => _searchString;
            set => this.RaiseAndSetIfChanged(ref _searchString, value);
        }

        private TableModel _model;
        public TableViewModel()
        {
            _model = new TableModel();

            var filterObservable = GetFilterObservable(); // Определяем, когда нужно заново применять фильтр
            filterRule = cachedCollection
                .Connect()
                .Filter(filterObservable, (state,row) => shouldFilter(row))
                .ObserveOn(new AvaloniaSynchronizationContext(Dispatcher.UIThread, DispatcherPriority.Render)) // Обрабатываем изменения только на основном потоке
                .Bind(out item_source)
                .DisposeMany()
                .Subscribe();
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
                        () => cachedCollection.AddOrUpdate(new_row),
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
                        () => cachedCollection.AddOrUpdate(new_row),
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
                        () => cachedCollection.AddOrUpdate(new_row),
                        DispatcherPriority.Render
                    );
                }

                // Завершаем транзакцию
                transaction.closeConnection();
            }
        }

        private IObservable<(string CurrentId, string SearchString)> GetFilterObservable()
        {
            var currentIdObservable = this.WhenAnyValue(x => x.CurrentTabId)
                                  .DistinctUntilChanged();

            var searchStringObservable = this.WhenAnyValue(x => x.SearchString)
                                            .Throttle(TimeSpan.FromMilliseconds(200))
                                            .DistinctUntilChanged();

            // Combine them but keep their individual throttling behaviors
            return Observable.CombineLatest(
                currentIdObservable,
                searchStringObservable,
                (id, search) => (id, search)
            );
        }

        // Решаем, нужно ли отображать элемент в таблице
        private bool shouldFilter(TableRow row)
        {
            if(row.tabId != CurrentTabId)
                return false;

            if (SearchString == String.Empty) return true;
            if (row == null || string.IsNullOrWhiteSpace(SearchString))
                return false;

            // Фильтруем по каждому столбцу
            foreach (var prop in typeof(TableRow).GetProperties())
            {
                if (!should_search_column[prop.Name])
                    continue;

                object? value = prop.GetValue(row);
                if (value == null) continue;
                string stringValue;
                Type propType = prop.PropertyType;

                if (propType == typeof(int?))
                {
                    int? intVal = (int?)value;
                    stringValue = intVal.Value.ToString(CultureInfo.InvariantCulture);
                }
                else if (propType == typeof(DateTime?))
                {
                    DateTime? dateVal = (DateTime?)value;
                    stringValue = dateVal.Value.ToString("o", CultureInfo.InvariantCulture);
                }
                else if (propType == typeof(string))
                {
                    stringValue = (string)value;
                }
                else
                {
                    continue; // Пропускаем неподдерживаемые типы
                }

                if (stringValue.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }

        //Отображение комплекта
        public void updateRowsFromComplect(string tabID, int compl_id)
        {
            DataTable table = _model.getComplectTable(tabID, compl_id.ToString());
            DataTableReader reader = table.CreateDataReader();

            Dispatcher.UIThread.InvokeAsync(
                () => temp_collection.Clear(),
                DispatcherPriority.Render
            );
            while (reader.Read())
            {
                var new_row = new TableRow();

                new_row.id = (int)reader["id"];
                new_row.tabId = reader["sklad"].ToString()!;
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
                    () => temp_collection.Add(new_row),
                    DispatcherPriority.Render
                );
            }
        }

        private TableRow TableRowForRepair(FbDataReader reader)
        {
            var new_row = new TableRow();

            new_row.id = (int)reader["id"];
            new_row.tabId = Global.RepairTab;
            new_row.icon = reader["icon"] as int?;
            new_row.compl_id = reader["compl_id"] as int?;
            new_row.mat_id = reader["mat_id"] as int?;

            new_row.compl_num = !reader.IsDBNull("compl_num") ? reader.GetInt32("compl_num"): (int?)null;
            new_row.vnutr_num = !reader.IsDBNull("vnutr_num") ? reader.GetInt32("vnutr_num") : (int?)null;
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
            new_row.tabId = reader["sklad"].ToString()!;
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
            new_row.prof_interval = reader["prof_interval"] as int?;
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
            { "date_do",        new DataGridLength(75, DataGridLengthUnitType.Pixel) },
            { "date_in",        new DataGridLength(75, DataGridLengthUnitType.Pixel) },
            { "date_done",      new DataGridLength(75, DataGridLengthUnitType.Pixel) },
            { "date_out",       new DataGridLength(75, DataGridLengthUnitType.Pixel) },
            { "Date_prof",      new DataGridLength(75, DataGridLengthUnitType.Pixel) },
            { "Date_create",    new DataGridLength(75, DataGridLengthUnitType.Pixel) },
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
            { "number",         true  },
            { "icon",           true  },
            { "code_op",        false },
            { "sklad",          true  },
            { "compl_num",      true  },
            { "vnutr_num",      true  },
            { "inv_num",        true  },
            { "ser_num",        true  },
            { "date_do",        true  },
            { "date_in",        false },
            { "date_done",      false },
            { "date_out",       false },
            { "Date_prof",      false  },
            { "Date_create",    true },
            { "name",           true  },
            { "login_name",     false },
            { "user_name",      false },
            { "dep_name",       false },
            { "pribor_name",    false },
            { "site_name",      false },
            { "jaloba",         false },
            { "diagnos",        false },
            { "repair",         false },
            { "MOL_name",       false },
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
            { "Date_prof",      true },
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

        private Dictionary<string, bool> should_search_column = new Dictionary<string, bool>()
        {
			{ "id", 		false },
			{ "tabId", 		false },
            { "icon", 		false },
			{ "code_op", 	false },
			{ "sklad",      false },
			{ "compl_id",      false },
			{ "mat_id",      false },
            { "compl_num",  false },
			{ "mat_num",    false },
			{ "vnutr_num",  true },
			{ "inv_num",    true },
			{ "ser_num",    true },
			{ "date_do",    false },
			{ "user_do",    false },
			{ "date_in",    true },
			{ "date_done",  true },
			{ "date_out",   true },
			{ "date_prof",  false },
			{ "prof_interval",  false },
            { "date_create", false },
			{ "name",       true },
			{ "login_name", true },
			{ "user_name",  true },
			{ "dep_name",   true },
			{ "pribor_name", true },
			{ "site_name",  false },
			{ "jaloba",     false },
			{ "diagnos",    false },
			{ "repair",     false },
			{ "MOL_name",   true },
			{ "MOLpl_name", true },
			{ "Description", true }
        };
    }
}
