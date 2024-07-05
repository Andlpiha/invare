using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inv.ViewModels.Forms
{
    public class ItemForm : ViewModelBase
    {
        public string vnutr_num { get; set; } = String.Empty;
        public string ser_num { get; set; } = String.Empty;
        public string login_name { get; set; } = String.Empty;
        public string mol_num { get; set; } = String.Empty;
        public string inv_num { get; set; } = String.Empty;
        public DateTime? Date_prof { get; set; } = DateTime.Today;
        public string sp_id { get; set; } = String.Empty;
        public string site_id { get; set; } = String.Empty;
        public string MOL_name { get; set; } = String.Empty;
        public string MOLpl_name { get; set; } = String.Empty;

        public string user_id { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;

    }
}
