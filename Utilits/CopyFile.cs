using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Utilits
{
    public static class CopyFile
    {
        public static void Copy(string Path, string ToPath)
        {
            byte[] buff = Read(Path);
            Write(buff, ToPath);
        }

        private static byte[] Read(string Path)
        {
            using (FileStream fstream = File.OpenRead(Path))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                return array;
            }
        }

        private static void Write(byte[] buff, string Path)
        {
            using (FileStream fstream = new FileStream(Path, FileMode.OpenOrCreate))
            {
                fstream.Write(buff, 0, buff.Length);
            }
        }
    }
}
