using System;
using FirebirdSql.Data.FirebirdClient;
using Inv.Views;
using MsgBox;

// Класс реализует паттерн Singleton. Одно соединение используется для всех классов
public class SQLConn 
{
    private static string connString = "" +
        "Data Source={0};" +
        "Database={1};" +
        "User Id=SYSDBA;" +
        "Password=redshr0v;";
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

    public bool setDatabase(string database)
    {
        // Разделяем строку на адрес сервера и адрес базы данных
        string[] db = database.Split(":", 2);

        if (db.Length != 2)
            return false;

        connString = string.Format(connString, db[0], db[1]);
        objConnect.ConnectionString = connString;

        return true;
    }

    public string GetConnectionString() => connString;
    public FbConnection GetConnection() => objConnect;

    public void OpenConnection() => objConnect.Open();
    public void CloseConnection() => objConnect.Close();
}