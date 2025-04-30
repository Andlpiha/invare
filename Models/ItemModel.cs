using FirebirdSql.Data.FirebirdClient;
using Inv.ViewModels.Forms;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inv.Models
{
    public class ItemModel
    {
        public static int GetLastVnutrNum(bool c_ma)
        {
            string query;
            if (c_ma) query = "SELECT vnutr_num FROM mat ORDER BY vnutr_num DESCENDING";
            else query = "SELECT vnutr_num FROM compl WHERE vnutr_num<9000 ORDER BY vnutr_num DESCENDING";

            var conn = SQLConn.Instance.GetConnection();
            FbCommand _cmd = new(query, conn);

            return (int)_cmd.ExecuteScalar();
        }

        public static string getMOL()
        {
            string query = "SELECT id FROM spr WHERE name='МОЛ' AND id_up=0";

            var conn = SQLConn.Instance.GetConnection();
            FbCommand _cmd = new(query, conn);

            return (string)_cmd.ExecuteScalar();
        }

        public static bool CreateItem(ItemForm form, bool c_ma)
        {
            string date_prof_sql = "";
            if (form.Date_prof != null)
                date_prof_sql = new SqlDateTime((DateTime)form.Date_prof).ToString();

            var conn = SQLConn.Instance.GetConnection();
            if (conn == null || conn.State != ConnectionState.Open)
                return false;

            string insert_query;
            if (c_ma)
            {
                if (form.compl_id != -1)
                {
                    var user_id_query = $"SELECT user_id FROM compl WHERE id={form.compl_id}";

                    FbCommand _cmd_usr_id = new(user_id_query, conn);
                    form.user_id = (string)_cmd_usr_id.ExecuteScalar();
                }

                var count_query = "select count(*) co from compl where vnutr_num=" + form.vnutr_num;
                FbCommand _cmd = new(count_query, conn);
                if (_cmd.ExecuteScalar() as int? > 0)
                {
                    MessageBox.Show(null,
                        "Такой внутренний №, в комплектах уже есть. Введите другой номер.",
                        "Комплект уже существует", MessageBox.MessageBoxButtons.Ok);
                    return false;
                }

                var compl_id = form.compl_id == -1 ? "null" : form.compl_id.ToString();

                insert_query = $"INSERT INTO mat(sklad, vnutr_num, inv_num, ser_num, compl_id, sp_id, user_id, name, description, date_prof, mol_id, molpl_id)" +
                    $" VALUES('{form.sklad}', '{form.vnutr_num}', '{form.inv_num}', '{form.ser_num}', '{form.vnutr_num}', '{form.sp_id}'," +
                    $" '{form.user_id}', '{form.name}', '{form.Description}', @datetime, '{form.MOL_name}', '{form.MOLpl_name}')";
            }
            else
                insert_query = $"INSERT INTO compl(sklad, vnutr_num, inv_num, ser_num, user_id, login_id, site_id, name, description, date_prof, mol_id, molpl_id) " +
                    $"VALUES('{form.sklad}', '{form.vnutr_num}', '{form.inv_num}', '{form.ser_num}', '{form.user_id}', '{form.login_name}'," +
                        $" '{form.site_id}', '{form.name}', '{form.Description}', @datetime, '{form.MOL_name}', '{form.MOLpl_name}')";

            insert_query = insert_query.Replace("''", "null");

            FbCommand _insert_cmd = new(insert_query, conn);
            _insert_cmd.Parameters.AddWithValue("@datetime", form.Date_prof);

            _insert_cmd.ExecuteNonQueryAsync();

            return true;
        }

        public static void DeleteRemontItem(long id)
        {
            var query = $"DELETE FROM rem WHERE id={id}";

            var conn = SQLConn.Instance.GetConnection();
            if (conn == null || conn.State != ConnectionState.Open)
                return;

            FbCommand _cmd = new(query, conn);
            _cmd.ExecuteNonQueryAsync();
        }

        public static void DeleteComplectItem(long id)
        {
            var query = $"DELETE FROM compl WHERE id={id}";

            var conn = SQLConn.Instance.GetConnection();
            if (conn == null || conn.State != ConnectionState.Open)
                return;

            FbCommand _cmd = new(query, conn);
            _cmd.ExecuteNonQueryAsync();
        }

        public static void DeleteMatItem(long id)
        {
            var query = $"DELETE FROM mat WHERE id={id}";

            var conn = SQLConn.Instance.GetConnection();
            if (conn == null || conn.State != ConnectionState.Open)
                return;

            FbCommand _cmd = new(query, conn);
            _cmd.ExecuteNonQueryAsync();
        }

        public static bool EditItem(ItemForm form, bool c_ma)
        {
            return true;
        }
    }
}
