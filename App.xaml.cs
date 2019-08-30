using Gallery.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Gallery.Utilits;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;
using Application = System.Windows.Application;

namespace Gallery
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    ///
    


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
        ApiVk lolkek = new ApiVk();
    }

    
}

