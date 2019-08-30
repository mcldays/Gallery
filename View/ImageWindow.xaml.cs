using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Gallery.ViewModel;


namespace Gallery.View
{
    /// <summary>
    /// Логика взаимодействия для ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        private const int startSecond = 0;
        private bool isPausedBySlider = false;
        private bool isOpened = false;
        DispatcherTimer timer;

        public ImageWindow()
        {
            InitializeComponent();
            Play_Pause.Visibility = Visibility.Hidden;
            sliProgress.Visibility = Visibility.Hidden;
            isOpened = false;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (myMediaElement.Source != null && myMediaElement.NaturalDuration.HasTimeSpan && !isPausedBySlider && !ViewModel.ImageWindow.IsPaused)
            {
                sliProgress.Value = myMediaElement.Position.TotalSeconds;
            }
        }

        private void myMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (sender is MediaElement mediaElement)
            {
                mediaElement.LoadedBehavior = MediaState.Manual;
                isOpened = true;
                var dataContext = DataContext as ViewModel.ImageWindow;
                var isEmail = dataContext?.KeyControlVisibly ?? false;
                mediaElement.Play();
                ViewModel.ImageWindow.IsPaused = false;
                if (mediaElement.Source != null && mediaElement.HasVideo && mediaElement.NaturalDuration.HasTimeSpan) 
                {
                    Play_Pause.Visibility = Visibility.Visible;
                    sliProgress.Visibility = Visibility.Visible;
                    if (isEmail)
                    {
                        Play_Pause_Click(sender, e);
                    }
                    sliProgress.Minimum = startSecond;
                    double totalSeconds = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds / 1000;
                    sliProgress.Maximum = totalSeconds;
                }
                else
                {
                    Play_Pause.Visibility = Visibility.Hidden;
                    sliProgress.Visibility = Visibility.Hidden;
                }
            }
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliProgress.Value >= sliProgress.Maximum)
            {
                sliProgress.Value = startSecond;
            }
            myMediaElement.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            isPausedBySlider = true;
            myMediaElement.Pause();
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isPausedBySlider = false;
            if (!ViewModel.ImageWindow.IsPaused)
            {
                myMediaElement.Play();
            }
        }

        private void Play_Pause_Click(object sender, RoutedEventArgs e)
        {
            if (!isPausedBySlider && isOpened)
            {
                var IsPaused = ViewModel.ImageWindow.IsPaused = !ViewModel.ImageWindow.IsPaused;
                if (IsPaused)
                {
                    myMediaElement.Pause();
                }
                else
                {
                    myMediaElement.Play();
                }
                ControlTemplate ct = Play_Pause.Template;
                Image btnImage = (Image)ct.FindName("Img", Play_Pause);
                btnImage.Source = new BitmapImage(new Uri(Gallery.ViewModel.ImageWindow.Play_Pause_Source, UriKind.RelativeOrAbsolute));
            }
        }
    }
}
