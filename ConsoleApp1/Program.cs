using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReleaseControlLib;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            StorageService.ConnectionType = ConnectionTypes.OracleMySql;
            StorageService.Database = "workdatabase";
            StorageService.Server = "192.0.0.165";
            StorageService.User = "root";
            StorageService.Password = "Bzpa/123456789";
            StorageService.Port = "3306";
            StorageService.Table = "controlapp";

            Console.WriteLine(StorageService.GetDataFromDb());

            var a = StorageService.Apps;
            Console.Read();
        }
    }
}
