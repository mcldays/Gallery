using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Gallery.Model;

namespace Gallery.View
{
    /// <summary>
    /// Логика взаимодействия для PhotoWindow.xaml
    /// </summary>
    public partial class PhotoWindow : Window
    {
        public int CountCopy { get; set; }
        public static readonly DependencyProperty NameImageProperty = DependencyProperty.Register(
            "NameImage", typeof(string), typeof(PhotoWindow), new PropertyMetadata(default(string)));

        public string NameImage
        {
            get { return (string) GetValue(NameImageProperty); }
            set { SetValue(NameImageProperty, value); }
        }



        public BitmapImage ScreenImage
        {
            get => (BitmapImage)GetValue(ScreenImageProperty);
            set => SetValue(ScreenImageProperty, value);
        }

        public static readonly DependencyProperty ScreenImageProperty = DependencyProperty.Register(
            "ScreenImage", typeof(BitmapImage), typeof(PhotoWindow), new PropertyMetadata(default(BitmapImage)));


        public PhotoWindow(string url, int countCopy)
        {
            
            InitializeComponent();

            NameImage = url;
            CountCopy = countCopy;
            
            if (File.Exists(url))
                ScreenImage = new BitmapImage(new Uri(url));


            


            
        }

        private void PrintPage(object o, PrintPageEventArgs e)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(Directory.GetCurrentDirectory() + "\\readyFile.png");
            System.Drawing.Point loc = new System.Drawing.Point(0, 0);
            e.Graphics.DrawImage(img, loc);
        }

        private void MakeScreenElement(FrameworkElement elem)
        {
            RenderTargetBitmap renderTargetBitmap =
                new RenderTargetBitmap((int)elem.Width, (int)elem.Height, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(elem);
            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (Stream fileStream = File.Create("readyFile.png"))
            {
                pngImage.Save(fileStream);
            }

        }


        private void PhotoWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < CountCopy; i++)
            {
                PrintDocument pd = new PrintDocument();
                //пробуй и true и false
                pd.OriginAtMargins = false;
                pd.PrintPage += PrintPage;
                PrintDialog printDialog = new PrintDialog();


                Border.Visibility = Visibility.Hidden;
                Border.VerticalAlignment = VerticalAlignment.Top;
                Border.HorizontalAlignment = HorizontalAlignment.Left;

                // Увеличить размер в 5 раз
                Border.Margin = new Thickness(0, 0, 0, 0);
                TransformGroup group = new TransformGroup();
                //group.Children.Add(new RotateTransform(270));
                group.Children.Add(new ScaleTransform(0.578, 0.578));

                Border.LayoutTransform = group;
                // Определить поля
                int pageMargin = 10;

                // Получить размер страницы
                System.Windows.Size pageSize = new System.Windows.Size(printDialog.PrintableAreaWidth,
                    printDialog.PrintableAreaHeight);

                // Инициировать установку размера элемента
                Border.Measure(pageSize);
                Border.Arrange(new Rect(pageMargin + 20, pageMargin, pageSize.Width, pageSize.Height));
                Border.Visibility = Visibility.Visible;
                Thread.Sleep(5000);
                MakeScreenElement(Border);

                pd.Print();
            }

            Close();
        }
    }
}
