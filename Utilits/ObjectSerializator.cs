using Gallery.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Utilits
{
    class ObjectSerializator
    {
        // Сериализация в строку
        static public void SerializeToString(object obj, out string serializedObject)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            System.IO.MemoryStream resultStream = new System.IO.MemoryStream();
            try
            {
                binFormatter.Serialize(resultStream, obj);
                resultStream.Flush();
                serializedObject = Convert.ToBase64String(resultStream.ToArray(), Base64FormattingOptions.None);
            }
            catch (Exception)
            {
                serializedObject = string.Empty;
            }
            finally
            {
                resultStream.Close();
            }
        }

        static public string SerializeToString(object obj)
        {
            string serializedObject = string.Empty;
            BinaryFormatter binFormatter = new BinaryFormatter();

            System.IO.MemoryStream resultStream = new System.IO.MemoryStream();
            try
            {
                binFormatter.Serialize(resultStream, obj);
                resultStream.Flush();
                serializedObject = Convert.ToBase64String(resultStream.ToArray(), Base64FormattingOptions.None);
            }
            catch (Exception exc)
            {
                serializedObject = exc.Message;//string.Empty;
            }
            finally
            {
                resultStream.Close();
            }
            return serializedObject;
        }

        // Десериализация из строки
        static public object DeserializeFromString(string s)
        {
            object result = null;
            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    // Получение массива байт
                    byte[] serializedData = Convert.FromBase64String(s);

                    // Десериализация
                    System.IO.MemoryStream memStream = new System.IO.MemoryStream(serializedData);
                    BinaryFormatter binFormatter = new BinaryFormatter();
                    result = binFormatter.Deserialize(memStream);
                    memStream.Close();
                }
                catch (Exception exc)
                {
                    result = exc.Message;
                }
            }
            return result;
        }

        static public byte[] SerializeToBytes(object obj)
        {
            byte[] serializedObject = null;
            BinaryFormatter binFormatter = new BinaryFormatter();

            System.IO.MemoryStream resultStream = new System.IO.MemoryStream();
            // Запись параметров
            try
            {
                binFormatter.Serialize(resultStream, obj);
                resultStream.Flush();
                serializedObject = resultStream.ToArray();
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(exc.ToString());
            }
            finally
            {
                resultStream.Close();
            }
            return serializedObject;
        }

        static public string SerializeToStream(object obj, System.IO.Stream openedStream)
        {
            string error = null;
            BinaryFormatter binFormatter = new BinaryFormatter();

            // Запись параметров
            try
            {
                binFormatter.Serialize(openedStream, obj);
            }
            catch (Exception exc)
            {
                error = exc.Message;
            }

            return error;
        }

        // Десериализация из строки
        static public object DeserializeFromBytes(byte[] serializedData)
        {
            object result = null;
            if (serializedData != null)
            {
                try
                {
                    // Десериализация
                    System.IO.MemoryStream memStream = new System.IO.MemoryStream(serializedData);
                    BinaryFormatter binFormatter = new BinaryFormatter();
                    result = binFormatter.Deserialize(memStream);
                    memStream.Close();
                }
                catch (Exception exc)
                {
                    result = exc.Message;
                }
            }
            return result;
        }

        // Десериализация из потока
        static public object DeserializeFromStream(System.IO.Stream serializedData)
        {
            object result = null;
            if (serializedData != null)
            {
                try
                {
                    // Десериализация
                    BinaryFormatter binFormatter = new BinaryFormatter();
                    result = binFormatter.Deserialize(serializedData);
                }
                catch (Exception exc)
                {
                    result = exc.Message;
                }
            }
            return result;
        }

        static public void SaveConfig(SettingsModel cfg)
        {
            string Config = "Config.bin";
            byte[] buff = SerializeToBytes(cfg);

            using (FileStream fstream = File.Create(Config))
            {
                fstream.Write(buff, 0, buff.Length);
            }

        }

        static public SettingsModel LoadConfig()
        {
            string Config = "Config.bin";

            try
            {
                using (FileStream fstream = File.OpenRead(Config))
                {
                    // преобразуем строку в байты
                    byte[] array = new byte[fstream.Length];
                    // считываем данные
                    fstream.Read(array, 0, array.Length);
                    // декодируем байты в строку
                    return DeserializeFromBytes(array) as SettingsModel;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
