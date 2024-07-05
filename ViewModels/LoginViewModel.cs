using Avalonia.Controls;
using MsgBox;
using System.Threading.Tasks;
using System;
using Inv.Models;
using System.Reactive;

namespace Inv.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        // Данные настройки будут сохранятся между сессиями
        public string DBLocation { get; set; }
        public string Login { get; set; }

        private UserModel _model;

        public LoginViewModel()
        {
            // Initialize properties from settings
            DBLocation = Properties.Login.Default.DBLocation;
            Login = Properties.Login.Default.Name;
            _model = new UserModel();
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

            _model.FetchUsers(_inst.GetConnection());
            return true;
        }

        public bool log_In()
        {
            var ty = _model.getTY(this.Login);

            if (ty == uint.MaxValue)
                return false;

            if (ty == 0) Global.RW = true; else Global.RW = false; // Write
            if (ty == 1) Global.RO = true; else Global.RO = false; // Read
            if (ty == 2) Global.AU = true; else Global.AU = false; // Author

            Global.Login = this.Login;

            return true;
        }
    }
}
