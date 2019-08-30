using Gallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Management;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Gallery.Utilits
{
    class UsbManager
    {
        // Для перехвата сообщений в WPF
        private const int WM_DRAWCLIPBOARD = 0x0308;
        private HwndSource hwndSource = null;

        private const int WM_DEVICECHANGE = 0x0219;
        /// <summary> Подключение устройства </summary>
        private const int DBT_DEVICEARRIVAL = 0x8000;
        /// <summary> Отключение устройства </summary>
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const uint DBT_DEVTYP_VOLUME = 0x00000002;

        public delegate void DConnectUSB(USBModel model);
        public event DConnectUSB ConnectUSB = delegate { };

        public delegate void DDisconnectUSB(USBModel model);
        public event DDisconnectUSB DisconnectUSB = delegate { };

        public delegate void NConnectUSB();
        public event NConnectUSB SConnectUSB = delegate { };

        public delegate void NDisconnectUSB();
        public event NDisconnectUSB SDisconnectUSB = delegate { };

        [StructLayout(LayoutKind.Sequential)]
        private struct DEV_BROADCAST_HDR
        {
            public uint dbch_size;
            public uint dbch_devicetype;
            public uint dbch_reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DEV_BROADCAST_VOLUME
        {
            public uint dbcv_size;
            public uint dbcv_devicetype;
            public uint dbcv_reserved;
            public uint dbcv_unitmask;
            public ushort dbcv_flags;//public System.UInt16
        }

        DispatcherTimer ConncetTimerTher = null;
        DispatcherTimer DisconectTimerTher = null;

        public UsbManager()
        {
            ConncetTimerTher = new DispatcherTimer();
            ConncetTimerTher.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ConncetTimerTher.Tick += ConncetTimerTher_Tick;

            DisconectTimerTher = new DispatcherTimer();
            DisconectTimerTher.Interval = new TimeSpan(0, 0, 0, 1, 0);
            DisconectTimerTher.Tick += DisconectTimerTher_Tick; ;
        }

        private void DisconectTimerTher_Tick(object sender, EventArgs e)
        {
            DisconectTimerTher.Stop();
            SDisconnectUSB();
            List<USBModel> list = RemoveUSB();
            foreach (USBModel item in list)
            {
                DisconnectUSB(item);
            }
        }

        private void ConncetTimerTher_Tick(object sender, EventArgs e)
        {
            ConncetTimerTher.Stop();
            SConnectUSB();
            List<USBModel> list = CheckValidUSB();
            foreach (USBModel item in list)
            {
                ConnectUSB(item);
            }
        }

        public List<USBModel> GetActiveUSB()
        {
            // Получаем список зарегестрированных устройств
            List<USBModel> Register = Explorer.uSBModels;
            // Получаем список зарегистрированных
            return CheckListUSB(Register, uSBModels);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DRAWCLIPBOARD)
            {
                // обрабатываем сообщение
            }

            if (msg == WM_DEVICECHANGE)
            {
                if (wParam.ToInt32() == DBT_DEVICEARRIVAL || wParam.ToInt32() == DBT_DEVICEREMOVECOMPLETE)
                {
                    var dbhARRIVAL = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_HDR));
                    if (dbhARRIVAL.dbch_devicetype == DBT_DEVTYP_VOLUME)
                    {
                        var dbv =
                            (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(lParam, typeof(DEV_BROADCAST_VOLUME));

                        int DriveLetter = 0;
                        // Далее ищем установленный бит и получаем нужную букву
                        while ((dbv.dbcv_unitmask & (1 << DriveLetter)) != dbv.dbcv_unitmask && DriveLetter != 32)
                            DriveLetter++;

                        // Буква USB dev
                        var label = (char)('A' + DriveLetter);
                        string status = "";
                        switch (wParam.ToInt32())
                        {
                            case DBT_DEVICEARRIVAL:
                                status = "Подключен";
                                ConncetTimerTher.Start();

                                break;
                            case DBT_DEVICEREMOVECOMPLETE:
                                status = "Отключен";
                                DisconectTimerTher.Start();
                                break;
                        }
                    }
                }
            }
            return IntPtr.Zero;
        }

        private List<USBModel> uSBModels = null;

        public List<USBModel> GetUsb()
        {
            List<USBModel> devices = new List<USBModel>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    if(drive.IsReady)
                    {
                        devices.Add(new USBModel()
                        {
                            Name = drive.VolumeLabel,
                            Size = drive.TotalSize,
                            VolumeLabel = drive.Name,
                            SerialNumber = GetUSBSerialNumber(drive.Name)
                        });
                    }
                }
            }

            return devices;
        }

        public void USBMonitorStart(Window window)
        {
            hwndSource = PresentationSource.FromVisual(window) as HwndSource;
            hwndSource.AddHook(WndProc);
            uSBModels = GetUsb();
        }

        public void USBMonitorStop()
        {
            if(hwndSource != null)
            {
                hwndSource.RemoveHook(WndProc);
            }
        }

        public List<USBModel> CheckValidUSB()
        {
            // Определяем какие новые устройства
            List<USBModel> list = USBListDifference();
            // Получаем список зарегестрированных устройств
            List<USBModel> Register = Explorer.uSBModels;
            // Получаем список зарегистрированных
            return CheckListUSB(Register, list);
        }

        private List<USBModel> USBListDifference()
        {
            // Получаем список подключённых USB устройств
            List<USBModel> listcon = GetUsb();

            // Проверяем какие уже есть обработтанные в списке
            if (uSBModels.Count != 0)
            {
                // Получаем спиисок новых USB устройств
                List<USBModel> newUSB = DifferencelList(uSBModels, listcon);
                // Допоолняем список к уже имеющимся
                uSBModels.AddRange(newUSB);
                // Возвращяемм список
                listcon = newUSB;
            }
            else
            {
                // Если список пустой добавляем в него устройства
                uSBModels.AddRange(listcon);
            }

            return listcon;
        }

        private List<USBModel> DifferencelList(List<USBModel> list, List<USBModel> uSBModels)
        {
            List<USBModel> newUSB = new List<USBModel>(uSBModels);

            for(int i = 0; i < list.Count(); i++)
            {
                newUSB.RemoveAll(w=>w.SerialNumber == list[i].SerialNumber);
            }

            return newUSB;
        }

        private List<USBModel> CheckListUSB(List<USBModel> Register, List<USBModel> list)
        {
            List<USBModel> rtn = new List<USBModel>();
            if(Register.Count >= list.Count)
            {
                foreach (var item in Register)
                {
                    USBModel elm = list.FirstOrDefault(c => c.SerialNumber == item.SerialNumber);
                    if (elm != null)
                    {
                        elm.PathImage = item.PathImage;
                        rtn.Add(elm);
                    }
                }
            }
            else
            {
                foreach (var item in list)
                {
                    USBModel elm = Register.FirstOrDefault(c => c.SerialNumber == item.SerialNumber);
                    if (elm != null)
                    {
                        item.PathImage = elm.PathImage;
                        rtn.Add(item);
                    }
                }
            }

            return rtn;
        }

        private List<USBModel> RemoveUSB()
        {
            List<USBModel> newUsb = new List<USBModel>();
            List<USBModel> list = GetUsb();

            newUsb.AddRange(uSBModels);

            for(int i = 0; i < list.Count(); i++)
            {
                newUsb.RemoveAll(w=>w.SerialNumber == list[i].SerialNumber);
            }

            for (int i = 0; i < newUsb.Count(); i++)
            {
                uSBModels.Remove(newUsb[i]);
            }

            List<USBModel> Register = Explorer.uSBModels;
            // Получаем список зарегистрированных
            return CheckListUSB(Register, newUsb);
        }

        // Функция возвращяет False, если аргумент является сьемным носителем
        public bool CheckNoUSBDisk(string Disk)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if(Disk == d.Name && d.DriveType == DriveType.Fixed)
                    return true;
            }

            return false;
        }

        private string GetUSBSerialNumber(string Disk)
        {
            Disk = Disk.Substring(0, 2);
            string serial = "";
            var index = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDiskToPartition").Get().Cast<ManagementObject>();
            var disks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive").Get().Cast<ManagementObject>();
            try
            {

                var drive = (from i in index where i["Dependent"].ToString().Contains(Disk) select i).FirstOrDefault();
                var key = drive["Antecedent"].ToString().Split('#')[1].Split(',')[0];

                var disk = (from d in disks
                            where
                                d["Name"].ToString() == "\\\\.\\PHYSICALDRIVE" + key &&
                                d["InterfaceType"].ToString() == "USB"
                            select d).FirstOrDefault();
                return serial = disk["PNPDeviceID"].ToString().Split('\\').Last();
            }
            catch
            {
               
            }

            return string.Empty;
        }
    }
}
