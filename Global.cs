using Avalonia.Animation;
using Inv.ViewModels.MainWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inv
{
    public static class Global
    {
        public const string RepairTab = "Ремонты";
        public const string JournalTab = "Журнал";

        public const int ComplectIcon = 1;

        // Глобальные не константы
        public static string Login { get; set; } = "";
        public static string Name { get; set; } = "";
        public static bool RW { get; set; } = false;
        public static bool RO { get; set; } = false;
        public static bool AU { get; set; } = false;

        public static TableRow? CurCompl { get; set; } = null;

        // Находимся ли на уровне отдельных компонентов
        public static bool TopLevel { get; set; } = true;
        public static bool BottomLevel { get; set; } = false;
    }
}
