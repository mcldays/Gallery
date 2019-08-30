using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Model
{
    [Serializable]
    class USBModel
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string VolumeLabel { get; set; }
        public string SerialNumber { get; set; }
        public string PathImage { get; set; }
    }
}
