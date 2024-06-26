using System;
using FirebirdSql.Data.FirebirdClient;

public class SQLConn 
{
    private static string connString = "" +
        "Data Source=localhost;" +
        "Database=C:\\Programming\\Pascal\\data\\inv.fdb;" +
        "User Id=SYSDBA;" +
        "Password=redshr0v;";
    private readonly FbConnection objConnect = new();

    private readonly static SQLConn _instance;
    private SQLConn()
    {

    }
    static SQLConn()
    {
        _instance = new SQLConn();
    }
    static public SQLConn Instance
    {
        get { return _instance; } 
    }

    private string GetConnectionString() => connString;
    private FbConnection GetConnection() => objConnect;
}