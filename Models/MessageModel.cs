using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using FirebirdSql.Data.FirebirdClient;
using Inv.ViewModels.Forms;

namespace Inv.Models
{
    internal class MessageModel
    {
        public static readonly string getMessagesQuery = @"
            SELECT SENDER_NAME, SENDER_LOGIN, MSG_TEXT, MSG_TIME 
            FROM MSG 
            WHERE REPAIR_ID = @repair_id
                AND ( SENDER_LOGIN = @user_login OR RECEIVER_LOGIN = @user_login )
            ORDER BY MSG_TIME DESC";
        public static DataTable getMessages(int repair_id, FbConnection con)
        {
            using var cmd = new FbCommand(getMessagesQuery, con);
            cmd.Parameters.AddWithValue("@repair_id", repair_id);
            cmd.Parameters.AddWithValue("@user_login", Global.Login);

            DataTable table = new();
            (new FbDataAdapter(cmd)).Fill(table);

            return table;
        }

        public static readonly string addMessageQuery = @"
            INSERT INTO MSG (ID, SENDER_LOGIN, SENDER_NAME, RECEIVER_LOGIN, MSG_TEXT, REPAIR_ID)
            VALUES (NEXT VALUE FOR GEN_MSG_ID, @sender_login, @sender_name, @receiver_login, @msg_text, @repair_id)";
        public static bool addMessage(int repair_id, string recipient, string text, FbConnection con)
        {
            using var cmd = new FbCommand(addMessageQuery, con);

            cmd.Parameters.AddWithValue("@sender_login", Global.Login);
            cmd.Parameters.AddWithValue("@sender_name", Global.Name);
            cmd.Parameters.AddWithValue("@receiver_login", recipient);
            cmd.Parameters.AddWithValue("@msg_text", text);
            cmd.Parameters.AddWithValue("@repair_id", repair_id);

            return cmd.ExecuteNonQuery() == 1;
        }
    }
}
