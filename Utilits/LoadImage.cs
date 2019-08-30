using Gallery.Model;
using Gallery.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Gallery.Utilits
{
    class LoadImage
    {
        string path = string.Empty;
        MainWindowModel ParentWindow;
        //DispatcherTimer TimerFileLoad = null;
        public delegate void TimerTicks();
        public event TimerTicks TimerTick = delegate { };
        ObservableCollection<ImageModel> PImageList = null;
        List<string> DirList = new List<string>();
        Object searchingLocker = new Object();

        public LoadImage(ObservableCollection<ImageModel> PImageList, MainWindowModel parent)
        {
            ParentWindow = parent;
            this.PImageList = PImageList;

            /*TimerFileLoad = new DispatcherTimer();
            TimerFileLoad.Interval = new TimeSpan(0, 0, 0, 3, 0);
            TimerFileLoad.Tick += FileLoad_Tick;
            TimerFileLoad.Start();*/

            if(this.PImageList != null)
            {
                FileLoad_Tick(null, null);
            }
        }

        public async void FileLoad_Tick(object sender, EventArgs e)
        {
            await GetFiles(PImageList);
        }

        public void Start()
        {
            /*if(PImageList != null)
            {
                TimerFileLoad.Start();
            }*/
        }

        private async Task GetFiles(ObservableCollection<ImageModel> PImageList)
        {
            await Task.Run(delegate { 
                try
                {
                    lock(searchingLocker)
                    {
                        path = Explorer.FilePath;
                        int imgCount = PImageList.Count;
                        DirList.Clear();
                        GetDirsFrom(path, DirList);
                        if (imgCount == 0)
                        {
                            ParentWindow.FileManager_FileManagerStart();
                            ParentWindow.FileManager_ProgressStatus($"Поиск медиа-файлов в {path}...");
                            ParentWindow.FileManager_ProgressMax(DirList.Count - 1);
                        }
                        //var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(str => str.IsAcceptable());
                        List<string> files = new List<string>();
                        for (int i = 0; i < DirList.Count; i++)
                        {
                            var item = DirList[i];
                            try
                            {
                                files.AddRange(Directory.GetFiles(item, "*.*", SearchOption.TopDirectoryOnly).Where(str => str.IsAcceptable()));
                            }
                            catch (Exception)
                            { }
                            finally
                            {
                                if (imgCount == 0)
                                {
                                    ParentWindow.FileManager_ProgressValue(i);
                                }
                            }
                        }

                        if (files != null && files.Count != 0)
                        {
                            if (imgCount == 0)
                            {
                                //влияет на порядок загрузки элементов в окне
                                var files1 = files.OrderByDescending(d => new FileInfo(d).CreationTime).ToList();

                                int count = files1.Count;
                                ParentWindow.FileManager_ProgressMax(count - 1);
                                ParentWindow.FileManager_ProgressStatus("Загрузка файлов...");
                                for (int i = 0; i < count; i++)
                                {
                                    var item = files1[i];
                                    ParentWindow.FileManager_ProgressValue(i);
                                    CheckList(PImageList, item, true);
                                }
                            }
                            else
                            {
                                var files1 = files.OrderBy(d => new FileInfo(d).CreationTime).ToList();

                                foreach (var item in files1)
                                {
                                    if (PImageList.Where(c => c.ImageName == item).Count() == 0)
                                    {
                                        CheckList(PImageList, item);
                                    }
                                }

                                for (int i = 0; i < PImageList.Count; i++)
                                {
                                    if (!File.Exists(PImageList[i].ImageName))
                                    {
                                        PImageList.RemoveAt(i);
                                    }
                                }
                                //TimerTick();
                            }
                        }
                        else
                        {
                            ParentWindow.FileManager_ProgressStatus($"В {path} медиа-файлы не найдены!");
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private static void GetDirsFrom(string path, List<string> toList)
        {
            toList.Add(path);
            var dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            foreach (var item in dirs)
            {
                try
                {
                    GetDirsFrom(item, toList);
                }
                catch (Exception)
                {}
            }
        }

        private static void CheckList(ObservableCollection<ImageModel> PImageList, string item, bool IsFirst = false)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                ImageModel newItem = new ImageModel
                {
                    ImageName = item,
                    DateTime = File.GetCreationTime(item).ToFileTime()
                };
                if (IsFirst)
                {
                    PImageList.Add(newItem);
                }
                else
                {
                    PImageList.Insert(0, newItem);
                }
            }));
        }
    }
}
