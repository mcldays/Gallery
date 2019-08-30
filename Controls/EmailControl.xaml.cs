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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gallery.Model;
using Gallery.ViewModel;

namespace Gallery.Controls
{
    /// <summary>
    /// Логика взаимодействия для EmailControl.xaml
    /// </summary>
    public partial class EmailControl : UserControl
    {
        private const int EmailSaveTimeMin = 15;

        public EmailControl()
        {
            InitializeComponent();
            CoolKeyBoard.Loaded += CoolKeyBoard_Loaded;
        }

        private void CoolKeyBoard_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Delay(100);
            this.textBlock.Focusable = true;
            Keyboard.Focus(this.textBlock);
        }

        private void textBlock_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModel.EmailControl emailControl)
            {
                EmailGoodModel emailGoodModel = Explorer.emailGoodModels?.FindLast(x => true);
                DateTime dateTime = DateTime.Now.AddMinutes(-EmailSaveTimeMin);
                DateTime dateTime1 = Convert.ToDateTime(emailGoodModel?.DateTime);
                emailControl.EmailText = dateTime1 > dateTime ? emailGoodModel?.Email : string.Empty;
            }
        }

        private void textBlock_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((sender as TextBox)?.IsVisible == true)
            {
                textBlock_Loaded(sender, null);
            }
        }
    }
}
