using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data.OleDb;
using MySql.Data.MySqlClient;

namespace ReleaseControlLib
{
    public class StorageService
    {
        static ObservableCollection<ControlledApp> apps = new ObservableCollection<ControlledApp>();

        public static string Provider { get; set; } = "";
        public static ConnectionTypes ConnectionType { get; set; } = ConnectionTypes.Sql;
        public static string Server { get; set; } = "";
        public static string Database { get; set; } = "";
        public static string Table { get; set; } = "";
        public static string User { get; set; } = "";
        public static string Password { get; set; } = "";
        public static string Port { get; set; } = "";
        public static StorageTypes StorageType { get; set; } = StorageTypes.Database;
        public static string FilePath { get; set; } = "";
        public static ObservableCollection<ControlledApp> Apps
        {
            get { return apps; }
            set
            {
                apps = value;
            }
        }
        /// <summary>
        /// Получить данные из базы данных
        /// </summary>
        /// <returns></returns>
        public static string GetDataFromDb()
        {
            try
            {
                Apps.Clear();
                switch (ConnectionType)
                {
                    case ConnectionTypes.OleDb:
                        OleDbConnection oleDbConnection = new OleDbConnection();
                        oleDbConnection.ConnectionString = string.Format("Provider={0}; Data Source={1};User ID={2};Password={3};Connection Timeout=3; ",Provider, Server, User, Password);
                        oleDbConnection.Open();
                        OleDbCommand oleDbCommand = oleDbConnection.CreateCommand();
                        oleDbCommand.CommandText = string.Format("SELECT Name, WorkFolder, ReleaseFolder, ReestrFolder From {0}", Table);
                        OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
                        while (oleDbDataReader.Read())
                        {
                            Apps.Add(ControlledApp.AddApp(oleDbDataReader["Name"].ToString(), oleDbDataReader["WorkFolder"].ToString(), oleDbDataReader["ReleaseFolder"].ToString(), oleDbDataReader["ReestrFolder"].ToString()));
                        }
                        oleDbConnection.Close();
                        break;
                    case ConnectionTypes.OracleMySql:
                        MySqlConnection mySqlConnection = new MySqlConnection();
                        mySqlConnection.ConnectionString = string.Format("host={0};port={1};User Id={2};database={3};password={4};character set=utf8", Server, Port, User, Database, Password);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                        mySqlCommand.CommandText = string.Format("SELECT Name, WorkFolder, ReleaseFolder, ReestrFolder From {0}", Table);
                        MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
                        while (mySqlDataReader.Read())
                        {
                            Apps.Add(ControlledApp.AddApp(mySqlDataReader["Name"].ToString(), mySqlDataReader["WorkFolder"].ToString(), mySqlDataReader["ReleaseFolder"].ToString(), mySqlDataReader["ReestrFolder"].ToString()));
                        }
                        mySqlConnection.Close();
                        break;
                    case ConnectionTypes.Sql:
                        SqlConnection sqlConnection = new SqlConnection();
                        sqlConnection.ConnectionString = string.Format("host={0};port={1};User Id={2};database={3};password={4};character set=utf8", Server, Port, User, Database, Password);
                        sqlConnection.Open();
                        SqlCommand sqlCommand = sqlConnection.CreateCommand();
                        sqlCommand.CommandText = string.Format("SELECT Name, WorkFolder, ReleaseFolder, ReestrFolder From {0}", Table);
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            Apps.Add(ControlledApp.AddApp(sqlDataReader["Name"].ToString(), sqlDataReader["WorkFolder"].ToString(), sqlDataReader["ReleaseFolder"].ToString(), sqlDataReader["ReestrFolder"].ToString()));
                        }
                        sqlConnection.Close();
                        break;
                }
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }
        /// <summary>
        /// Получить данные о приложениях из XML файла
        /// </summary>
        /// <returns></returns>
        public static string GetDataFromXml()
        {
            try
            {
                Apps.Clear();
                if (File.Exists(FilePath))
                {
                    XDocument xdoc = XDocument.Load(FilePath);
                    foreach (XElement app in xdoc.Element("Applications").Elements("App"))
                    {
                        XElement name = app.Element("Name");
                        XElement workFolder = app.Element("WorkFolderPath");
                        XElement releaseFolder = app.Element("ReleaseFolderPath");
                        XElement reestrFolder = app.Element("ReestrFolderPath");
                        ControlledApp tApp = ControlledApp.AddApp(
                            name.Value.ToString(),
                            workFolder.Value==null?"":workFolder.Value.ToString(),
                            releaseFolder==null?"":releaseFolder.Value.ToString(),
                            reestrFolder==null?"":reestrFolder.Value.ToString()
                            );
                        Apps.Add(tApp);
                    }
                }
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
        /// <summary>
        /// Добавить новую запись в БД
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static string AddDataToDb(ControlledApp app)
        {
            try
            {
                switch (ConnectionType)
                {
                    case ConnectionTypes.OleDb:
                        OleDbConnection oleDbConnection = new OleDbConnection();
                        oleDbConnection.ConnectionString = string.Format("Provider={0}; Data Source={1};User ID={2};Password={3};Connection Timeout=3; ", Provider, Server, User, Password);
                        oleDbConnection.Open();
                        OleDbCommand oleDbCommand = oleDbConnection.CreateCommand();
                        oleDbCommand.CommandText = string.Format("INSERT {0} (Name, WorkFolder, ReleaseFolder, ReestrFolder) VALUES ('{1}' ,'{2}', '{3}', '{4}')", Table, app.Name, app.WorkingReleasePath, app.ReleasePath, app.ReleasePath);
                        oleDbCommand.ExecuteNonQuery();
                        oleDbConnection.Close();
                        break;
                    case ConnectionTypes.OracleMySql:
                        MySqlConnection mySqlConnection = new MySqlConnection();
                        mySqlConnection.ConnectionString = string.Format("host={0};port={1};User Id={2};database={3};password={4};character set=utf8", Server, Port, User, Database, Password);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                        mySqlCommand.CommandText = string.Format("INSERT {0} (Name, WorkFolder, ReleaseFolder, ReestrFolder) VALUES ('{1}' ,'{2}', '{3}', '{4}')", Table, app.Name, app.WorkingReleasePath, app.ReleasePath, app.ReleasePath);
                        mySqlCommand.ExecuteNonQuery();
                        mySqlConnection.Close();
                        break;
                    case ConnectionTypes.Sql:
                        SqlConnection sqlConnection = new SqlConnection();
                        sqlConnection.ConnectionString = string.Format("host={0};port={1};User Id={2};database={3};password={4};character set=utf8", Server, Port, User, Database, Password);
                        sqlConnection.Open();
                        SqlCommand sqlCommand = sqlConnection.CreateCommand();
                        sqlCommand.CommandText = string.Format("INSERT {0} (Name, WorkFolder, ReleaseFolder, ReestrFolder) VALUES ('{1}' ,'{2}', '{3}', '{4}')", Table, app.Name, app.WorkingReleasePath, app.ReleasePath, app.ReleasePath);
                        sqlCommand.ExecuteNonQuery();
                        sqlConnection.Close();
                        break;
                }
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// Обновить запись в БД
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static string UpdateDataInDb(ControlledApp app)
        {
            try
            {
                switch (ConnectionType)
                {
                    case ConnectionTypes.OleDb:
                        OleDbConnection oleDbConnection = new OleDbConnection();
                        oleDbConnection.ConnectionString = string.Format("Provider={0}; Data Source={1};User ID={2};Password={3};Connection Timeout=3; ", Provider, Server, User, Password);
                        oleDbConnection.Open();
                        OleDbCommand oleDbCommand = oleDbConnection.CreateCommand();
                        oleDbCommand.CommandText = string.Format("UPDATE {0} SET Name='{1}', WorkFolder='{2}', ReleaseFolder='{3}', ReestrFolder='{4}' WHERE Name = '{1}')", Table, app.Name, app.WorkingReleasePath, app.ReleasePath, app.ReleasePath);
                        oleDbCommand.ExecuteNonQuery();
                        oleDbConnection.Close();
                        break;
                    case ConnectionTypes.OracleMySql:
                        MySqlConnection mySqlConnection = new MySqlConnection();
                        mySqlConnection.ConnectionString = string.Format("host={0};port={1};User Id={2};database={3};password={4};character set=utf8", Server, Port, User, Database, Password);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                        mySqlCommand.CommandText = string.Format("UPDATE {0} SET Name='{1}', WorkFolder='{2}', ReleaseFolder='{3}', ReestrFolder='{4}' WHERE Name = '{1}')", Table, app.Name, app.WorkingReleasePath, app.ReleasePath, app.ReleasePath);
                        mySqlCommand.ExecuteNonQuery();
                        mySqlConnection.Close();
                        break;
                    case ConnectionTypes.Sql:
                        SqlConnection sqlConnection = new SqlConnection();
                        sqlConnection.ConnectionString = string.Format("host={0};port={1};User Id={2};database={3};password={4};character set=utf8", Server, Port, User, Database, Password);
                        sqlConnection.Open();
                        SqlCommand sqlCommand = sqlConnection.CreateCommand();
                        sqlCommand.CommandText = string.Format("UPDATE {0} SET Name='{1}', WorkFolder='{2}', ReleaseFolder='{3}', ReestrFolder='{4}' WHERE Name = '{1}')", Table, app.Name, app.WorkingReleasePath, app.ReleasePath, app.ReleasePath);
                        sqlCommand.ExecuteNonQuery();
                        sqlConnection.Close();
                        break;
                }
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// Сохранить все данные в файле XML
        /// </summary>
        /// <returns>Errors</returns>
        public static string SaveToXml()
        {
            try
            {
                XDocument xdoc = new XDocument();
                if (File.Exists(FilePath))
                {
                    xdoc = XDocument.Load(FilePath);
                    xdoc.RemoveNodes();
                }
                XElement rootNode = new XElement("Applications");
                foreach (var app in Apps)
                {
                    XElement currApp = new XElement("App");
                    currApp.Add(new XElement("Name", app.Name));
                    currApp.Add(new XElement("ReleaseFolderPath", app.ReleasePath));
                    currApp.Add(new XElement("WorkFolderPath", app.WorkingReleasePath));
                    currApp.Add(new XElement("ReestrFolderPath", app.ReestrPath));
                    rootNode.Add(currApp);
                }
                xdoc.Add(rootNode);
                xdoc.Save(FilePath);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }

        //TODO: Проверка на полноту введенных данных

        /// <summary>
        /// Сохранить данные 
        /// </summary>
        /// <returns></returns>
        public static string Save()
        {
            try
            {
                string errors = "";
                switch (StorageType)
                {
                    case StorageTypes.Database:
                        foreach (var app in Apps)
                        {
                            if (app.ExistInDb)
                            {
                                var er = UpdateDataInDb(app);
                                errors += er != "OK" ? er : "";
                            }
                            else
                            {
                                var er = AddDataToDb(app);
                                errors += er != "OK" ? er : "";
                            }
                        }
                        break;
                    case StorageTypes.XML:
                        return SaveToXml();
                        
                }
                return errors;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
    }

    public enum ConnectionTypes
    {
        Sql,
        OracleMySql,
        OleDb
    }
    public enum StorageTypes
    {
        Database,
        XML
    }
}
