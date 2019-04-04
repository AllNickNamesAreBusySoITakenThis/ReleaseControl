using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                            Apps.Add(ControlledApp.AddApp(oleDbDataReader["Name"].ToString(), oleDbDataReader["WorkFolder"].ToString(), oleDbDataReader["ReleaseFolder"].ToString()));
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
                            Apps.Add(ControlledApp.AddApp(mySqlDataReader["Name"].ToString(), mySqlDataReader["WorkFolder"].ToString(), mySqlDataReader["ReleaseFolder"].ToString()));
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
                            Apps.Add(ControlledApp.AddApp(sqlDataReader["Name"].ToString(), sqlDataReader["WorkFolder"].ToString(), sqlDataReader["ReleaseFolder"].ToString()));
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

        public static string Save()
        {
            switch(StorageType)
            {
                case StorageTypes.Database:
                    break;
                case StorageTypes.XML:
                    break;
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
