using Gallery.Model;
using Gallery.Utilits;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Globalization;

namespace Gallery.ViewModel
{
    class SettingsWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<USBModel> PComboUsbList = null;
        private ObservableCollection<USBModel> PListBoxSource = null;
        private Window windows = null;
        private UsbManager usbManager = null;
        private MailModel emailModel = new MailModel();

        private Command getPath;
        private Command addUSBToList;
        private Command listItemDelete;
        private Command windowLoaded;
        private Command getPathUSB;
        private Command checkMail;
        private Command startApplication;
        private Command comboSelectionChanged;
        private Command emailStatClear;
        private Command saveEmailToFile;
        private Command settingCansel;

        private SettingsModel setting = null;
        string PPath = string.Empty;
        bool PIsCheckUsb = false;
        string PPathUSB = string.Empty;
        string formEmailTitle = string.Empty;
        string styleImage = "../Image/wait.png";
        int selectComboStyle = 0;
        bool _IsVidEnable = true;
        int _VidSec = 2;

        public SettingsWindow()
        {
            LoadCfg();
        }

        public Command GetPath
        {
            get
            {
                return getPath ??
                  (getPath = new Command(obj =>
                  {
                      FolderBrowserDialog FBD = new FolderBrowserDialog();
                      if (FBD.ShowDialog() == DialogResult.OK)
                      {
                          Path = FBD.SelectedPath;
                          if(!usbManager.CheckNoUSBDisk(Path.Substring(0, 3)))
                          {
                              System.Windows.MessageBox.Show("Каталог для загрузки изображений не может быть съемным устройством!");
                              Path = string.Empty;
                          }
                      }
                  }));
            }
        }
       
        public Command AddUSBToList
        {
            get
            {
                return addUSBToList ??
                  (addUSBToList = new Command(obj =>
                  {
                     if(obj != null && obj is USBModel)
                     {
                         USBModel model = (USBModel)obj;
                          if(model.PathImage == null || model.PathImage == string.Empty)
                          {
                              model.PathImage = @"\";
                          }

                          USBModel elm = PListBoxSource.FirstOrDefault(w => w.SerialNumber == model.SerialNumber);
                          if (elm != null)
                          {
                              DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Флешка " + model.Name + " уже есть в списке! \n Заменить?","Добавление USB-устройства", MessageBoxButtons.YesNo);
                              if(dialogResult == DialogResult.Yes)
                              {
                                  PListBoxSource.Remove(elm);
                                  PListBoxSource.Add(model);
                              }
                          }
                          else
                          {
                              PListBoxSource.Add(model);
                              OnPropertyChanged("ListBoxSource");
                          }

                          PathUSB = string.Empty;
                      }
                  }));
            }
        }

        public Command ListItemDelete
        {
            get
            {
                return listItemDelete ??
                  (listItemDelete = new Command(obj =>
                  {
                      if (obj != null && obj is USBModel)
                      {
                          USBModel model = (USBModel)obj;
                          int index = PListBoxSource.IndexOf(model);
                          if (index != -1)
                          {
                              PListBoxSource.RemoveAt(index);
                              OnPropertyChanged("PListBoxSource");
                          }
                      }
                  }));
            }
        }

        public Command WindowLoaded => windowLoaded ??
        (windowLoaded = new Command(async obj =>
        {
            if (obj != null && obj is Window)
            {
                windows = (Window)obj;

                usbManager = new UsbManager();
                // Добавляем события подключения/отключения USB
                usbManager.SConnectUSB += UsbManager_ConnectUSB;
                usbManager.SDisconnectUSB += UsbManager_DisconnectUSB;
                // Регистрируем хук на события отключения/подключения USB
                usbManager.USBMonitorStart(windows);
                          
                windows.Closing += delegate (object sender, CancelEventArgs e)
                {
                    usbManager.USBMonitorStop();
                };

                await SetUsbList();
                SetStyleImg(SelectComboStyle);
            }
        }));

