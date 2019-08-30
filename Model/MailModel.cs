using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Model
{
    [Serializable]
    class MailModel
    {
        public string Mail { get; set; }
        public string Password { get; set; }
        public string SMTPServer { get; set; }
        public int Port { get; set; }
        public string Title { get; set; }
    }
}
