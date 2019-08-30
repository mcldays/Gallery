using Gallery.Model;
using Gallery.Utilits;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Windows.Data;


namespace Gallery.ViewModel
{
    class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public FileSystemWatcher Watcher;
        //private ObservableCollection<ImageModel> PImageList = null;
        private UsbManager usbManager = null;
        private FileManager fileManager = null;
        private Window windows = null;
        private bool statusGridVisibly = true;
        private double progressBarMax = 0;
        private double progressBarValue = 0;
        private string gridStatusText = "Загрузка...";
        private string fontImage = string.Empty;
        private double widthh;
        private double heightt;
        private double widthOfElement;
        private double heightOfElement;
        private Command windowLoaded;
        private Command showImage;
        private LoadImage loadImage;
        private List<string> backgrounds;

        public MainWindowModel()
        {
            ImageList = new ObservableCollection<ImageModel>();
            //Backgrounds = Directory.GetFiles(Directory.GetCurrentDirectory() + "//Backgrounds").ToList();
            Widthh = SystemParameters.PrimaryScreenWidth;
            Heightt = SystemParameters.PrimaryScreenHeight;
            WidthOfElement = Widthh / 5.05;
            HeightOfElement = Heightt / 4.8;
            loadImage = new LoadImage(ImageList, this);

            Watcher = new FileSystemWatcher();
            Watcher.Path = Explorer.FilePath;
            Watcher.NotifyFilter = NotifyFilters.Attributes |
                                    NotifyFilters.CreationTime |
                                    NotifyFilters.FileName |
                                    NotifyFilters.LastAccess |
                                    NotifyFilters.LastWrite |
                                    NotifyFilters.Size |
                                    NotifyFilters.Security;
            Watcher.Changed += UpdateList;
            Watcher.Deleted += UpdateList;
            Watcher.Renamed += UpdateList;
            Watcher.EnableRaisingEvents = true;
        }

        private void UpdateList(object source, FileSystemEventArgs e)
        {
            loadImage?.FileLoad_Tick(this,null);
        }

        private object _personCollectionLock = new object();
        public ObservableCollection<ImageModel> ImageList
        {
            get
            {
                return Explorer.ImageList;
            }
            set
            {
                Explorer.ImageList = value;
                //OnPropertyChanged("ImageList");
                BindingOperations.EnableCollectionSynchronization(Explorer.ImageList, _personCollectionLock);
            }
        }


        public List<string> Backgrounds
        {
            get
            {
                return backgrounds;
            }
            set
            {
                if (backgrounds != value)
                {
                    backgrounds = value;
                    OnPropertyChanged("Backgrounds");
                }
            }
        }

        public double WidthOfElement
        {
            get
            {
                return widthOfElement;
            }
            set
            {
                if (widthOfElement != value)
                {
                    widthOfElement = value;
                    OnPropertyChanged("WidthhOfElement");
                }
            }
        }

        public double HeightOfElement
        {
            get
            {
                return heightOfElement;
            }
            set
            {
                if (heightOfElement != value)
                {
                    heightOfElement = value;
                    OnPropertyChanged("HeightOfElement");
                }
            }
        }


        public double Widthh
        {
            get
            {
                return widthh;
            }
            set
            {
                if (widthh != value)
                {
                    widthh = value;
                    OnPropertyChanged("Widthh");
                }
            }
        }

        public double Heightt
        {
            get
            {
                return heightt;
            }
            set
            {
                if (heightt != value)
                {
                    heightt = value;
                    OnPropertyChanged("Heightt");
                }
            }
        }



        public bool StatusGridVisibly
        {
            get
            {
                return statusGridVisibly;
            }
            set
            {
                if(statusGridVisibly != value)
                {
                    statusGridVisibly = value;
                    OnPropertyChanged("StatusGridVisibly");
                }
            }
        }

        public double ProgressBarMax
        {
            get
            {
                return progressBarMax;
            }
            set
            {
                if(progressBarMax != value)
                {
                    progressBarMax = value;
                    OnPropertyChanged("ProgressBarMax");
                }
            }
        }

