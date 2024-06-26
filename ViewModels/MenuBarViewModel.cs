using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inv.ViewModels
{
    internal class MenuBarViewModel :ViewModelBase
    {
        // Toolbar Actions
        public bool ExitApp()
        {
            System.Environment.Exit(0);
            return true;
        }
    }
}
