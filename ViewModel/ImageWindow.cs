using Gallery.Model;
using Gallery.Utilits;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gallery.ViewModel
{
    class ImageWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<ImageModel> imageList;
        private ImageModel imageModel;
        private Command close;
        private Command windowLoaded;
        private Command imgNext;
        private Command imgPrev;
        private Command showEmailControl;
        private double widthh;
        private double heightt;
        private Window windows;
        private bool isEnabelLeft;
        public bool isEnabelRight;
        private bool keyControlVisibly;

        private static bool _IsPaused = false;
        public static bool IsPaused
        {
            get => _IsPaused;
            set => _IsPaused = value;
        }

        private static string Play_Source = "../image/play.png";
        private static string Pause_Source = "../image/pause.png";
        public static string Play_Pause_Source
        {
            get => (IsPaused ? Play_Source : Pause_Source);
        } 


        public ImageWindow()
        {
            Widthh = SystemParameters.PrimaryScreenWidth;
            Heightt = SystemParameters.PrimaryScreenHeight;
            imageList = Explorer.ImageList;
            imageModel = Explorer.ImageModel;
        }

        public string ImageUrl
        {
            get
            {
                Explorer.ImgUrl = imageModel.ImageName;
                return imageModel.ImageName;
            }
        }

        public bool IsEnabelLeft
        {
            get
            {
                return isEnabelLeft;
            }
            set
            {
                if(isEnabelLeft != value)
                {
                    isEnabelLeft = value;
                    OnPropertyChanged("IsEnabelLeft");
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

        public bool IsEnabelRight
        {
            get
            {
                return isEnabelRight;
            }
            set
            {
                if (isEnabelRight != value)
                {
                    isEnabelRight = value;
                    OnPropertyChanged("IsEnabelRight");
                }
            }
        }

        public bool KeyControlVisibly
        {
            get
            {
                return keyControlVisibly;
            }
            set
            {
                if (keyControlVisibly != value)
                {
                    keyControlVisibly = value;
                    OnPropertyChanged("KeyControlVisibly");
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
                          UpVisibleImgNavi();
                      }
                  }));
            }
        }

        public Command Close
        {
            get
            {
                return close ??
                  (close = new Command(obj =>
                  {
                      windows.DialogResult = true;
                  }));
            }
        }

        public Command ImgNext
        {
            get
            {
                return imgNext ??
                  (imgNext = new Command(obj =>
                  {
                      for(int i = 0; i < imageList.Count(); i++)
                      {
                          if(imageList[i].ImageName == imageModel.ImageName)
                          {
                              
                              if(!(++i >= imageList.Count()))
                              {
                                  imageModel = imageList[i];
                                  OnPropertyChanged("ImageUrl");

                                  if(i+1 >= imageList.Count())
                                  {
                                      IsEnabelRight = false;
                                  }

                                  IsEnabelLeft = true;

                              }

                              break;
                          }
                      }
                      
                  }));
            }
        }

        public Command ImgPrev
        {
            get
            {
                return imgPrev ??
                    (imgPrev = new Command(obj =>
                    {
                        for (int i = 0; i < imageList.Count(); i++)
                        {
                            if (imageList[i].ImageName == imageModel.ImageName)
                            {
                                if(--i >= 0)
                                {
                                    imageModel = imageList[i];
                                    OnPropertyChanged("ImageUrl");

                                    if (!(--i >= 0))
                                    {
                                        IsEnabelLeft = false;
                                    }

                                    IsEnabelRight = true;
                                }

                                break;
                            }
                        }
                    }));
            }
        }

        public Command ShowEmailControl
        {
            get
            {
                return showEmailControl ??
                    (showEmailControl = new Command(obj =>
                    {
                        if(KeyControlVisibly  != true)
                        {
                            KeyControlVisibly = true;
                        }
                        else
                        {
                            KeyControlVisibly = false;
                        }
                    }));
            }
        }

        private void UpVisibleImgNavi()
        {
            if(imageList.Count() == 1)
            {
                IsEnabelRight = false;
                IsEnabelLeft = false;

                return;
            }

            for(int i = 0; i < imageList.Count(); i++)
            {
                if (imageList[i].ImageName == imageModel.ImageName)
                {
                    if (!(++i >= imageList.Count()))
                    {
                        IsEnabelRight = true;
                    }

                    if ((i-2 >= 0))
                    {
                        IsEnabelLeft = true;
                    }
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
