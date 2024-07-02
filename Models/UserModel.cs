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
        DataTable users;

        public UserModel() 
        {
            users = new DataTable();
        }

        private string fetchUsersQuery = "SELECT * FROM us";
        public bool FetchUsers(FbConnection con)
        {
            if (con == null || con.State != ConnectionState.Open)
                return false;

            FbCommand _cmd = new(fetchUsersQuery, con);
            (new FbDataAdapter(_cmd)).Fill(users);

            return true;
        }

        public uint getTY(string login)
        {
            DataRow[] result = users.Select(string.Format("LOGIN = '{0}'", login));

            if (result.Length == 0)
                return uint.MaxValue;

            return Convert.ToUInt32(result.First()["TY"]);
        }
    }
}
