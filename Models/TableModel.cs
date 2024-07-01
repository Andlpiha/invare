using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inv.Models
{
    internal class TableModel
    {
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public List<string>? WhereClause { get; set; }

        public TableModel() {}

        static private readonly string getTabsQuery = "SELECT s.NAME FROM SPR s " +
            "LEFT JOIN SPR ss ON ss.id=s.id_up " +
            "WHERE ss.Name = 'Состояния'";
        static public List<string> getTabs(FbConnection con)
        {
            if (con == null || con.State != ConnectionState.Open)
                return new List<string>();

            DataTable tabs_table = new DataTable();
            FbCommand _cmd = new(getTabsQuery, con);
            (new FbDataAdapter(_cmd)).Fill(tabs_table);

            // Берем нужный нам столбец и конвертируем его в массив
            List<string> tabs = tabs_table.Rows.OfType<DataRow>()
                .Select(dr => dr.Field<string>("Name")).OfType<string>().ToList();

            return tabs;
        }

        static private readonly string getTabContentQuery = "";
        public DataTable getTabContent()
        {
            DataTable table = new DataTable();

            return table;
        }
    }
}
