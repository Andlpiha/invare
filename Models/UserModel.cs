using Aspose.Email.Clients.Imap;
using FirebirdSql.Data.FirebirdClient;
using Inv.Properties;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Inv.Models
{
    internal class UserModel
    {
        public string? dbLogin {  get; set; }
        public string? dbPassword {  get; set; }
        public string? userName {  get; set; }
        public int? userTy {  get; set; }


        private static readonly string authQuery = "SELECT * FROM auth_proc(@login, @password)";
        public bool doAuth(string login, string password, FbConnection conn)
        { 
            var cmd = new FbCommand(authQuery, conn);

            cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = login;
            cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;

            DataTable userData = new();
            (new FbDataAdapter(cmd)).Fill(userData);

            DataRow firstRow = userData.Rows[0];

            dbLogin = firstRow.Field<string>("db_user_login");
            dbPassword = firstRow.Field<string>("db_user_password");
            userName = firstRow.Field<string>("user_name");
            userTy = firstRow.Field<int?>("ty");

            if(dbLogin == null || dbPassword == null || userName == null || userTy == null)
                return false;
            return true;
        }
    }
}
