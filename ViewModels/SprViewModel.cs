using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using DynamicData;
using Inv.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ReactiveUI;

namespace Inv.ViewModels
{
    public class SpRowData : ReactiveObject
    {
        public required int ID { get; set; }
        public required int ID_up { get; set; }
        public string Name_up { get; set; } = string.Empty;
        public bool UR { get; set; } = false;

        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Cap { get; set; }
        public int Posit { get; set; }
        public int Icon { get; set; }

        public ObservableCollection<SpRowData> Children { get; set; } = new();

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }
    };

    public class SprViewModel : ViewModelBase
    {
        private ObservableCollection<SpRowData> _spRows;
        public HierarchicalTreeDataGridSource<SpRowData> DataGridSource { get; }

        public SprViewModel()
        {
            DataTable rootRows = SprModel.getChildren(0);

            _spRows = new ObservableCollection<SpRowData>
            {
                rootRows.AsEnumerable()
                    .Select(r => convertRaw(r))
                    .OrderBy(r => r.Name)
                    .ToList()
            };

            DataGridSource = new HierarchicalTreeDataGridSource<SpRowData>(_spRows)
            {
                Columns =
                {
                    new HierarchicalExpanderColumn<SpRowData> (
                        new TemplateColumn<SpRowData>(
                            null,
                            "DefaultColumnTemplate",
                            null,
                            new GridLength(1, GridUnitType.Star)
                        ),
                        x => loadChildren(x), // Потомки загружаются динамически(когда раскрывается шеврон)
                        x => x.UR, // Отображать ли шеврон
                        x => x.IsExpanded
                    )
                }
            };
        }

        private SpRowData convertRaw(DataRow row)
        {
            SpRowData returnValue = new SpRowData
            {
                ID = row.Field<int>("ID"),
                ID_up = row.Field<int>("ID_UP"),
                UR = row.Field<int>("UR") == 1, // Я не знаю почему в БД это int, приходится вот так конвертировать
                Name = row.Field<string>("name") ?? string.Empty,
                Description = row.Field<string>("description") ?? string.Empty,
                Cap = row.Field<decimal>("cap"),
                Posit = row.Field<int?>("posit") ?? 0,
                Icon = row.Field<int>("icon")
            };

            return returnValue;
        }

        private ObservableCollection<SpRowData> loadChildren(SpRowData row)
        {
            if(row.Children.Count != 0)
                return row.Children;

            DataTable childrenRaw = SprModel.getChildren(row.ID);

            row.Children = new ObservableCollection<SpRowData>
            {
                childrenRaw.AsEnumerable()
                    .Select(r => convertRaw(r))
                    .OrderBy(r => r.Name)
                    .ToList()
            };

            return row.Children;
        }
    }
}