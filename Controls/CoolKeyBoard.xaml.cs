using Gallery.Utilits;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using UserControl = System.Windows.Controls.UserControl;

namespace Gallery.Controls
{
    /// <summary>
    /// Логика взаимодействия для CoolKeyBoard.xaml
    /// </summary>
    public partial class CoolKeyBoard : UserControl
    {
        public CoolKeyBoard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ChoosedCultureProperty = DependencyProperty.Register(
            "ChoosedCulture", typeof(CultureInfo), typeof(CoolKeyBoard),
            new PropertyMetadata(CultureInfo.GetCultureInfo("en-US")));

        public CultureInfo ChoosedCulture
        {
            get => (CultureInfo) GetValue(ChoosedCultureProperty);
            set => SetValue(ChoosedCultureProperty, value);
        }

        public static readonly DependencyProperty CapsPressedProperty = DependencyProperty.Register(
            "CapsPressed", typeof(bool), typeof(CoolKeyBoard), new PropertyMetadata(default(bool)));

        public bool CapsPressed
        {
            get => (bool) GetValue(CapsPressedProperty);
            set => SetValue(CapsPressedProperty, value);
        }

        public static readonly DependencyProperty ShiftPressedProperty = DependencyProperty.Register(
            "ShiftPressed", typeof(bool), typeof(CoolKeyBoard), new PropertyMetadata(default(bool)));

        public bool ShiftPressed
        {
            get => (bool) GetValue(ShiftPressedProperty);
            set => SetValue(ShiftPressedProperty, value);
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            "IsOpen", typeof(bool), typeof(CoolKeyBoard), new PropertyMetadata(default(bool)));

        public bool IsOpen
        {
            get => (bool) GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        private ICommand _switchLanguageCommand;
        private ICommand _sendKeysCommand;
        private ICommand _deleteCommand;
        private ICommand _closeKeyBoardCommand;

        public ICommand CloseKeyBoardCommand =>
            _closeKeyBoardCommand ?? (_closeKeyBoardCommand = new Command(() => { IsOpen = false; }));

        public ICommand DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new Command(() => { Send(Keys.Back, false); }));

        public ICommand SwitchLanguageCommand => _switchLanguageCommand ?? (_switchLanguageCommand = new Command(() =>
        {
            try
            {
                ChoosedCulture = CultureInfo.GetCultureInfo("en-US");//ChoosedCulture == CultureInfo.GetCultureInfo("en-US")
                    //? CultureInfo.GetCultureInfo("en-US")
                    //: CultureInfo.GetCultureInfo("ru-Ru");
            }
            catch (Exception ee)
            {
                System.Windows.MessageBox.Show(ee.Message);
            }
        }));

