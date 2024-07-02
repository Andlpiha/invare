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
        public TableModel() {}

        static private readonly string getTabsQuery = "SELECT * FROM SPR s " +
            "LEFT JOIN SPR ss ON ss.id=s.id_up " +
            "WHERE ss.Name = 'Состояния'";
        static public DataTable getTabs()
        {
            var con = SQLConn.Instance.GetConnection();

            DataTable tabs_table = new DataTable();
            FbCommand _cmd = new(getTabsQuery, con);
            (new FbDataAdapter(_cmd)).Fill(tabs_table);

            // Берем нужный нам столбец и конвертируем его в массив
            List<string> tabs = tabs_table.Rows.OfType<DataRow>()
                .Select(dr => dr.Field<string>("Name")).OfType<string>().ToList();

            return tabs_table;
        }

        // TODO: добавить сортировку по столбцу (TMain_Form.SkladGridSort)
        static private readonly string getSkladItemsQuery = "SELECT * FROM view_cm({0}) {1}";
        public DataTable getSkladItems(string tabID, string compl_id = "")
        {
            var con = SQLConn.Instance.GetConnection();

            DataTable table = new DataTable();

            string query;
            if (compl_id != "")
                query = string.Format(getSkladItemsQuery, tabID, "WHERE Compl_id="+compl_id);
            else
                query = string.Format(getSkladItemsQuery, tabID, "");

            FbCommand _cmd = new(query, con);
            (new FbDataAdapter(_cmd)).Fill(table);

            return table;
        }

        static private readonly string getRepairsQuery = "";
        public DataTable getRepairs()
        {
            var con = SQLConn.Instance.GetConnection();

            DataTable table = new DataTable();

            return table;
        }

        static private readonly string getLogQuery = "";
        public DataTable getLog()
        {
            var con = SQLConn.Instance.GetConnection();

            DataTable table = new DataTable();

            return table;
        }
    }
}
