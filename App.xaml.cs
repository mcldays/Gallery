using Gallery.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Gallery.Utilits;
using Application = System.Windows.Application;

namespace Gallery
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    ///

   

    public partial class App : Application
    {
        public static App CurrentApp => App.Current as App;
        private PrintPage _kw;
        public PrintPage Kw
        {
            get => _kw;
            set { _kw = value; }

        }

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

        //ApiVk instance = new ApiVk();
    }

    
}