        public ICommand SendKeysCommand => _sendKeysCommand ?? (_sendKeysCommand = new Command(a =>
        {
            try
            {
                if (a is string key)
                {
                    InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(ChoosedCulture);
                    switch (key)
                    {
                        case " ":
                            Send(Keys.Space, false);
                            return;
                        case "!":
                            Send(Keys.D1, true);
                            return;
                        case "?":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("ru-RU"));
                            Send(Keys.D7, true);
                            return;
                        case ".":
                            //InputLanguage.CurrentInputLanguage =
                            //    InputLanguage.FromCulture(CultureInfo.GetCultureInfo("en-US"));
                            Send(Keys.OemPeriod, false);
                            return;
                        case ",":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("en-US"));
                            Send(Keys.Oemcomma, false);
                            return;
                        case "Ввод":
                            Send(Keys.Enter, false);
                            return;
                        case "Enter":
                            Send(Keys.Enter, false);
                            return;
                        case "х":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("ru-RU"));
                            Send(Keys.OemOpenBrackets, false);
                            return;
                        case "ж":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("ru-RU"));
                            Send(Keys.OemSemicolon, false);
                            return;
                        case "э":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("ru-RU"));
                            Send(Keys.OemQuotes, false);
                            return;
                        case "б":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("ru-RU"));
                            Send(Keys.Oemcomma, false);
                            return;
                        case "ю":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("ru-RU"));
                            Send(Keys.OemPeriod, false);
                            return;
                        case "ё":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("ru-RU"));
                            Send(Keys.Oemtilde, false);
                            return;
                        case "[":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("en-US"));
                            Send(Keys.OemOpenBrackets, false);
                            return;
                        case "]":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("en-US"));
                            Send(Keys.OemCloseBrackets, false);
                            return;
                        case "ъ":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("ru-RU"));
                            Send(Keys.OemCloseBrackets, false);
                            return;
                        case "\\":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("ru-RU"));
                            Send((Keys) 220, false);
                            return;
                        case "-":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("ru-RU"));
                            Send((Keys) 189, false);
                            return;
                        case "_":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("ru-RU"));
                            Send((Keys) 189, true);
                            return;
                        case "Tab":
                            Send((Keys)9, false);
                            return;
                        case "CapsLock":
                            //Send((Keys)20, false);
                            return;
                        case "Shift":
                            ShiftPressed = true;
                            return;
                        case "@":
                            InputLanguage.CurrentInputLanguage =
                                InputLanguage.FromCulture(CultureInfo.GetCultureInfo("en-US"));
                            Send((Keys) 50, true);
                            return;
                        case "1":
                            Send((Keys) 49, false, true);
                            return;
                        case "2":
                            Send((Keys) 50, false, true);
                            return;
                        case "3":
                            Send((Keys) 51, false, true);
                            return;
                        case "4":
                            Send((Keys) 52, false, true);
                            return;
                        case "5":
                            Send((Keys) 53, false, true);
                            return;
                        case "6":
                            Send((Keys) 54, false, true);
                            return;
                        case "7":
                            Send((Keys) 55, false, true);
                            return;
                        case "8":
                            Send((Keys) 56, false, true);
                            return;
                        case "9":
                            Send((Keys) 57, false, true);
                            return;
                        case "0":
                            Send((Keys) 48, false, true);
                            return;
                        case "`":
                            Send(Keys.Oemtilde, false);
                            return;
                        case ";":
                            Send((Keys) 186, false);
                            return;
                        case "'":
                            Send((Keys) 222, false);
                            return;
                    }
                    var Key = Enum.Parse(typeof(Keys),
                        AllWord.Keys.Contains(key) ? AllWord[key].ToUpper() : key.ToUpper());

                    Send((Keys) Key, false);
                    InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(ChoosedCulture);
                }
            }
            catch
            {
                //
            }
        }));

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private static void PressKey(Keys key, bool up)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            if (up)
            {
                keybd_event((byte) key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr) 0);
            }
            else
            {
                keybd_event((byte) key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr) 0);
            }
        }

        void Send(Keys bukva, bool shift, bool cifra = false)
        { 
            if(bukva == Keys.OemPeriod)
            {
                PressKey(bukva, false);
                return;
            }

            if(bukva == (Keys)189)
            {
                PressKey(bukva, false);
                return;
            }

            if ((shift || ShiftPressed || CapsPressed) && !cifra )
                PressKey(Keys.LShiftKey, false);

            PressKey(bukva, false);
            PressKey(bukva, true);

            if ((shift || ShiftPressed || CapsPressed)&& !cifra)
                PressKey(Keys.LShiftKey, true);
            ShiftPressed = false;
        }

        private readonly Dictionary<string, string> AllWord = new Dictionary<string, string>()
        {
            {"й", "q"},
            {"ц", "w"},
            {"у", "e"},
            {"к", "r"},
            {"е", "t"},
            {"н", "y"},
            {"г", "u"},
            {"ш", "i"},
            {"щ", "o"},
            {"з", "p"},
            {"ф", "a"},
            {"ы", "s"},
            {"в", "d"},
            {"а", "f"},
            {"п", "g"},
            {"р", "h"},
            {"о", "j"},
            {"л", "k"},
            {"д", "l"},
            {"я", "z"},
            {"ч", "x"},
            {"с", "c"},
            {"м", "v"},
            {"и", "b"},
            {"т", "n"},
            {"ь", "m"},
            {"ю", "?"},
            {"!", "!"}
        };
    }
}
