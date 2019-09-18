using Gallery.Model;
using Gallery.Utilits;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Gallery.View;


namespace Gallery.ViewModel
{
    class EmailControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string emailText = string.Empty;
        private int countCopy = 1;

        private string sendStatus = string.Empty;
        private string sendStatus2 = string.Empty;
        private bool sendAnimation = true;
        private bool sendAnimation2 = true;


        private UserControl userControl;
        private Command emailSend;
        private Command print;
        private Command minusCount;
        private Command plusCount;

        private Command controlLoaded;



        

        public string EmailText
        {
            get
            {
                return emailText;
            }
            set
            {
                if (emailText != value)
                {
                    emailText = value;
                    OnPropertyChanged("EmailText");
                }
            }
        }


        public int CountCopy
        {
            get
            {
                return countCopy;
            }
            set
            {
                if (countCopy != value)
                {
                   countCopy = value;
                    OnPropertyChanged("CountCopy");
                }
            }
        }

        public bool colorStatusText;
        public bool colorStatusText2;

        public bool SendAnimation
        {
            get
            {
                return sendAnimation;
            }
            set
            {
                if(sendAnimation != value)
                {
                    sendAnimation = value;
                    OnPropertyChanged("SendAnimation");
                }
            }
        }

        public bool SendAnimation2
        {
            get
            {
                return sendAnimation2;
            }
            set
            {
                if (sendAnimation2 != value)
                {
                    sendAnimation2 = value;
                    OnPropertyChanged("SendAnimation2");
                }
            }
        }

        public string SendStatus
        {
            get
            {
                return sendStatus;
            }
            set
            {
                if(sendStatus != value)
                {
                    sendStatus = value;
                    OnPropertyChanged("SendStatus");
                }
            }
        }

        public string SendStatus2
        {
            get
            {
                return sendStatus2;
            }
            set
            {
                if (sendStatus2 != value)
                {
                    sendStatus2 = value;
                    OnPropertyChanged("SendStatus2");
                }
            }
        }

        public bool ColorStatusText
        {
            get
            {
                return colorStatusText;
            }
            set
            {
                if(colorStatusText != value)
                {
                    colorStatusText = value;
                    OnPropertyChanged("ColorStatusText");
                }
            }
        }

        public bool ColorStatusText2
        {
            get
            {
                return colorStatusText2;
            }
            set
            {
                if (colorStatusText2 != value)
                {
                    colorStatusText2 = value;
                    OnPropertyChanged("ColorStatusText2");
                }
            }
        }

        public string FormEmailTitle
        {
            get
            {
                return Explorer.FormEmailTitle;
            }
        }

        public Command ControlLoaded
        {
            get
            {
                return controlLoaded ??
                    (controlLoaded = new Command(async obj =>
                    {
                        if (obj != null && obj is UserControl)
                        {
                            userControl = (UserControl)obj;
                            userControl.IsVisibleChanged += UserControl_IsVisibleChanged;
                        }
                    }));
            }
        }

        public Command EmailSend
        {
            get
            {
                return emailSend ??
                    (emailSend = new Command(async obj =>
                    {
                        if(obj != null && obj is string)
                        {
                            string Url = Explorer.ImgUrl;
                            string Email = (string)obj;
                            

                            SendAnimation = false;
                            await Task.Delay(100);
                            EmailManager emai = new EmailManager();

                            if (!emai.IsValidEmail(Email))
                            {
                                ColorStatusText = false;
                                SendStatus = "Введен некорректный Email";
                                ResetSendStatus();
                                SendAnimation = true;
                                return;
                            }
                            string ReturnedMsg = await emai.SendEmail(Email, Url);
                            SendAnimation = true;

                            ColorStatusText = string.IsNullOrEmpty(ReturnedMsg);
                            if (ColorStatusText)
                            {
                                // Успешно отправил
                                SendStatus = "Сообщение успешно отправлено!";
                                ResetSendStatus();
                                Explorer.AddMailGood(Email);
                                EmailText = string.Empty;
                            }
                            else
                            {
                                // Ошибка отправки
                                SendStatus = ReturnedMsg; //"Ошибка при отправке сообщения!"
                                ResetSendStatus();
                                Explorer.AddMailBad();
                            }
                        }
                    }));
            }
        }

        public Command MinusCount
        {
            get
            {
                return minusCount ??
                       (minusCount = new Command(async obj =>
                       {
                           if (obj != null && obj is string && obj != "")
                           {
                               int tempCopyCount = Int32.Parse((string)obj);
                               if (tempCopyCount > 1)
                               {
                                   CountCopy--;
                               }
                           }
                       }));
            }
        }

        public Command PlusCount
        {
            get
            {
                return plusCount ??
                       (plusCount = new Command(async obj =>
                       {
                           if (obj != null && obj is string && obj != "")
                           {
                               int tempCopyCount = Int32.Parse((string)obj);
                               if (tempCopyCount < 10)
                               {
                                   CountCopy++;
                               }

                           }
                       }));
            }
        }


        public Command Print
        {
            get
            {
                return print ??
                       (print = new Command(async obj =>
                       {
                           if (obj != null && obj is string && obj != "")
                           {
                               string Url = Explorer.ImgUrl;

                               try
                               {
                                   int tempCopyCount = Int32.Parse((string)obj);
                                   SendAnimation2 = false;
                                   await Task.Delay(100);


                               

                                   App.CurrentApp.Kw = new PhotoWindow(Explorer.ImgUrl, tempCopyCount);

                                   App.CurrentApp.Kw.Show();
                                   //App.CurrentApp.Kw.Topmost = true;


                                   SendAnimation2 = true;
                                   SendStatus2 = "Распечатано!";
                                   ResetSendStatus();
                                   CountCopy = 1;
                               }
                               catch
                               {
                                   SendAnimation2 = true;
                                   SendStatus2 = "";
                                   ResetSendStatus();
                               }





                           }
                       }));
            }
        }

      

        private void PrintPage(object o, PrintPageEventArgs e)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(Directory.GetCurrentDirectory() + "\\readyFile.png");
            System.Drawing.Point loc = new System.Drawing.Point(0, 0);
            e.Graphics.DrawImage(img, loc);
        }

        private async void ResetSendStatus()
        {
            await Task.Delay(5000);
            SendStatus = string.Empty;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            EmailText = string.Empty;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        


    }
}
