using Avalonia.Threading;
using FirebirdSql.Data.FirebirdClient;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inv.Models
{
    public class TableModel
    {
        public class TransactionReader
        {
            private FbConnection connection;
            private FbTransaction transaction;
            private FbCommand command;
            private FbDataReader reader;

            public TransactionReader(FbConnection con, string query)
            {
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    Dispatcher.UIThread.InvokeAsync(
                        () => MessageBox.Show(null, "БД недоступна", "Ошибка", MessageBox.MessageBoxButtons.Ok)
                    );
                    return;
                }

                connection = con;
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

        // Не забыть сделать transaction.Commit после обработки всех столбцов
        static private readonly string getSkladItemsQuery = "SELECT * FROM view_cm({0}) {1}";
        public TransactionReader getSkladReader(string tabID, string compl_id = "")
        {
            // Эта модель использует отделюные подключение к базе данных, потому что
            // в коде эти методы вызываются асинхронно, и использование одного подключения
            // для нескольких транзакций вызывает ошибку
            var con = new FbConnection();
            con.ConnectionString = SQLConn.Instance.GetConnectionString();

            string query;
            if (compl_id != "")
                query = string.Format(getSkladItemsQuery, tabID, "WHERE Compl_id="+compl_id);
            else
                query = string.Format(getSkladItemsQuery, tabID, "");

            TransactionReader reader = new TransactionReader(con, query);

            return reader;
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
