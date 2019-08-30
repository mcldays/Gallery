using Gallery.Model;
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
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;

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


        public Command VkSend
        {
            get
            {
                return vkSend ??
                       (vkSend = new Command(async obj =>
                       {
                           if (obj != null && obj is string && obj != "")
                           {
                               string Url = Explorer.ImgUrl;
                               string vkId = (string)obj;
                               SendAnimation2 = false;
                               await Task.Delay(100);

                               var api = Utilits.ApiVk.api;
                               var uploadServer = api.Photo.GetMessagesUploadServer(1); //Получаем ссылку на сервер для загрузок
                               var uploadServerUri = uploadServer.UploadUrl;
                               var uploader = new WebClient();
                               var uploadResponseInBytes = uploader.UploadFile(uploadServerUri, Url); // Загружаем фото на сервер
                               var uploadResponseInString = Encoding.UTF8.GetString(uploadResponseInBytes);

                               var photo = api.Photo.SaveMessagesPhoto(uploadResponseInString);

                               Random random = new Random();
                               int randomNumber = random.Next(1, 99999999);

                               try
                               {
                                   if (Regex.IsMatch(vkId, @"^\d+$"))
                                   {
                                       var check = api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                       {
                                           RandomId = randomNumber, // уникальный
                                           UserId = Int32.Parse(vkId),
                                       });

                                   }
                                   else
                                   {
                                       var check = api.Messages.Send(new VkNet.Model.RequestParams.MessagesSendParams
                                       {
                                           RandomId = randomNumber, // уникальный
                                           Domain = vkId,
                                           Attachments = photo
                                       });
                                   }

                                   SendAnimation2 = true;
                                   ColorStatusText2 = true;
                                   SendStatus2 = "Сообщение успешно отправлено!";
                                   ResetSendStatus();
                                   VkText = string.Empty;
                               }
                               catch
                               {
                                   SendAnimation2 = true;
                                   ColorStatusText2 = false;
                                   SendStatus2 = "Пользователь не найден или он ограничил круг лиц, которые могут отправлять ему сообщения!";
                                   ResetSendStatus();
                               }

                               





                               //EmailManager emai = new EmailManager();

                               //if (!emai.IsValidEmail(Email))
                               //{
                               //    ColorStatusText = false;
                               //    SendStatus = "Введен некорректный Email";
                               //    ResetSendStatus();
                               //    SendAnimation = true;
                               //    return;
                               //}
                               //string ReturnedMsg = await emai.SendEmail(Email, Url);
                               //SendAnimation = true;

                               //ColorStatusText = string.IsNullOrEmpty(ReturnedMsg);
                               //if (ColorStatusText)
                               //{
                               //    // Успешно отправил
                               //    SendStatus = "Сообщение успешно отправлено!";
                               //    ResetSendStatus();
                               //    Explorer.AddMailGood(Email);
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
