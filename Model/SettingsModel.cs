using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Model
{
    [Serializable]
    class SettingsModel
    {
        public List<USBModel> ListBoxSource { get; set; }
        public List<EmailGoodModel> EmailGoodModels { get; set; }
        public MailModel Mail { get; set; }
        public bool IsUSB { get; set; }
        public string Path { get; set; }
        public string FormEmailTitle { get; set; }
        public string ImgStyle { get; set; }
        public int SelectComboStyle { get; set; }
        public int SendMailErrorCount { get; set; }
        public bool IsVidEnable { get; set; }
        public int VidSec { get; set; }


        public SettingsModel()
        {
            ListBoxSource = new List<USBModel>();
            Mail = new MailModel();
            EmailGoodModels = new List<EmailGoodModel>();
        }
    }
}
