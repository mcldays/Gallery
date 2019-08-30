using Gallery.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
/*
Класс отвечает за сравнения файлов, копирование  файлов     
*/
namespace Gallery.Utilits
{
    class FileManager
    {
        public delegate void DProgressMax(double max);
        public event DProgressMax ProgressMax = delegate { };

        public delegate void DProgressValue(double value);
        public event DProgressValue ProgressValue = delegate { };

        public delegate void DProgressStatus(string text);
        public event DProgressStatus ProgressStatus = delegate { };

        public delegate void DFileManagerStart();
        public event DFileManagerStart FileManagerStart = delegate { };

        public delegate void DEndCopy();
        public event DEndCopy EndCopy = delegate { };

        private string PatchUSB = string.Empty;
        private string PatchTo = string.Empty;
        private USBModel uSBModel = null;
        private List<USBModel> uSBModelsWork = new List<USBModel>();
        private bool IsFlashWork = false;

        public FileManager(USBModel Model, string PatchTo)
        {
            uSBModel = Model;
            PatchUSB = uSBModel.PathImage;
            this.PatchTo = PatchTo;
        }

        public FileManager(USBModel Model)
        {
            Initializer(Model);
        }

        public FileManager()
        {

        }

        private void Initializer(USBModel Model)
        {
            uSBModel = Model;
            PatchUSB = Model.VolumeLabel + uSBModel.PathImage;
            PatchTo = Explorer.FilePath;
        }

        public void Start()
        {
            StartMAN();
        }

        public void Start(USBModel Model)
        {
            uSBModelsWork.Add(Model);

            if (!IsFlashWork)
            {
                StartMAN();
                IsFlashWork = true;
            }
        }

        public void Dissconect(USBModel Model)
        {
            if (uSBModel != null && uSBModel.SerialNumber == Model.SerialNumber)
            {
                uSBModel = null;
            }

            uSBModelsWork.Remove(Model);
        }

        private async void StartMAN()
        {
            FileManagerStart();
            bool IsCopy = false;
            await Task.Run(() =>
            {
                for(int i = 0; i < uSBModelsWork.Count; ++i)
                {
                    Initializer(uSBModelsWork[i]);
                    SendStatusEvent(string.Format("Сканируется {0}({1})...", uSBModel.Name, uSBModel.VolumeLabel), 2000);
               
                    try
                    {
                        List<string> list = AnalysisFile(PatchUSB, PatchTo);
                        if (list.Count == 0)
                        {
                            SendStatusEvent(string.Format("На носителе {0}({1}) нет новых файлов!", uSBModel.Name, uSBModel.VolumeLabel), 3000);
                        }
                        else
                        {
                            FileCopy(list, PatchTo);
                            IsCopy = true;
                        }
                    }
                    catch(IOException e)
                    {
                        i = -1;
                    }
                }
                IsFlashWork = false;

                if (IsCopy)
                {
                    SendStatusEvent("Копирование завершено!", 3000);
                }
            });

            EndCopy();
        }

        public void SendStatusEvent(string Msg, int sleep = 0)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                ProgressStatus(Msg);
            }));

            if(sleep != 0)
            {
                //System.Threading.Thread.Sleep(sleep);
                Task.Delay(sleep).Wait();
            }
        }

        public void SendStatusProgEvent(int value)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                ProgressValue(value);
            }));
        }
        // Возвращяет пути файлов которе пренадлежат копированию
        public List<string> AnalysisFile(string Patch, string PatchTo)
        {
            List<string> list = new List<string>();

            if (!Directory.Exists(Patch))
            {
                SendStatusEvent("Путь для копирования не существует", 3000);
                return list;
            }

            if (!Directory.Exists(PatchTo))
            {
                SendStatusEvent("Путь в папку назначения не существует", 3000);
                return list;
            }

            // Получаем файлы с флешки
            List<string> PatchFile = GetFiles(Patch, "*.*");
            if (PatchFile.Count() == 0)
            {
                // Если флешка пустая
                return list;
            }

            //Получаем файлы с локальной папки
            List<string> PatchToFile = GetFiles(PatchTo, "*.*");

            if (PatchToFile.Count() == 0)
            {
                // Если локальная папка пустая возвращяем файлы флешки
                return PatchFile;
            }

            // Начинаем с файлов флешки
            for (int i = 0; i < PatchFile.Count(); i++)
            {
                // Проверяем есть ли файл в локальной папке?
                if(PatchToFile.Where(e => Path.GetFileName(e) == Path.GetFileName(PatchFile[i])).FirstOrDefault() == null)
                {
                    // Если нет, добавляем на копирование
                    list.Add(PatchFile[i]);
                }
            }
            
            return list;
        }

        private List<string> GetFiles(string path, string pattern)
        {
            var files = new List<string>();

            try
            {
                files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories).Where(str => str.IsAcceptable()).ToList();
                /*files.AddRange(Explorer.Filter(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly).ToList()));
                foreach (var directory in Directory.GetDirectories(path))
                    files.AddRange(Explorer.Filter(GetFiles(directory, pattern).ToList()));*/
            }
            catch (UnauthorizedAccessException) { }

            return files;
        }

        public void FileCopy(List<string> Patch, string PatchTo)
        {
            ProgressMax(Patch.Count-1);
            
            for (int i  = 0, index = 1; i < Patch.Count; i++, index++)
            {

                SendStatusProgEvent(i);
                string FileName = Path.GetFileName(Patch[i]);
                SendStatusEvent(string.Format("Копирую: {0} {1}/{2}", FileName, index, Patch.Count), 40);
                try
                {
                    CopyFile.Copy(Patch[i], Path.Combine(PatchTo, FileName));
                }
                catch(Exception e)
                {
                    // Тут нужно проверить подключён ли USB?
                    if(uSBModel != null)
                    {
                        DriveInfo disk = new DriveInfo(uSBModel.VolumeLabel);
                        if(!disk.IsReady)
                        {
                            SendStatusEvent(string.Format("Ошибка копирования файла {0} USB {1}({2}) накопитель вынут.", FileName, uSBModel.Name, uSBModel.VolumeLabel), 3500);
                            throw new IOException("USB накопитель вынут!");
                        }
                    }
                    continue;
                }
            }
        }
    }
}
