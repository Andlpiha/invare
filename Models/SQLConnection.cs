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
        Password = "TPeyFYN6SGRuu5eH",
        UserID = "Initial",
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

        objConnect.Close();
        objConnect.ConnectionString = connString.ToString();

        return true;
    }

    public bool setUser(string userName, string password)
    {
        connString.UserID = userName;
        connString.Password = password;

        objConnect.Close();
        objConnect.ConnectionString = connString.ToString();

        return true;
    }

    public string GetConnectionString() => connString.ToString();

    public bool isOpen() => objConnect.State == System.Data.ConnectionState.Open;

    public FbConnection GetConnection() => objConnect;

    public void OpenConnection() => objConnect.Open();
    public void CloseConnection() => objConnect.Close();
}