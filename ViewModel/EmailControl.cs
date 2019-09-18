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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Gallery.View;
using Rect = System.Windows.Rect;


namespace Gallery.ViewModel
{
    class EmailControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string emailText = string.Empty;
        private string emailText1 = string.Empty;
        private string emailText2 = string.Empty;
        private int countCopy = 1;

        private string sendStatus = string.Empty;
        private string sendStatus3 = string.Empty;
        private string sendStatus4 = string.Empty;
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

        public string EmailText1
        {
            get
            {
                return emailText1;
            }
            set
            {
                if (emailText1 != value)
                {
                    emailText1 = value;
                    OnPropertyChanged("EmailText1");
                }
            }
        }

        public string EmailText2
        {
            get
            {
                return emailText2;
            }
            set
            {
                if (emailText2 != value)
                {
                    emailText2 = value;
                    OnPropertyChanged("EmailText2");
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
        public bool colorStatusText3;
        public bool colorStatusText4;

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


        public string SendStatus3
        {
            get
            {
                return sendStatus3;
            }
            set
            {
                if (sendStatus3 != value)
                {
                    sendStatus3 = value;
                    OnPropertyChanged("SendStatus3");
                }
            }
        }


        public string SendStatus4
        {
            get
            {
                return sendStatus4;
            }
            set
            {
                if (sendStatus4 != value)
                {
                    sendStatus4 = value;
                    OnPropertyChanged("SendStatus4");
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

        public bool ColorStatusText3
        {
            get
            {
                return colorStatusText3;
            }
            set
            {
                if (colorStatusText3 != value)
                {
                    colorStatusText3 = value;
                    OnPropertyChanged("ColorStatusText3");
                }
            }
        }

        public bool ColorStatusText4
        {
            get
            {
                return colorStatusText4;
            }
            set
            {
                if (colorStatusText4 != value)
                {
                    colorStatusText4 = value;
                    OnPropertyChanged("ColorStatusText4");
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

                            EmailManager emai = new EmailManager();

                            if (Email != "")
                            {
                                SendAnimation = false;
                                await Task.Delay(100);
                                

                                if (!emai.IsValidEmail(Email))
                                {
                                    ColorStatusText = false;
                                    SendStatus = "Введен некорректный Email";
                                    ResetSendStatus();
                                    SendAnimation = true;

                                }
                                else
                                {
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


                                } // конец 1-го блока
                            }

                            if (EmailText1 != "")
                            {
                                if (!emai.IsValidEmail(EmailText1))
                                {
                                    ColorStatusText3 = false;
                                    SendStatus3 = "Введен некорректный Email";
                                    ResetSendStatus();
                                    SendAnimation = true;

                                }
                                else
                                {
                                    string ReturnedMsg = await emai.SendEmail(EmailText1, Url);
                                    SendAnimation = true;

                                    ColorStatusText3 = string.IsNullOrEmpty(ReturnedMsg);
                                    if (ColorStatusText3)
                                    {
                                        // Успешно отправил
                                        SendStatus3 = "Сообщение успешно отправлено!";
                                        ResetSendStatus();
                                        Explorer.AddMailGood(EmailText1);
                                        EmailText = string.Empty;
                                    }
                                    else
                                    {
                                        // Ошибка отправки
                                        SendStatus3 = ReturnedMsg; //"Ошибка при отправке сообщения!"
                                        ResetSendStatus();
                                        Explorer.AddMailBad();
                                    }


                                }
                            }

                            if (EmailText2 != "")
                            {
                                if(!emai.IsValidEmail(EmailText2))
                                {
                                    ColorStatusText4 = false;
                                    SendStatus4 = "Введен некорректный Email";
                                    ResetSendStatus();
                                    SendAnimation = true;

                                }

                                else
                                {
                                    string ReturnedMsg = await emai.SendEmail(EmailText2, Url);
                                    SendAnimation = true;

                                    ColorStatusText4 = string.IsNullOrEmpty(ReturnedMsg);
                                    if (ColorStatusText4)
                                    {
                                        // Успешно отправил
                                        SendStatus4 = "Сообщение успешно отправлено!";
                                        ResetSendStatus();
                                        Explorer.AddMailGood(EmailText2);
                                        EmailText = string.Empty;
                                    }
                                    else
                                    {
                                        // Ошибка отправки
                                        SendStatus4 = ReturnedMsg; //"Ошибка при отправке сообщения!"
                                        ResetSendStatus();
                                        Explorer.AddMailBad();
                                    }


                                }
                            }

                            SendAnimation = true;


                            //string ReturnedMsg = await emai.SendEmail(Email, Url);
                            //string ReturnedMsg1 = await emai.SendEmail(EmailText1, Url);
                            //string ReturnedMsg2 = await emai.SendEmail(EmailText2, Url);




                            //ColorStatusText = string.IsNullOrEmpty(ReturnedMsg);
                            //if (ColorStatusText)
                            //{
                            //    // Успешно отправил
                            //    SendStatus = "Сообщение успешно отправлено!";
                            //    ResetSendStatus();
                            //    Explorer.AddMailGood(Email);
                            //    EmailText = string.Empty;
                            //}
                            //else
                            //{
                            //    // Ошибка отправки
                            //    SendStatus = ReturnedMsg; //"Ошибка при отправке сообщения!"
                            //    ResetSendStatus();
                            //    Explorer.AddMailBad();
                            //}
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
                               try
                               { 
                                   SendAnimation2 = false;
                                   await Task.Delay(100);

                                   for (int i = 0; i < CountCopy; i++)
                                   {
                                       PrintDocument pd = new PrintDocument();
                                       pd.OriginAtMargins = false;
                                       pd.PrintPage += PrintPage;

                                       pd.Print();
                                   }


                                   ColorStatusText = true;
                                   SendAnimation2 = true;
                                   SendStatus2 = "Отправлено на печать!";
                                   ResetSendStatus();
                                   CountCopy = 1;
                               }
                               catch
                               {
                                   ColorStatusText = false;
                                   SendAnimation2 = true;
                                   SendStatus2 = "Ошибка при печати";
                               }

                           }
                       }));
            }
        }


        private void PrintPage(object o, PrintPageEventArgs e)
        {
            string Url = Explorer.ImgUrl;
            System.Drawing.Image img = System.Drawing.Image.FromFile(Url);
            var a = e.MarginBounds;
            Margins b = e.PageSettings.Margins;
            System.Drawing.Rectangle c = new System.Drawing.Rectangle(0, 0, a.Width + b.Left + b.Right, a.Height + b.Top + b.Bottom);
            e.Graphics.DrawImage(img, c);
        }


        private async void ResetSendStatus()
        {
            await Task.Delay(5000);
            SendStatus = string.Empty;
            SendStatus2 = string.Empty;
            SendStatus3 = string.Empty;
            SendStatus4 = string.Empty;

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