        public Command GetPathUSB
        {
            get
            {
                return getPathUSB ??
                  (getPathUSB = new Command(obj =>
                  {
                      if(obj != null && obj is USBModel)
                      {
                          FolderBrowserDialog FBD = new FolderBrowserDialog();
                          if (FBD.ShowDialog() == DialogResult.OK)
                          {
                              PathUSB = FBD.SelectedPath;
                              USBModel elm = (USBModel)obj;
                              if(elm.VolumeLabel == PathUSB.Substring(0, 3))
                              {
                                  elm.PathImage = PathUSB.Substring(2);
                              }
                              else
                              {
                                  System.Windows.MessageBox.Show(string.Format("Указанный путь {0} не соответствует USB устройству, укажите путь относительно пути USB устройства {1}", PathUSB, elm.VolumeLabel));
                                  PathUSB = string.Empty;
                              }
                          }
                      }
                  }));
            }
        }

        public Command CheckMail
        {
            get
            {
                return checkMail ??
                  (checkMail = new Command(async obj =>
                  {
                      string Message = string.Empty;
                      bool IsMail = await MailManager.CheckMail(emailModel);

                      if(IsMail)
                      {
                          Message = "ОК";
                      }
                      else
                      {
                          Message = "Не верный логин или пароль!";
                      }

                      System.Windows.MessageBox.Show(Message);
                  }));
            }
        }

        public Command SaveConfig
        {
            get
            {
                return checkMail ??
                  (checkMail = new Command(obj =>
                  {
                      SetSaveConfig();
                  }));
            }
        }
        
        public Command SettingCancel
        {
            get
            {
                return settingCansel ??
                  (settingCansel = new Command(obj =>
                  {
                      LoadCfg();
                  }));
            }
        }

