using ControlzEx.Theming;
using flop.net.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace flop.net
{
   /// <summary>
   /// Логика взаимодействия для App.xaml
   /// </summary>
   public partial class App : Application
   {
        protected override void OnStartup(StartupEventArgs e)
        {
            // now set the Green color scheme and dark base color
            ThemeManager.Current.ChangeTheme(Application.Current, (string)Settings.Default["Theme"]);

            base.OnStartup(e);
        }
    }
}