        public double ProgressBarValue
        {
            get
            {
                return progressBarValue;
            }
            set
            {
                if(progressBarValue != value)
                {
                    progressBarValue = value;
                    OnPropertyChanged("ProgressBarValue");
                }
            }
        }

        public string GridStatusText
        {
            get
            {
                return gridStatusText;
            }
            set
            {
                if(gridStatusText != value)
                {
                    gridStatusText = value;
                    OnPropertyChanged("GridStatusText");
                }
            }
        }

        public Command WindowLoaded
        {
            get
            {
                return windowLoaded ??
                  (windowLoaded = new Command(obj =>
                  {
                      if (obj != null && obj is Window)
                      {
                          windows = (Window)obj;
                          if(Explorer.IsUSB)
                          {
                              fileManager = new FileManager();
                              // Событие запуска файлового менеджера
                              fileManager.FileManagerStart += FileManager_FileManagerStart;
                              // Установка максимального значения для прогресс бара
                              fileManager.ProgressMax += FileManager_ProgressMax;
                              // Статусное сообщение
                              fileManager.ProgressStatus += FileManager_ProgressStatus;
                              // Тукущая позиция для прогресс бара
                              fileManager.ProgressValue += FileManager_ProgressValue;
                              // Завершение копирования
                              fileManager.EndCopy += FileManager_EndCopy;

                              usbManager = new UsbManager();
                              // Добавляем события подключения/отключения USB
                              usbManager.ConnectUSB += UsbManager_ConnectUSB;
                              usbManager.DisconnectUSB += UsbManager_DisconnectUSB;
                              // Регистрируем хук на события отключения/подключения USB
                              usbManager.USBMonitorStart(windows);

                              windows.Closing += delegate (object sender, CancelEventArgs e)
                              {
                                  // При закрытии окна останавливаем 
                                  usbManager.USBMonitorStop();
                              };

                              // Получаем список зарегестрированных устройств
                              List<USBModel> list = usbManager.GetActiveUSB();
                              for (int i = 0; i < list.Count; i++)
                              {
                                  UsbManager_ConnectUSB(list[i]);
                              }
                          }

                          SetFont(Explorer.SelectComboStyle);
                      }
                  }));
            }
        }

        public Command ShowImage
        {
            get
            {
                return showImage ??
                  (showImage = new Command(obj =>
                  {
                      if(obj != null && obj is ImageModel)
                      {
                          Explorer.ImageModel = (ImageModel)obj;
                          View.ImageWindow img = new View.ImageWindow();
                          img.ShowDialog();
                      }
                  }));
            }
        }

        public void FileManager_ProgressStatus(string text)
        {
            GridStatusText = text;
        }

        public void FileManager_ProgressMax(double max)
        {
            ProgressBarMax = max;
        }

        public void FileManager_FileManagerStart()
        {
            StatusGridVisibly = true;
        }

        public void FileManager_EndCopy()
        {
            StatusGridVisibly = false;
            ProgressBarMax = 0;
        }

        private void SetFont(int index)
        {
            if (index == 0)
            {
                FontImage = "../Image/white.png";
            }
            else if (index == 1)
            {
                FontImage = "../Image/Black.png";
            }
            else if (index == 2)
            {
                FontImage = "../Image/nebo.jpg";
            }
            else
            {
                FontImage = "../Image/wait.png";
            }
        }

        public void FileManager_ProgressValue(double value)
        {
            ProgressBarValue = value;
        }

        private void UsbManager_ConnectUSB(USBModel model)
        {
            fileManager.Start(model);
            //ImageList = new ObservableCollection<ImageModel>();
            //new LoadImage(ImageList);
        }

        private void UsbManager_DisconnectUSB(USBModel model)
        {
            // Отключино зарегестрированное устройство
            fileManager.Dissconect(model);
        }

        public string FontImage
        {
            get
            {
                return fontImage;
            }
            set
            {
                if(fontImage != value)
                {
                    fontImage = value;
                    OnPropertyChanged("FontImage");
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
