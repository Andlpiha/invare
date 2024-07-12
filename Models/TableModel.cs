using Avalonia.Threading;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Inv.Models
{
    public class TableModel
    {
        // Открывает транзакцию м предоставляет методы по управлению ей
        public class TransactionReader
        {
            private FbConnection connection;
            private FbTransaction transaction;
            private FbCommand command;
            private FbDataReader reader;

            public TransactionReader(FbConnection con, string query)
            {
                connection = con;
                if (connection.State != ConnectionState.Open){
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception e)
                    {
                        Dispatcher.UIThread.InvokeAsync(
                            () => MessageBox.Show(null, "БД недоступна", "Ошибка", MessageBox.MessageBoxButtons.Ok)
                        );
                        return;
                    }
                }

                transaction = connection.BeginTransaction();
                command = new FbCommand(query, con, transaction);
                reader = command.ExecuteReader();
            }

            public bool isOpen()
            {
                return connection.State == ConnectionState.Open;
            }

            public void closeConnection()
            {
                transaction.Commit();
                connection.Close();
            }

            public FbDataReader getReader()
            {
                return reader;
            }
        }

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

        public TransactionReader getSkladReader(string tabID, string compl_id = "")
        {
            // Эта модель использует отдельное подключение к базе данных, потому что
            // в коде эти методы вызываются асинхронно, и использование одного подключения
            // для нескольких транзакций вызывает ошибку
            var con = new FbConnection();
            con.ConnectionString = SQLConn.Instance.GetConnectionString();

            string query = "";
            if (Global.BottomLevel)
            {
                query += string.Format("SELECT * FROM mat_s({0})", tabID);
                if (Global.AU) // поиск с исполнителем (прибористом) и без даты выдачи
                    query += string.Format("where compl_id in (select compl_id from rem where pribor_name=\"{0}\" and date_out is null)", Global.Login);
                if (compl_id != "")
                {
                    if (Global.AU)
                        query += "AND compl_id=" + compl_id;
                    else
                        query += "WHERE compl_id=" + compl_id;
                }
            }
            else
            {
                query += string.Format("SELECT * FROM view_cm({0})", tabID);
                if (Global.AU) // поиск с исполнителем (прибористом) и без даты выдачи
                {
                    query += string.Format("WHERE id IN (SELECT compl_id FROM rem WHERE pribor_name=\"{0}\" AND date_out IS NULL)" +
                        "or id in (select mat_id from rem where pribor_name=\"{0}\" and date_out is null)", Global.Login);
                }
            }   

            return new TransactionReader(con, query);
        }

        public TransactionReader getRepairsReader()
        {
            // Эта модель использует отдельное подключение к базе данных, потому что
            // в коде эти методы вызываются асинхронно, и использование одного подключения
            // для нескольких транзакций вызывает ошибку
            var con = new FbConnection();
            con.ConnectionString = SQLConn.Instance.GetConnectionString();

            string query = "";
            if (Global.AU)
                query += $"SELECT * FROM rem WHERE pribor_name IS NULL OR pribor_name={Global.Login} AND " +
                    $"date_done IS NULL OR date_out IS NULL";
            else
                query += "SELECT * FROM rem WHERE date_done IS NULL OR date_out IS NULL";

            return new TransactionReader(con, query);
        }

        public TransactionReader getLogReader()
        {
            // Эта модель использует отдельное подключение к базе данных, потому что
            // в коде эти методы вызываются асинхронно, и использование одного подключения
            // для нескольких транзакций вызывает ошибку
            var con = new FbConnection();
            con.ConnectionString = SQLConn.Instance.GetConnectionString();

            //TODO: getLogReader
            string query = "";
            if (Global.TopLevel)
            {
                query = "SELECT FIRST 50000 id, icon, date_do, user_do, code_op, mat_id, sklad," +
                    $"compl_num, inv_num, ser_num, vnutr_num, name, user_name," +
                    $"description, date_prof, date_create, MOL_name FROM view_log ORDER BY date_do DESCENDING";
            }
            else
            {
                if (Global.CurCompl != null && Global.CurCompl.icon == Global.ComplectIcon)
                {
                    query = $"SELECT id, 1 icon, date_do, user_do, code_op, compl_id, Sklad," +
                        $"vnutr_num compl_num, inv_num, ser_num, vnutr_num, name, user_name," +
                        $"description, date_prof, date_create, MOL_name from compl_log" +
                        $"WHERE compl_id={Global.CurCompl!.compl_num} ORDER BY date_do DESCENDING";
                }
                else
                {
                    query = $"SELECT id, 2 icon, date_do, user_do, code_op, mat_id, Sklad," +
                        $"Compl_num, inv_num, ser_num, vnutr_num, sp_name, user_name," +
                        $"description, date_prof, date_create, MOL_name from mat_log" +
                        $"WHERE mat_id={Global.CurCompl!.mat_num} ORDER BY date_do DESCENDING";
                }
            }
            return new TransactionReader(con, query);
        }

        public DataTable getComplectTable(string tabID, string compl_id = "")
        {
            var con = SQLConn.Instance.GetConnection();

            var query = $"SELECT * FROM mat_s({tabID})";
            if (compl_id != "")
                query += $" WHERE Compl_id={compl_id}";

            DataTable complect_table = new DataTable();
            FbCommand _cmd = new(query, con);
            (new FbDataAdapter(_cmd)).Fill(complect_table);

            return complect_table;
        }
    }
}
