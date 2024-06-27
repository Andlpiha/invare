using FirebirdSql.Data.FirebirdClient;
using System.Data;
using System.Drawing;

namespace Inv.Models
{
    internal class UserModel
    {
        DataTable users;

        public UserModel() 
        {
            users = new DataTable();
        }

        private string fetchUsersQuery = "select * from us";
        public bool FetchUsers(FbConnection con)
        {
            if (con == null || con.State != ConnectionState.Open)
                return false;

            FbCommand _cmd = new(fetchUsersQuery, con);
            (new FbDataAdapter(_cmd)).Fill(users);

            return true;
        }
    }
}
