using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Inv.Models;
using Inv.ViewModels.MainWindow;
using ReactiveUI;

namespace Inv.ViewModels.Forms
{
    public partial class RepairForm : ObservableObject
	{
        public long? id { get; set; } = null;

		public string compl_num { get; set; } = String.Empty;
		public string vnutr_num { get; set; } = String.Empty;
		public string inv_num { get; set; } = String.Empty;
        public int? compl_id { get; set; } = null;
        public int? mat_id { get; set; } = null;

		public string name { get; set; } = String.Empty;
		public string? executor { get; set; } = null;

        [ObservableProperty]
        private string _department = String.Empty;
        [ObservableProperty]
        private string _user = Global.Name;

        public string complaint { get; set; } = String.Empty;
        public string diagnosis { get; set; } = String.Empty;
        public string actions_taken { get; set; } = String.Empty;

        public DateTime? acceptedTime { get; set; }
        public DateTime? doneTime { get; set; }
        public DateTime? returnedTime { get; set; }


        private ObservableCollection<string> _executors = [];
        public ObservableCollection<string> executors { get => _executors; }

        public RepairForm()
        {
            var getExecutors = new Task(() =>
            {
                var data = SprModel.getTechnicians(SQLConn.Instance.GetConnection());
                _executors = new ObservableCollection<string>(
                    data!.AsEnumerable().Select(row => (string)row["Name"]).ToList()
                );
            });
            getExecutors.Start();
        }

        public RepairForm(RepairForm other) : this()
        {
            id = other.id;
            compl_num = other.compl_num;
            vnutr_num = other.vnutr_num;
            inv_num = other.inv_num;
            mat_id = other.mat_id;
            name = other.name;
            executor = other.executor;
            complaint = other.complaint;
            diagnosis = other.diagnosis;
            actions_taken = other.actions_taken;

            _department = other._department;
            _user = other._user;

            acceptedTime = other.acceptedTime;
            doneTime = other.doneTime;
            returnedTime = other.returnedTime;
        }

        public RepairForm(TableRow tableRow) : this ()
        {
            id = tableRow.id;
            compl_id = tableRow.compl_id;
            mat_id = tableRow.mat_id;
            compl_num = tableRow.compl_num.ToString() ?? "";
            vnutr_num = tableRow.vnutr_num.ToString() ?? "";
            inv_num = tableRow.inv_num ?? "";
            name = tableRow.name ?? "";
            executor = tableRow.pribor_name;
            complaint = tableRow.jaloba ?? "";
            diagnosis = tableRow.diagnos ?? "";
            actions_taken = tableRow.repair ?? "";

            _department = tableRow.dep_name ?? "";
            _user = tableRow.user_name ?? "";

            acceptedTime = tableRow.date_in;
            doneTime = tableRow.date_done;
            returnedTime = tableRow.date_out;
        }

        public string validate()
		{
            try
            {
                Int32.Parse(this.compl_num);
                Int32.Parse(this.vnutr_num);
            }
            catch 
            {
                return "Неправильно набран номер комплекта, внутренний номер или инвентарный номер";
            }
            //TODO: обработать даты

            return String.Empty;
		}

        public bool Equals(RepairForm? other)
        {
            return new RepairFormComparer().Equals(this, other);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RepairForm);
        }

        public override int GetHashCode()
        {
            return new RepairFormComparer().GetHashCode(this);
        }
    }

    public class RepairFormComparer : IEqualityComparer<RepairForm>
    {
        public bool Equals(RepairForm? x, RepairForm? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            return x.compl_num == y.compl_num &&
                   x.vnutr_num == y.vnutr_num &&
                   x.inv_num == y.inv_num &&
                   x.mat_id == y.mat_id &&
                   x.name == y.name &&
                   x.executor == y.executor &&
                   x.Department == y.Department &&      
                   x.User == y.User &&                  
                   x.complaint == y.complaint &&
                   x.diagnosis == y.diagnosis &&
                   x.actions_taken == y.actions_taken &&
                   Nullable.Equals(x.acceptedTime, y.acceptedTime) &&
                   Nullable.Equals(x.doneTime, y.doneTime) &&
                   Nullable.Equals(x.returnedTime, y.returnedTime);
        }

        public int GetHashCode(RepairForm obj)
        {
            var hash = new HashCode();
            hash.Add(obj.compl_num);
            hash.Add(obj.vnutr_num);
            hash.Add(obj.inv_num);
            hash.Add(obj.mat_id);
            hash.Add(obj.name);
            hash.Add(obj.executor);
            hash.Add(obj.Department);
            hash.Add(obj.User);
            hash.Add(obj.complaint);
            hash.Add(obj.diagnosis);
            hash.Add(obj.actions_taken);
            hash.Add(obj.acceptedTime);
            hash.Add(obj.doneTime);
            hash.Add(obj.returnedTime);
            return hash.ToHashCode();
        }
    }
}