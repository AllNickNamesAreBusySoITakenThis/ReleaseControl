using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using ReleaseControlLib;
using NLog;
using System.Collections.ObjectModel;


namespace ControlApp
{
    public class Service
    {
        const string SettingsAdress = @"\SemSettings\ReleaseControl\Config.xml";

        static Logger logger = LogManager.GetCurrentClassLogger();

        #region Configuration
        /// <summary>
        /// Получили словарь настроек
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> GetSettingsDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("Database", StorageService.Database);
            result.Add("Apps file path", StorageService.FilePath);
            result.Add("Database password", StorageService.Password);
            result.Add("Database port (MySql)", StorageService.Port);
            result.Add("Database provider (OleDb)",StorageService.Provider);
            result.Add("Database server", StorageService.Server);
            result.Add("Database table name", StorageService.Table);
            result.Add("Database user", StorageService.User);
            result.Add("Storage type", StorageService.StorageType.ToString());
            result.Add("Database type", StorageService.ConnectionType.ToString());
            result.Add("Forbidden extentions", CollectionToString(StorageService.ForbiddenExt));
            return result;
        }
        /// <summary>
        /// Смотрим словарь настроек и согласно ключам заполняем  настройки приложения
        /// </summary>
        /// <param name="dictionary"></param>
        public static void ParseDictionary(Dictionary<string, object> dictionary)
        {
            try
            {
                StorageService.Database = dictionary["Database"].ToString();
                StorageService.FilePath = dictionary["Apps file path"].ToString();
                StorageService.Password = dictionary["Database password"].ToString();
                StorageService.Port = dictionary["Database port (MySql)"].ToString();
                StorageService.Provider = dictionary["Database provider (OleDb)"].ToString();
                StorageService.Server = dictionary["Database server"].ToString();
                StorageService.Table = dictionary["Database table name"].ToString();
                StorageService.User = dictionary["Database user"].ToString();
                StorageService.StorageType = (StorageTypes)Enum.Parse(typeof(StorageTypes),dictionary["Storage type"].ToString());
                StorageService.ConnectionType = (ConnectionTypes)Enum.Parse(typeof(ConnectionTypes), dictionary["Database type"].ToString());
                StorageService.ForbiddenExt = CollectionFromString(dictionary["Forbidden extentions"].ToString());
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("Ошибка чтения настроек: {0}", ex.Message));
            }

        }

        public static string CollectionToString(ObservableCollection<string> collection)
        {
            string res = "";
            foreach(var item in collection)
            {
                res += string.Format("{0};", item);
            }
            return res;
        }

        public static ObservableCollection<string> CollectionFromString(string source)
        {
            ObservableCollection<string> result = new ObservableCollection<string>();
            var sSource = source.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var data in sSource)
            {
                result.Add(data);
            }
            return result;
        }

        /// <summary>
        /// Читаем настройки из XML-файла
        /// </summary>
        public static void ReadSettings()
        {
            try
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + SettingsAdress))
                {
                    var dict = GetSettingsDictionary();
                    XDocument xdoc = XDocument.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + SettingsAdress);
                    foreach (XElement Element in xdoc.Element("ApplicationConfiguration").Element("Settings").Elements("Setting"))
                    {
                        XElement SettingName = Element.Element("Name");
                        XElement SettingValue = Element.Element("Value");
                        if (SettingName != null && SettingValue != null)
                        {
                            object temp = new object();
                            if (dict.ContainsKey(SettingName.Value))
                            {
                                dict[SettingName.Value] = SettingValue.Value;
                            }
                        }
                    }
                    ParseDictionary(dict);
                }
                else
                {
                    GenerateXML_Config();
                }
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("Ошибка чтения файла конфигурации: {0}", ex.Message));
            }
        }
        /// <summary>
        /// Сохраняем настройки в XML-файл
        /// </summary>
        public static void SaveSettings()
        {
            try
            {
                //получили словарь настроек
                var dict = GetSettingsDictionary();
                //открыли документ
                XDocument xdoc = XDocument.Load(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + SettingsAdress);
                foreach (var setting in dict)
                {
                    bool flag = false;
                    int counter = 0;
                    foreach (XElement Element in xdoc.Element("ApplicationConfiguration").Elements("Settings").Elements("Setting"))
                    {
                        XElement SettingName = Element.Element("Name");
                        XElement SettingValue = Element.Element("Value");
                        counter++;
                        if (setting.Key == SettingName.Value)
                        {

                            Element.SetElementValue(SettingValue.Name, dict[SettingName.Value] == null ? "" : dict[SettingName.Value]);
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        xdoc.Element("ApplicationConfiguration").Element("Settings").Add(new XElement("Setting", new XElement("ID", counter + 1), new XElement("Name", setting.Key), new XElement("Value", setting.Value), new XElement("Description")));
                    }
                }

                xdoc.Save(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + SettingsAdress);
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("Ошибка записи файла конфигурации: {0}", ex.Message));
            }

        }
        
        /// <summary>
        /// Сгенерировать новый XML файл Config.XML
        /// </summary>
        public static void GenerateXML_Config()
        {
            try
            {               
                #region Config.xml
                if (!Directory.Exists(new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + SettingsAdress).Directory.FullName))
                    Directory.CreateDirectory(new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + SettingsAdress).Directory.FullName);
                XmlTextWriter textWritter = new XmlTextWriter(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + SettingsAdress, Encoding.UTF8);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("ApplicationConfiguration");
                textWritter.WriteEndElement();
                textWritter.Close();
                //получили словарь настроек
                var dict = GetSettingsDictionary();
                XDocument xDoc = new XDocument();
                var parentElement = new XElement("ApplicationConfiguration");
                var settingsGroupElement = new XElement("Settings");
                int counter = 0;
                foreach (var setting in dict)
                {
                    settingsGroupElement.Add(new XElement("Setting", new XElement("ID", counter), new XElement("Name", setting.Key), new XElement("Value", setting.Value), new XElement("Description")));
                    counter++;
                }
                counter = 0;

                parentElement.Add(settingsGroupElement);
                xDoc.Add(parentElement);
                xDoc.Save(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + SettingsAdress);
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("Ошибка генерации файла конфигурации: {0}", ex.Message));
            }

        }
        #endregion

    }
}
