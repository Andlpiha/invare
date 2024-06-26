namespace Inv.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        // Данные настройки будут сохранятся между сессиями
        public string DBLocation { get; set; }
        public string Login { get; set; }

        public LoginViewModel()
        {
            // Initialize properties from settings
            DBLocation = Properties.Login.Default.DBLocation;
            Login = Properties.Login.Default.Name;
        }
    }
}
