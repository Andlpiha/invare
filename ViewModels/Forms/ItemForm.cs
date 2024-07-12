using Inv.ViewModels.MainWindow;
using Microsoft.Extensions.Options;
using MsgBox;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Inv.ViewModels.Forms
{
    public abstract class ItemForm : ViewModelBase
    {
        public string vnutr_num { get; set; } = String.Empty;
        public string ser_num { get; set; } = String.Empty;
        public string login_name { get; set; } = String.Empty;
        public string inv_num { get; set; } = String.Empty;
        public DateTime? Date_prof { get; set; } = DateTime.Today;
        public string sp_id { get; set; } = String.Empty;
        public string site_id { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string MOL_name { get; set; } = String.Empty;
        public string MOLpl_name { get; set; } = String.Empty;

        public string user_id { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;

        // Extras
        public int sklad { get; set; } = -1;
        public int compl_id { get; set; } = -1;

        protected ItemForm()
        {
            okBtnClickCommand = ReactiveCommand.Create(() =>
            {
                if (!validate()) return null;
                return handleOkBtnClick();
            });

            cancelBtnClickCommand = ReactiveCommand.Create(() =>
            {
                return handleCancelBtnClick();
            });
        }

        protected abstract TableRow? handleOkBtnClick();
        protected virtual TableRow? handleCancelBtnClick() => null;

        public virtual bool validate()
        {
            try
            {
                Int32.Parse(this.vnutr_num);
                Int32.Parse(this.ser_num);
            }
            catch (Exception e)
            {
                MessageBox.Show(null, "Неправильный формат внутреннего или серийного номера", "Ошибка", MessageBox.MessageBoxButtons.Ok);
                return false;
            }

            if (vnutr_num == String.Empty)
                return false;
            if (sklad == -1)
                return false;

            return true;
        }

        public ReactiveCommand<Unit, TableRow?> okBtnClickCommand { get; }
        public ReactiveCommand<Unit, TableRow?> cancelBtnClickCommand { get; }

        // Выдимость различных компонентов формы
        public bool LoginVisible { get; set; } = true;
        public bool SiteVisible { get; set; } = true;
        public bool NameVisible { get; set; } = true;
    }
}
