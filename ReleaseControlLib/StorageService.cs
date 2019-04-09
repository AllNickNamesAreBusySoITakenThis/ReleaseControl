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
        public static ObservableCollection<string> ForbiddenExt { get; set; } = new ObservableCollection<string>();
        public static ObservableCollection<ControlledApp> Apps
        {
            get { return apps; }
            set
            {
                apps = value;
            }
        }
        
        static StorageService()
        {
            ForbiddenExt.CollectionChanged += ForbiddenExt_CollectionChanged;
        }
        private static void ForbiddenExt_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CheckData();
        }

        static void CheckData()
        {
            foreach(var app in Apps)
            {
                for(int i=0;i<app.Files.Count;i++)
                {
                    if(ForbiddenExt.Contains(new FileInfo(app.WorkingReleasePath+Path.DirectorySeparatorChar+app.Files[i].Path).Extension))
                    {
                        app.Files.RemoveAt(i);
                        i = -1;
                    }
                }
            }
        }
        /// <summary>
        /// Получить данные из базы данных
        /// </summary>
        /// <returns></returns>
        static string GetDataFromDb()
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
                        oleDbCommand.CommandText = string.Format("SELECT Name, WorkFolder, ReleaseFolder, ReestrFolder, ID From {0}", Table);
                        OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
                        while (oleDbDataReader.Read())
                        {
                            Apps.Add(ControlledApp.AddApp(oleDbDataReader["Name"].ToString(), oleDbDataReader["WorkFolder"].ToString(), oleDbDataReader["ReleaseFolder"].ToString(), oleDbDataReader["ReestrFolder"].ToString(), Convert.ToInt32(oleDbDataReader["ID"].ToString())));
                        }
                        oleDbConnection.Close();
                        break;
                    case ConnectionTypes.OracleMySql:
                        MySqlConnection mySqlConnection = new MySqlConnection();
                        mySqlConnection.ConnectionString = string.Format("host={0};port={1};User Id={2};database={3};password={4};character set=utf8", Server, Port, User, Database, Password);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                        mySqlCommand.CommandText = string.Format("SELECT Name, WorkFolder, ReleaseFolder, ReestrFolder, ID From {0}", Table);
                        MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
                        while (mySqlDataReader.Read())
                        {
                            Apps.Add(ControlledApp.AddApp(mySqlDataReader["Name"].ToString(), mySqlDataReader["WorkFolder"].ToString().Replace('+',Path.DirectorySeparatorChar ), mySqlDataReader["ReleaseFolder"].ToString().Replace('+', Path.DirectorySeparatorChar), mySqlDataReader["ReestrFolder"].ToString().Replace('+', Path.DirectorySeparatorChar), Convert.ToInt32(mySqlDataReader["ID"].ToString())));
                        }
                        mySqlConnection.Close();
                        break;
                    case ConnectionTypes.Sql:
                        SqlConnection sqlConnection = new SqlConnection();
                        if (Password != "")
                            sqlConnection.ConnectionString = string.Format("Data Source={0};User ={1};Initial Catalog={2};Password={3}; Integrated Security = false;", Server, User, Database, Password);
                        else
                            sqlConnection.ConnectionString = string.Format("Data Source={0};Initial Catalog={1}; Integrated Security = True;", Server, Database);
                        sqlConnection.Open();
                        SqlCommand sqlCommand = sqlConnection.CreateCommand();
                        sqlCommand.CommandText = string.Format("SELECT Name, WorkFolder, ReleaseFolder, ReestrFolder, ID From {0}", Table);
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            Apps.Add(ControlledApp.AddApp(sqlDataReader["Name"].ToString(), sqlDataReader["WorkFolder"].ToString(), sqlDataReader["ReleaseFolder"].ToString(), sqlDataReader["ReestrFolder"].ToString(), Convert.ToInt32(sqlDataReader["ID"].ToString())));
                        }
                        sqlConnection.Close();
                        break;
                }
                foreach(var app in Apps)
                {
                    app.ExistInDb = true;
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
        static string GetDataFromXml()
        {
            try
            {
                Apps.Clear();
                if (File.Exists(FilePath))
                {
                    XDocument xdoc = XDocument.Load(FilePath);
                    var counter = 0;
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
                            reestrFolder==null?"":reestrFolder.Value.ToString(),
                            counter
                            );
                        counter++;
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
        static string AddDataToDb(ControlledApp app)
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
                        mySqlCommand.CommandText = string.Format("INSERT {0} (Name, WorkFolder, ReleaseFolder, ReestrFolder) VALUES ('{1}' ,'{2}', '{3}', '{4}')", Table, app.Name, app.WorkingReleasePath.Replace(Path.DirectorySeparatorChar,'+'), app.ReleasePath.Replace(Path.DirectorySeparatorChar, '+'), app.ReleasePath.Replace(Path.DirectorySeparatorChar, '+'));
                        mySqlCommand.ExecuteNonQuery();
                        mySqlConnection.Close();
                        break;
                    case ConnectionTypes.Sql:
                        SqlConnection sqlConnection = new SqlConnection();
                        if (Password != "")
                            sqlConnection.ConnectionString = string.Format("Data Source={0};User ={1};Initial Catalog={2};Password={3}; Integrated Security = false;", Server, User, Database, Password);
                        else
                            sqlConnection.ConnectionString = string.Format("Data Source={0};Initial Catalog={1}; Integrated Security = True;", Server, Database);
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
        static string UpdateDataInDb(ControlledApp app)
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
                        oleDbCommand.CommandText = string.Format("UPDATE {0} SET Name='{1}', WorkFolder='{2}', ReleaseFolder='{3}', ReestrFolder='{4}' WHERE ID = {5}", Table, app.Name, app.WorkingReleasePath, app.ReleasePath, app.ReleasePath, app.Id);
                        oleDbCommand.ExecuteNonQuery();
                        oleDbConnection.Close();
                        break;
                    case ConnectionTypes.OracleMySql:
                        MySqlConnection mySqlConnection = new MySqlConnection();
                        mySqlConnection.ConnectionString = string.Format("host={0};port={1};User Id={2};database={3};password={4};character set=utf8", Server, Port, User, Database, Password);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                        mySqlCommand.CommandText = string.Format("UPDATE {0} SET Name='{1}', WorkFolder='{2}', ReleaseFolder='{3}', ReestrFolder='{4}' WHERE ID = {5}", Table, app.Name, app.WorkingReleasePath.Replace(Path.DirectorySeparatorChar, '+'), app.ReleasePath.Replace(Path.DirectorySeparatorChar, '+'), app.ReleasePath.Replace(Path.DirectorySeparatorChar, '+'), app.Id);
                        mySqlCommand.ExecuteNonQuery();
                        mySqlConnection.Close();
                        break;
                    case ConnectionTypes.Sql:
                        SqlConnection sqlConnection = new SqlConnection();
                        if (Password != "")
                            sqlConnection.ConnectionString = string.Format("Data Source={0};User ={1};Initial Catalog={2};Password={3}; Integrated Security = false;", Server, User, Database, Password);
                        else
                            sqlConnection.ConnectionString = string.Format("Data Source={0};Initial Catalog={1}; Integrated Security = True;", Server, Database);
                        sqlConnection.Open();
                        SqlCommand sqlCommand = sqlConnection.CreateCommand();
                        sqlCommand.CommandText = string.Format("UPDATE {0} SET Name='{1}', WorkFolder='{2}', ReleaseFolder='{3}', ReestrFolder='{4}' WHERE ID = {5}", Table, app.Name, app.WorkingReleasePath, app.ReleasePath, app.ReleasePath, app.Id);
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
        /// Удалить данные из таблицы
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        static string RemoveDataFormDb(ControlledApp app)
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
                        oleDbCommand.CommandText = string.Format("DELETE FROM {0} WHERE ID = {1}", Table, app.Id);
                        oleDbCommand.ExecuteNonQuery();
                        oleDbConnection.Close();
                        break;
                    case ConnectionTypes.OracleMySql:
                        MySqlConnection mySqlConnection = new MySqlConnection();
                        mySqlConnection.ConnectionString = string.Format("host={0};port={1};User Id={2};database={3};password={4};character set=utf8", Server, Port, User, Database, Password);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                        mySqlCommand.CommandText = string.Format("DELETE FROM {0} WHERE ID = {1}", Table, app.Id);
                        mySqlCommand.ExecuteNonQuery();
                        mySqlConnection.Close();
                        break;
                    case ConnectionTypes.Sql:
                        SqlConnection sqlConnection = new SqlConnection();
                        if(Password!="")
                            sqlConnection.ConnectionString = string.Format("Data Source={0};User ={1};Initial Catalog={2};Password={3}; Integrated Security = false;", Server,  User, Database, Password);
                        else
                            sqlConnection.ConnectionString = string.Format("Data Source={0};Initial Catalog={1}; Integrated Security = True;", Server, Database);
                        sqlConnection.Open();
                        SqlCommand sqlCommand = sqlConnection.CreateCommand();
                        sqlCommand.CommandText = string.Format("DELETE FROM {0} WHERE ID = {1}", Table, app.Id);
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
        static string SaveToXml()
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
        /// <summary>
        /// Проверка на полноту введенных данных
        /// </summary>
        /// <returns>True - данные введены в полном объеме, False - нет</returns>
        public static bool CheckSettings()
        {
            //bool verdict = true;
            switch(StorageType)
            {
                case StorageTypes.Database:
                    switch(ConnectionType)
                    {
                        case ConnectionTypes.OleDb:
                            if (Provider == "")
                                return false;
                            break;
                        case ConnectionTypes.OracleMySql:
                            if (Port == "")
                                return false;
                            break;                        
                    }
                    if (Server == "" | Database == "" | Table == "" | (User == "" & Password != "") | (User != "" & Password==""))
                    {
                        return false;
                    }
                    break;
                case StorageTypes.XML:
                    if(FilePath=="")
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }
        /// <summary>
        /// Получить данные
        /// </summary>
        /// <returns></returns>
        public static string Get()
        {
            try
            {
                switch(StorageType)
                {
                    case StorageTypes.Database:
                        return GetDataFromDb();
                    case StorageTypes.XML:
                        return GetDataFromXml();
                }
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// Сохранить данные 
        /// </summary>
        /// <returns></returns>
        public static string Save()
        {
            try
            {
                string errors = "";
                if(CheckSettings())
                {
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
                }
                else
                {
                    errors = "Настройка произведена не полностью";
                }
                return errors;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
        /// <summary>
        /// Удалить приложение
        /// </summary>
        /// <param name="appToRemove"></param>
        /// <returns></returns>
        public static string RemoveApp(ControlledApp appToRemove)
        {
            try
            {
                for(int i=0;i<Apps.Count;i++)
                {
                    if(appToRemove==Apps[i])
                    {
                        Apps.RemoveAt(i);
                        break;
                    }
                }
                switch(StorageType)
                {
                    case StorageTypes.Database:
                        RemoveDataFormDb(appToRemove);
                        break;
                    case StorageTypes.XML:
                        SaveToXml();
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
        /// Создать таблицу в БД
        /// </summary>
        /// <returns></returns>
        public static string CreateDbTable()
        {
            try
            {
                if(CheckSettings())
                {
                    switch (ConnectionType)
                    {
                        case ConnectionTypes.OleDb:
                            OleDbConnection oleDbConnection = new OleDbConnection();
                            oleDbConnection.ConnectionString = string.Format("Provider={0}; Data Source={1};User ID={2};Password={3};Connection Timeout=3; ", Provider, Server, User, Password);
                            oleDbConnection.Open();
                            OleDbCommand oleDbCommand = oleDbConnection.CreateCommand();
                            oleDbCommand.CommandText = string.Format("CREATE TABLE {0}.dbo.{1} (ID INT IDENTITY," +
                                                                  "Name VARCHAR(MAX) NULL," +
                                                                  "WorkFolder VARCHAR(MAX) NULL," +
                                                                  "ReleaseFolder VARCHAR(MAX) NULL," +
                                                                  "ReestrFolder VARCHAR(MAX) NULL," +
                                                                  "PRIMARY KEY CLUSTERED (ID))" +
                                                                  "ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]", Database, Table);
                            oleDbCommand.ExecuteNonQuery();
                            oleDbConnection.Close();
                            break;
                        case ConnectionTypes.OracleMySql:
                            MySqlConnection mySqlConnection = new MySqlConnection();
                            mySqlConnection.ConnectionString = string.Format("host={0};port={1};User Id={2};database={3};password={4};character set=utf8", Server, Port, User, Database, Password);
                            mySqlConnection.Open();
                            MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                            mySqlCommand.CommandText = string.Format("CREATE TABLE {0}.{1} (ID int(11) UNSIGNED NOT NULL AUTO_INCREMENT," +
                                                                  "Name varchar(500) DEFAULT NULL," +
                                                                  "WorkFolder varchar(2000) DEFAULT NULL," +
                                                                  "ReleaseFolder varchar(2000) DEFAULT NULL," +
                                                                  "ReestrFolder varchar(2000) DEFAULT NULL," +
                                                                  "PRIMARY KEY (ID))" +
                                                                  "ENGINE = INNODB, CHARACTER SET utf8, COLLATE utf8_general_ci, COMMENT = 'Контроль за актуальностью ППО';", Database, Table);
                            mySqlCommand.ExecuteNonQuery();
                            mySqlConnection.Close();
                            break;
                        case ConnectionTypes.Sql:
                            SqlConnection sqlConnection = new SqlConnection();
                            if (Password != "")
                                sqlConnection.ConnectionString = string.Format("Data Source={0};User ={1};Initial Catalog={2};Password={3}; Integrated Security = false;", Server, User, Database, Password);
                            else
                                sqlConnection.ConnectionString = string.Format("Data Source={0};Initial Catalog={1}; Integrated Security = True;", Server, Database);
                            sqlConnection.Open();
                            SqlCommand sqlCommand = sqlConnection.CreateCommand();
                            sqlCommand.CommandText = string.Format("CREATE TABLE {0}.dbo.{1} (ID INT IDENTITY," +
                                                                  "Name VARCHAR(MAX) NULL," +
                                                                  "WorkFolder VARCHAR(MAX) NULL," +
                                                                  "ReleaseFolder VARCHAR(MAX) NULL," +
                                                                  "ReestrFolder VARCHAR(MAX) NULL," +
                                                                  "PRIMARY KEY CLUSTERED (ID))" +
                                                                  "ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]", Database, Table);
                            sqlCommand.ExecuteNonQuery();
                            sqlConnection.Close();
                            break;
                    }
                    return "OK";
                }
                else
                {
                    return "Настройка произведена не полностью";
                }
            }
            catch(Exception ex)
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
