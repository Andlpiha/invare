using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace Inv.Models
{
    public class SprModel
    {
        static public DataTable getAllRows()
        {
            var con = SQLConn.Instance.GetConnection();

            DataTable rows_table = new DataTable();
            FbCommand _cmd = new("SELECT ID, ID_UP, UR, ICON, NAME, DESCRIPTION, CAP, POSIT FROM SPR ORDER BY ID_UP ASC, NAME ASC", con);
            (new FbDataAdapter(_cmd)).Fill(rows_table);

            return rows_table;
        }

        static public DataTable getChildren(int ID)
        {
            var con = SQLConn.Instance.GetConnection();

            DataTable children = new DataTable();
            FbCommand _cmd = new($"SELECT " +
                $"ID, ID_UP, UR, ICON, NAME, DESCRIPTION, CAP, POSIT FROM SPR " +
                $"WHERE ID_UP={ID} ",
                con);
            (new FbDataAdapter(_cmd)).Fill(children);

            return children;
        }

        private static string getTechniciansQuery = "SELECT name FROM spr WHERE id_up IN (SELECT id FROM spr WHERE name='Ремонтники')";
        public static DataTable? getTechnicians(FbConnection con)
        {
            if (con == null || con.State != ConnectionState.Open)
                return null;

            DataTable technicians = new DataTable();
            FbCommand _cmd = new(getTechniciansQuery, con);
            (new FbDataAdapter(_cmd)).Fill(technicians);

            return technicians;
        }

        // Последний созданный ID в таблице SPR (без инкремента)
        static public int getSprGen()
        {
            var con = SQLConn.Instance.GetConnection();
            FbCommand _cmd = new("SELECT GEN_ID(SPR_GEN, 0) FROM RDB$DATABASE", con);

            return (int)_cmd.ExecuteScalar();
        }

        static public void incrementSprGen()
        {
            var con = SQLConn.Instance.GetConnection();
            FbCommand _cmd = new("SELECT GEN_ID(SPR_GEN, 1) FROM RDB$DATABASE", con);
            _cmd.ExecuteNonQuery();
        }
    }
}
