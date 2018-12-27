using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib.core.DataBase;

namespace PinoQLLib.core.QueryGenerator.ConnectionString
{
    public interface IConnectionString
    {
        string GetConnection();
        IDataBase CurrentDataBase { get; }
    }
}