        public Command StartApplication
        {
            get
            {
                return startApplication ??
                  (startApplication = new Command(obj =>
                  {
                      bool Return = false;
                      if (!CheckValue())
                      {
                          var result = System.Windows.MessageBox.Show("Сохранить изменения?", "Данные были изменены!", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                          switch (result)
                          {
                              case MessageBoxResult.Yes:
                                  SetSaveConfig();
                                  Return = true; break;
                              case MessageBoxResult.No:
                                  LoadCfg();
                                  Return = true; break;
                              default:
                                  Return = false; break;
                          }
                      }
                      else
                      {
                          Return = true;
                      }
                      if (Return)
                      {
                          windows.DialogResult = Return;
                      }
                  }));
            }
        }

        // проверка изменений
        private bool CheckValue()
        {
            if (
                emailModel != setting.Mail ||
                IsCheckUsb != setting.IsUSB ||
                Path != setting.Path ||
                FormEmailTitle != setting.FormEmailTitle ||
                SelectComboStyle != setting.SelectComboStyle ||
                IsVidEnable != setting.IsVidEnable ||
                VidSec != setting.VidSec
                )
            {
                return false;
            }
            else 
            {
                var _ListBoxSource = new ObservableCollection<USBModel>(setting.ListBoxSource);
                int count = ListBoxSource.Count;
                if (_ListBoxSource.Count != count)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (_ListBoxSource[i].SerialNumber != ListBoxSource[i].SerialNumber || _ListBoxSource[i].PathImage != ListBoxSource[i].PathImage)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public Command ComboSelectionChanged
        {
            get
            {
                return comboSelectionChanged ??
                  (comboSelectionChanged = new Command(obj =>
                  {
                      if(obj != null && obj is int)
                      {
                          int index = (int)obj;

                          SetStyleImg(index);
                      }
                  }));
            }
        }
        
        public Command EmailStatClear
        {
            get
            {
                return emailStatClear ??
                  (emailStatClear = new Command(obj =>
                  {
                      DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Сбросить всю статистику Email отправок?", "Сброс статистики Email", MessageBoxButtons.YesNo);
                      if (dialogResult == DialogResult.Yes)
                      {
                          Explorer.EmailClearStat();
                          OnPropertyChanged("EmailGood");
                          OnPropertyChanged("SendMailErrorCount");
                          OnPropertyChanged("SendMailGoodCount");
                      }
                  }));
            }
        }

        public Command SaveEmailToFile
        {
            get
            {
                return saveEmailToFile ??
                  (saveEmailToFile = new Command(obj =>
                  {
                      if(Explorer.SendMailGoodCount == 0)
                      {
                          System.Windows.MessageBox.Show("Нет данных для выгрузки!");
                          return;
                      }

                      Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                      dlg.FileName = "Список Email";
                      dlg.Filter = "Text documents (.txt)|*.txt"; 

                      // Show save file dialog box
                      Nullable<bool> result = dlg.ShowDialog();

                      if (result == true)
                      {
                          // Save document
                          string filename = dlg.FileName;

                          using (StreamWriter sw = new StreamWriter(new FileStream(filename, FileMode.Append, FileAccess.Write)))
                          {
                              for (int i = 0; i < Explorer.emailGoodModels.Count; i++)
                              {
                                  sw.WriteLine(Explorer.emailGoodModels[i].Email);
                              }
                          }

                          System.Windows.MessageBox.Show("Список Email успешно создан!", "ОК", MessageBoxButton.OK);
                      }
                  }));
            }
        }

        private void SetStyleImg(int index)
        {
            if (index == 0)
            {
                StyleImage = "../Image/white.png";
            }
            else if (index == 1)
            {
                StyleImage = "../Image/Black.png";
            }
            else if (index == 2)
            {
                StyleImage = "../Image/nebo.jpg";
            }
            else
            {
                StyleImage = "../Image/wait.png";
            }
        }

        public string StyleImage
        {
            get
            {
                return styleImage;
            }
            set
            {
                if(styleImage != value)
                {
                    styleImage = value;
                    OnPropertyChanged("StyleImage");
                }
            }
        }

        private async void UsbManager_DisconnectUSB()
        {
            await SetUsbList();
        }

        private async void UsbManager_ConnectUSB()
        {
            await SetUsbList();
        }

        public string Path
        {
            get
            {
                return PPath;
            }

            set
            {
                if(PPath != value)
                {
                    PPath = value;
                    OnPropertyChanged("Path");
                }
            }
        }
        
        public int VidSec
        {
            get => _VidSec;
            set
            {
                if (_VidSec != value)
                {
                    _VidSec = value;
                    OnPropertyChanged("VidSecStr");
                }
            }

        }

        public string VidSecStr
        {
            get => _VidSec.ToString();

            set
            {
                int val = 0;
                try
                {
                    double value1 = Convert.ToDouble(value.Replace(",", "."), CultureInfo.InvariantCulture);
                    val = Math.Abs(Convert.ToInt32(value1));
                }
                catch (Exception)
                {
                    val = _VidSec;
                }
                finally
                {
                    if (_VidSec != val)
                    {
                        _VidSec = val;
                        OnPropertyChanged("VidSecStr");
                    }
                }
            }
        }


        public bool IsCheckUsb
        {
            get
            {
                return PIsCheckUsb;
            }

            set
            {
                if (PIsCheckUsb != value)
                {
                    PIsCheckUsb = value;
                    OnPropertyChanged("IsCheckUsb");
                    SetUsbList();
                }
            }
        }

        public bool IsVidEnable
        {
            get => _IsVidEnable;

            set
            {
                if (_IsVidEnable != value)
                {
                    _IsVidEnable = value;
                    OnPropertyChanged("IsVidEnable");
                }
            }
        }

        public string PathUSB
        {
            get
            {
                return PPathUSB;
            }

            set
            {
                if (PPathUSB != value)
                {
                    PPathUSB = value;
                    OnPropertyChanged("PathUSB");
                }
            }
        }

        public string Mail
        {
            get
            {
                return emailModel.Mail ?? string.Empty;
            }
            set
            {
                if(emailModel.Mail  != value)
                {
                    emailModel.Mail = value;
                    OnPropertyChanged("Email");
                }
            }
        }

        public string MailPassword
        {
            get
            {
                return emailModel.Password ?? string.Empty;
            }
            set
            {
                if(emailModel.Password != value)
                {
                    emailModel.Password = value;
                    OnPropertyChanged("MailPassword");
                }
            }
        }

        public string SMTPServer
        {
            get
            {
                return emailModel.SMTPServer ?? string.Empty;
            }
            set
            {
                if(emailModel.SMTPServer != value)
                {
                    emailModel.SMTPServer = value;
                    OnPropertyChanged("SMTPServer");
                }
            }
        }

        public int SMTPPort
        {
            get
            {
                return emailModel.Port;
            }
            set
            {
                if(emailModel.Port != value)
                {
                    emailModel.Port = value;
                    OnPropertyChanged("SMTPPort");
                }
            }
        }

        public string MailTitle
        {
            get
            {
                return emailModel.Title ?? string.Empty;
            }
            set
            {
                if(emailModel.Title != value)
                {
                    emailModel.Title = value;
                    OnPropertyChanged("MailTitle");
                }
            }
        }

        public int SelectComboStyle
        {
            get
            {
                return selectComboStyle;
            }
            set
            {
                if(selectComboStyle != value)
                {
                    selectComboStyle = value;
                    OnPropertyChanged("SelectComboStyle");
                }
            }
        }

        public string FormEmailTitle
        {
            get
            {
                return formEmailTitle;
            }
            set
            {
                if(formEmailTitle != value)
                {
                    formEmailTitle = value;
                    OnPropertyChanged("FormEmailTitle");
                }
            }
        }

        public ObservableCollection<EmailGoodModel> EmailGood
        {
            get
            {
                return new ObservableCollection<EmailGoodModel>(Explorer.emailGoodModels);
            }
        }

        public string SendMailErrorCount
        {
            get
            {
                return string.Format("Ошибки отправки: {0}", Explorer.SendMailErrorCount);
            }
        }

        public string SendMailGoodCount
        {
            get
            {
                return string.Format("Отправлено сообщений: {0}", Explorer.SendMailGoodCount);
            }
        }

        public ObservableCollection<USBModel> ComboUsbList
        {
            get
            {
                return PComboUsbList;
            }
        }

        public ObservableCollection<USBModel> ListBoxSource
        {
            get
            {
                return PListBoxSource;
            }
        }

        private async Task SetUsbList()
        {
            List<USBModel> devices = usbManager.GetUsb();
            PComboUsbList = new ObservableCollection<USBModel>(devices);
            OnPropertyChanged("ComboUsbList");
        }

        private void LoadCfg()
        {
            setting = ObjectSerializator.LoadConfig();
            if(setting == null)
            {
                setting = new SettingsModel();
            }

            PListBoxSource = new ObservableCollection<USBModel>(setting.ListBoxSource);
            emailModel = setting.Mail;
            IsCheckUsb = setting.IsUSB;
            Path = setting.Path;
            FormEmailTitle = setting.FormEmailTitle;
            SelectComboStyle = setting.SelectComboStyle;
            IsVidEnable = setting.IsVidEnable;
            VidSec = setting.VidSec;
            Explorer.Setting = setting;

            SetStyleImg(SelectComboStyle);
            OnPropertyChanged("Mail");
            OnPropertyChanged("MailPassword");
            OnPropertyChanged("SMTPServer");
            OnPropertyChanged("SMTPPort");
            OnPropertyChanged("MailTitle");
        }

        private void SetSaveConfig()
        {
            setting = new SettingsModel();
            foreach (var item in ListBoxSource)
                setting.ListBoxSource.Add(item);

            setting.Mail = emailModel;
            setting.IsUSB = IsCheckUsb;
            setting.Path = Path;
            setting.FormEmailTitle = FormEmailTitle;
            setting.SelectComboStyle = SelectComboStyle;
            setting.IsVidEnable = IsVidEnable;
            setting.VidSec = VidSec;
            ObjectSerializator.SaveConfig(setting);
            LoadCfg();
        }
        
        public void StopUSBMonitor()
        {
            usbManager.USBMonitorStop();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
