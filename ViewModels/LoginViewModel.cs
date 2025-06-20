﻿using MsgBox;
using System.Threading.Tasks;
using System;
using Inv.Models;
using ReactiveUI;

namespace Inv.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private bool _isDatabaseSectionExpanded = false;
        public bool IsDatabaseSectionExpanded
        {
            get => _isDatabaseSectionExpanded;
            set => this.RaiseAndSetIfChanged(ref _isDatabaseSectionExpanded, value);
        }

        // Данные настройки будут сохранятся между сессиями
        public string  ServerLocation { get; set; }
        public string DatabaseFile { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        private UserModel _model;

        public LoginViewModel()
        {
            // Initialize properties from settings
            ServerLocation = Properties.Login.Default.ServerLocation;
            DatabaseFile = Properties.Login.Default.DatabaseFile;
            Login = Properties.Login.Default.UserLogin;
            Password = "";

            _model = new UserModel();
        }

        public async Task<bool> dbConnect()
        {
            SQLConn _inst = SQLConn.Instance;

            if (this.ServerLocation == "" || this.DatabaseFile == "")
                return false;

            if (_inst.GetConnection().State == System.Data.ConnectionState.Open)
                _inst.CloseConnection();

            if (!_inst.setDatabase(this.ServerLocation, this.DatabaseFile))
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

            return true;
        }

        public bool log_In()
        {
            SQLConn _inst = SQLConn.Instance;
            var result = _model.doAuth(Login, Password, _inst.GetConnection());

            if(!result)
                return false;

            _inst.setUser(_model.dbLogin!, _model.dbPassword!);
            try
            {
                _inst.OpenConnection();
            }
            catch (Exception e)
            {
                MessageBox.Show(null, "БД недоступна", "Ошибка", MessageBox.MessageBoxButtons.Ok);
                return false;
            }

            var ty = _model.userTy;

            if (ty == 1) Global.RW = true; else Global.RW = false; // ReadWrite
            if (ty == 2) Global.RO = true; else Global.RO = false; // ReadOnly
            if (ty == 3) Global.AU = true; else Global.AU = false; // Author

            Global.Login = this.Login;
            Global.Name = _model.userName!;

            return true;
        }
    }
}
