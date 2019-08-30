using Gallery.Utilits;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Gallery.Model
{
    static class Explorer
    {
        private static string[] videoExtensions = { ".mp4", ".mpg", ".wmv", ".avi" };
        private static string[] imageExtensions = { ".png", ".jpg", ".jpeg", ".bmp" };

        private static ObservableCollection<ImageModel> imageList;

        private static ImageModel imageModel;

        private static string imgUrl = string.Empty;
        
        public static SettingsModel Setting { get; set; }

        public static List<USBModel> uSBModels
        {
            get
            {
                return Setting.ListBoxSource ?? new List<USBModel>();
            }
        }

        public static string FilePath
        {
            get
            {
                return Setting.Path;
            }
        }

        public static bool IsUSB
        {
            get
            {
                return Setting.IsUSB;
            }
        }

        public static int VidSec
        {
            get
            {
                return Setting.VidSec;
            }
        }

        public static bool IsVidEnable
        {
            get
            {
                return Setting.IsVidEnable;
            }
        }

        public static ObservableCollection<ImageModel> ImageList
        {
            get
            {
                return imageList;
            }
            set
            {
                if(imageList != value)
                {
                    imageList = value;
                }
            }
        }

        public static ImageModel ImageModel
        {
            get
            {
                return imageModel;
            }
            set
            {
                if(imageModel != value)
                {
                    imageModel = value;
                }
            }
        }

        public static MailModel MailModel
        {
            get
            {
                return Setting.Mail;
            }
        }

        public static string ImgUrl
        {
            get
            {
                return imgUrl;
            }
            set
            {
                imgUrl = value;
            }
        }

        public static string FormEmailTitle
        {
            get
            {
                return Setting.FormEmailTitle;
            }
        }

        public static string ImgStyle
        {
            get
            {
                return Setting.ImgStyle;
            }
            set
            {

            }
        }

        public static int SelectComboStyle
        {
            get
            {
                return Setting.SelectComboStyle;
            }
        }

        public static List<EmailGoodModel> emailGoodModels
        {
            get
            {
                return Setting.EmailGoodModels;
            }
        }

        public static int SendMailErrorCount
        {
            get
            {
                return Setting.SendMailErrorCount;
            }
        }

        public static int SendMailGoodCount
        {
            get
            {
                return Setting.EmailGoodModels.Count;
            }
        }

        public static void AddMailGood(string Email)
        {
            Setting.EmailGoodModels.Add(new EmailGoodModel { Email = Email, DateTime = DateTime.Now.ToString("g") });
            ObjectSerializator.SaveConfig(Setting);
        }

        public static void AddMailBad()
        {
            ++Setting.SendMailErrorCount;
            ObjectSerializator.SaveConfig(Setting);
        }

        public static void EmailClearStat()
        {
            Setting.EmailGoodModels.Clear();
            Setting.SendMailErrorCount = 0;
            ObjectSerializator.SaveConfig(Setting);
        }

        public static bool IsAcceptable(this string path)
        {
            string path1 = path.ToLower();
            return (!IsVidEnable ? false : videoExtensions.Contains(Path.GetExtension(path1))) || imageExtensions.Contains(Path.GetExtension(path1));
        }

        /*public static List<string> Filter(List<string> list)
        {
            return list.Where(str =>
                 !IsVidEnable ? true : str.EndsWith(".mp4")
              || !IsVidEnable ? true : str.EndsWith(".mpg")
              || !IsVidEnable ? true : str.EndsWith(".wmv")
              || !IsVidEnable ? true : str.EndsWith(".avi")

                                    || str.EndsWith(".png")
                                    || str.EndsWith(".jpg")
                                    || str.EndsWith(".jpeg")
                                    || str.EndsWith(".bmp")

                                    || str.EndsWith(".PNG")
                                    || str.EndsWith(".JPG")
                                    || str.EndsWith(".JPEG")
                                    || str.EndsWith(".BMP")).ToList();
        }*/
    }
}
