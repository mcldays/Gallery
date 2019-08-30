using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Gallery.Model;
using Gallery.ViewModel;
using System.Diagnostics;
using Gallery.View;
using Gallery.Utilits;

namespace Gallery
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            //Скрытый выход по таймеру
            Start(KillExplorer);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, ExitSeconds);
        }

       

       


        public static readonly DependencyProperty HeighttProperty = DependencyProperty.Register(
            "Heightt", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

        public double Heightt
        {
            get { return (double) GetValue(HeighttProperty); }
            set { SetValue(HeighttProperty, value); }
        }

        public static readonly DependencyProperty WidthhProperty = DependencyProperty.Register(
            "Widthh", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

        public double Widthh
        {
            get { return (double) GetValue(WidthhProperty); }
            set { SetValue(WidthhProperty, value); }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (sender is MediaElement mediaElement)
            {
                mediaElement.LoadedBehavior = MediaState.Manual;
                if (mediaElement.Source != null && mediaElement.HasVideo)// && mediaElement.NaturalDuration.HasTimeSpan
                {
                    //var max = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                    var VidSec = Explorer.VidSec;
                    TimeSpan timeSpan = TimeSpan.FromSeconds(VidSec);//VidSec > max ? max : VidSec
                    mediaElement.Position = timeSpan;
                    mediaElement.Pause();
                }

                if (!ScrollControl.IsEnabled)
                {
                    ScrollControl.IsEnabled = true;
                }

                if (DataContext is MainWindowModel dataContext)
                {
                    /*if (!dataContext.StatusGridVisibly)
                    {
                        dataContext.FileManager_FileManagerStart();
                    }
                    dataContext.ProgressBarValue++;*/
                    //if (dataContext.ProgressBarValue >= dataContext.ProgressBarMax)
                    if (dataContext.StatusGridVisibly)
                    {
                        dataContext.FileManager_EndCopy();
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainWindowModel dataContext)
            {
                dataContext.Watcher.Dispose();
            }
            explorerThread?.Abort();
            Start(RunExplorer);
        }

        #region Скрытый выход по таймеру (+ блок инициализации и приввязка обработчика на закрытие окна)
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private const int ExitSeconds = 3;
        static Thread explorerThread;

        //private void XMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    explorerThread?.Abort();
        //    Start(RunExplorer);
        //}

        static void RunExplorer()
        {
            Process[] processInfo = Process.GetProcessesByName("explorer");
            if (processInfo?.Length == 0)
            {
                //Process.Start("explorer.exe");
                Process proc = new Process();
                proc.StartInfo.FileName = "C:\\Windows\\explorer.exe";
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
        }

        static void KillExplorer()
        {
            explorerThread = Thread.CurrentThread;
            do
            {
                Process[] processInfo = Process.GetProcessesByName("explorer");
                if (processInfo?.Length > 0)
                {
                    try
                    {
                        foreach (Process p in processInfo)
                            p.Kill();
                    }
                    catch (Exception) { }
                }
            }
            while (true);
        }

        private void Start(ThreadStart process)
        {
            Thread thr = new Thread(new ThreadStart(process));
            thr.Priority = ThreadPriority.AboveNormal;
            thr.IsBackground = false;
            thr.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            Window.Close();
        }

        private void Button_TouchDown(object sender, TouchEventArgs e)
        {
            dispatcherTimer.Start();
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dispatcherTimer.Start();
        }


        private void Button_TouchUp(object sender, TouchEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void Button_TouchLeave(object sender, TouchEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            dispatcherTimer.Stop();
        }
        #endregion

    }
}
