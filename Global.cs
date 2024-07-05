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

        // Глобальные не константы
        public static string Login { get; set; } = "";
        public static bool RW { get; set; } = false;
        public static bool RO { get; set; } = false;
        public static bool AU { get; set; } = false;

        // Находимся ли на уровне отдельных компонентов
        public static bool BottomLevel { get; set; } = false;
    }
}
