using Avalonia.Controls;
using MsgBox;
using System.Threading.Tasks;
using System;
using Inv.Models;

namespace Inv.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        // Данные настройки будут сохранятся между сессиями
        public string DBLocation { get; set; }
        public string Login { get; set; }

        private UserModel model;

        public LoginViewModel()
        {
            // Initialize properties from settings
            DBLocation = Properties.Login.Default.DBLocation;
            Login = Properties.Login.Default.Name;
            model = new UserModel();
        }

        public async Task<bool> dbConnect()
        {
            SQLConn _inst = SQLConn.Instance;

            if (_inst.GetConnection().State == System.Data.ConnectionState.Open)
                _inst.CloseConnection();

            if (!_inst.setDatabase(this.DBLocation))
            {
                await MessageBox.Show(null, "Неверно набран адрес БД", "Ошибка", MessageBox.MessageBoxButtons.Ok);
                return false;
            }
            else
            {
                try
                {
                    _inst.OpenConnection();
                }
                catch (Exception e)
                {
                    await MessageBox.Show(null, "БД недоступна", "Ошибка", MessageBox.MessageBoxButtons.Ok);
                    return false;
                }
            }

            model.FetchUsers(_inst.GetConnection());
            return true;
        }
    }
}
