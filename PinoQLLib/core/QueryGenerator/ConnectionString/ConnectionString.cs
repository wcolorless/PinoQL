using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib.core.DataBase;
using System.IO;

namespace PinoQLLib.core.QueryGenerator.ConnectionString
{
    public class ConnectionString : IConnectionString
    {
        private string _connectionString;

        public IDataBase CurrentDataBase { get; private set; }

        public ConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            if(File.Exists(_connectionString))
            {
                CurrentDataBase = InitialDataBase.Load(_connectionString);
            }
            else
            {
                string newDBName = Path.GetFileNameWithoutExtension(connectionString);
                CurrentDataBase = InitialDataBase.Get(newDBName);
                CurrentDataBase.Save(connectionString);
            }

        }

        public string GetConnection()
        {
            return _connectionString;
        }

        public static IConnectionString Parse(string PathToFile)
        {
            return new ConnectionString(PathToFile);
        }
    }
}
