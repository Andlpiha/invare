using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Inv.ViewModels.Forms;

namespace Inv.Models
{
    internal class RepairModel
    {
        private const string DuplicateQuery =
            "SELECT 1 FROM rdb$database WHERE EXISTS(SELECT 1 FROM rem " +
            "WHERE (@compl_num = '' OR compl_id = @compl_num) " +
                "AND (@mat_id = '' OR mat_id = @mat_id) " +
                "AND date_out IS NULL)";
        public static bool CheckDuplicate(RepairForm form, FbConnection conn)
        {
            var cmd = new FbCommand(DuplicateQuery, conn);

            cmd.Parameters.Add("@compl_num", SqlDbType.VarChar).Value = form.compl_num; 
            cmd.Parameters.Add("@mat_id", SqlDbType.VarChar).Value = form.mat_id;

            return (bool)cmd.ExecuteScalar();
        }

        private const string AddRepairQuery =
            "INSERT VALUES INTO REM(ICON, COMPL_ID, MAT_ID, COMPL_NUM, " +
                "VNUTR_NUM, INV_NUM, NAME, DEP_NAME, USER_NAME, PRIBOR_NAME, " +
                "JALOBA, DIAGNOS, REPAIR, DATE_IN, DATE_DONE, DATE_OUT) VALUES" +
            "(@icon, @c_id, @m_id, @c_num, @v_num, @i_num, @name, @d_name, " +
                "@u_name, @p_name, @jal, @diag, @rep, @d_in, @d_done, @d_out)";
        public static bool AddRepair(RepairForm form, FbConnection conn)
        {
            var cmd = new FbCommand(AddRepairQuery, conn);

            cmd.Parameters.Add("@icon", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@c_id", SqlDbType.Int).Value = form.compl_id;
            cmd.Parameters.Add("@m_id", SqlDbType.Int).Value = form.mat_id;
            cmd.Parameters.Add("@c_num", SqlDbType.VarChar).Value = form.compl_num;
            cmd.Parameters.Add("@v_num", SqlDbType.VarChar).Value = form.vnutr_num;
            cmd.Parameters.Add("@i_num", SqlDbType.VarChar).Value = form.inv_num;
            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = form.name;
            cmd.Parameters.Add("@d_name", SqlDbType.VarChar).Value = form.Department;
            cmd.Parameters.Add("@u_name", SqlDbType.VarChar).Value = form.User;
            cmd.Parameters.Add("@p_name", SqlDbType.VarChar).Value = form.executor;
            cmd.Parameters.Add("@jal", SqlDbType.VarChar).Value = form.complaint;
            cmd.Parameters.Add("@diag", SqlDbType.VarChar).Value = form.diagnosis;
            cmd.Parameters.Add("@rep", SqlDbType.VarChar).Value = form.actions_taken;
            cmd.Parameters.Add("@d_in", SqlDbType.Timestamp).Value = form.acceptedTime;
            cmd.Parameters.Add("@d_done", SqlDbType.Timestamp).Value = form.doneTime;
            cmd.Parameters.Add("@d_out", SqlDbType.Timestamp).Value = form.returnedTime;

            var rows_affected = cmd.ExecuteNonQuery();
            return rows_affected > 0;
        }

        private const string EditRepairQuery =
            "UPDATE REM " +
            "SET ICON=@icon, COMPL_ID=@c_id, MAT_ID=@m_id, COMPL_NUM=@c_num, " +
                "VNUTR_NUM=@v_num, INV_NUM=@i_num, NAME=@name, DEP_NAME=@d_name, USER_NAME=@u_name, " +
                "PRIBOR_NAME=@p_name, JALOBA=@jal, DIAGNOS=@diag, REPAIR=@rep, DATE_IN=@d_in, " +
                "DATE_DONE=@d_done, DATE_OUT=@d_out " +
            "WHERE ID=@id";
        public static bool EditRepair(RepairForm new_form, RepairForm old_form, FbConnection conn)
        {
            if (old_form.id == null)
                throw new ArgumentException("Id is null, cannot edit repair");
            if(new_form.Equals(old_form))
                return false;

            var cmd = new FbCommand(EditRepairQuery, conn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = old_form.id;

            cmd.Parameters.Add("@icon", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@c_id", SqlDbType.Int).Value = new_form.compl_id;
            cmd.Parameters.Add("@m_id", SqlDbType.Int).Value = new_form.mat_id;
            cmd.Parameters.Add("@c_num", SqlDbType.VarChar).Value = new_form.compl_num;
            cmd.Parameters.Add("@v_num", SqlDbType.VarChar).Value = new_form.vnutr_num;
            cmd.Parameters.Add("@i_num", SqlDbType.VarChar).Value = new_form.inv_num;
            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = new_form.name;
            cmd.Parameters.Add("@d_name", SqlDbType.VarChar).Value = new_form.Department;
            cmd.Parameters.Add("@u_name", SqlDbType.VarChar).Value = new_form.User;
            cmd.Parameters.Add("@p_name", SqlDbType.VarChar).Value = new_form.executor;
            cmd.Parameters.Add("@jal", SqlDbType.VarChar).Value = new_form.complaint;
            cmd.Parameters.Add("@diag", SqlDbType.VarChar).Value = new_form.diagnosis;
            cmd.Parameters.Add("@rep", SqlDbType.VarChar).Value = new_form.actions_taken;
            cmd.Parameters.Add("@d_in", SqlDbType.Timestamp).Value = new_form.acceptedTime;
            cmd.Parameters.Add("@d_done", SqlDbType.Timestamp).Value = new_form.doneTime;
            cmd.Parameters.Add("@d_out", SqlDbType.Timestamp).Value = new_form.returnedTime;

            var rows_affected = cmd.ExecuteNonQuery();
            return rows_affected > 0;
        }
    }
}
