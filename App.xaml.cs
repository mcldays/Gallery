using Gallery.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Gallery
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if(!StartSetting())
            {
                Application.Current.Shutdown();
            }

            base.OnStartup(e);
        }

        private bool StartSetting()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            bool? result = new SettingsWindow().ShowDialog();
            ShutdownMode = ShutdownMode.OnLastWindowClose;
            return result == null ? false : (result == true ? true : false);
        }
    }
}
