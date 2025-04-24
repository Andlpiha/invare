using System;
using FirebirdSql.Data.FirebirdClient;
using Inv.Views;
using MsgBox;

// Класс реализует паттерн Singleton. Одно соединение используется для всех классов
public class SQLConn 
{
    private static FbConnectionStringBuilder connString = new FbConnectionStringBuilder
    {
        DataSource = "",
        Port = 3050,
        Database = "",
        Password = "redsh0v",
        UserID = "SYSDBA",
        ServerType = FbServerType.Default,
    };
    private readonly FbConnection objConnect = new();

    private readonly static SQLConn _instance;
    private SQLConn() {}

    static SQLConn()
    {
        _instance = new SQLConn();
    }
    static public SQLConn Instance
    {
        get { return _instance; } 
    }

    public bool setDatabase(string serverAddress, string databaseFile)
    {
        connString.DataSource = serverAddress;
        connString.Database = databaseFile;

        objConnect.ConnectionString = connString.ToString();
        return true;
    }

    public string GetConnectionString() => connString.ToString();

    // TODO: сделать проверку на подключение
    public FbConnection GetConnection() => objConnect;

    public void OpenConnection() => objConnect.Open();
    public void CloseConnection() => objConnect.Close();
}