﻿using Gallery.Model;
using Gallery.Utilits;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Gallery.View;


namespace Gallery.ViewModel
{
    class EmailControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string emailText = string.Empty;
        private string vkText = string.Empty;

        private string sendStatus = string.Empty;
        private string sendStatus2 = string.Empty;
        private bool sendAnimation = true;
        private bool sendAnimation2 = true;


        private UserControl userControl;
        private Command emailSend;
        private Command vkSend;

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


        public string VkText
        {
            get
            {
                return vkText;
            }
            set
            {
                if (vkText != value)
                {
                   vkText = value;
                    OnPropertyChanged("VkText");
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

        private ICommand _photoCommand;
        public ICommand PhotoCommand => _photoCommand ?? (_photoCommand = new Command((c =>
                                                {
                                                    App.CurrentApp.Kw = new Window1();

                                                    //NavigationService.Navigate(new Photo_Page());
                                                    App.CurrentApp.Kw.Show();
                                                    App.CurrentApp.Kw.Topmost = true;
                                                }
                                            )));



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
