using Inv.Models;
using Inv.ViewModels.MainWindow;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Inv.ViewModels.Forms
{
    public class CreateComplectViewModel : ItemForm
    {
        public CreateComplectViewModel(int sklad)
            : base()
        {
            this.vnutr_num = (ItemModel.GetLastVnutrNum(false) + 1).ToString();
            this.sklad = sklad;

            this.NameVisible = false;
        }

        protected override TableRow? handleOkBtnClick()
        {
            if (!ItemModel.CreateItem(this, false)) return null;

            TableRow tr = new();

            tr.icon = 1;
            tr.vnutr_num = this.vnutr_num == String.Empty ? null : Int32.Parse(this.vnutr_num);
            tr.inv_num = this.vnutr_num;
            tr.ser_num = this.ser_num == String.Empty ? null : Int32.Parse(this.ser_num);
            tr.login_name = this.login_name;
            tr.user_name = this.user_id;
            tr.name = this.sp_id;
            tr.inv_num = this.inv_num;
            tr.date_prof = this.Date_prof;
            tr.MOL_name = this.MOL_name;
            tr.MOLpl_name = this.MOLpl_name;
            tr.Description = this.Description;

            return tr;
        }
    }

    public class CreateComponentViewModel : ItemForm
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compl_last_vnutr">Если компонент создается в комплекте, то нужно передать самый большой vnutr_num в этом комплекте</param>
        public CreateComponentViewModel(int sklad, int? compl_vnutr = null)
            :base()
        {
            this.sklad = sklad;
            if (compl_vnutr != null)
                this.vnutr_num = compl_vnutr.ToString();
            else
                this.vnutr_num = (ItemModel.GetLastVnutrNum(true) + 1).ToString();

            this.LoginVisible = false;
            this.SiteVisible = false;
        }

        protected override TableRow? handleOkBtnClick()
        {
            if (!ItemModel.CreateItem(this, true)) return null;

            TableRow tr = new();

            tr.icon = 2;
            tr.vnutr_num = this.vnutr_num == String.Empty ? null : Int32.Parse(this.vnutr_num);
            tr.ser_num = this.ser_num == String.Empty ? null : Int32.Parse(this.ser_num);
            tr.login_name = this.login_name;
            tr.user_name = this.user_id;
            tr.name = this.name;
            tr.inv_num = this.inv_num;
            tr.date_prof = this.Date_prof;
            tr.MOL_name = this.MOL_name;
            tr.MOLpl_name = this.MOLpl_name;
            tr.Description = this.Description;

            return tr;
        }
    }

}
