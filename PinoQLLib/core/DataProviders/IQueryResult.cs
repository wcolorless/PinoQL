using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib.core.DataBase;

namespace PinoQLLib.core.DataProviders
{
    public enum QueryResultType
    {
        Empty,
        Data,
        Info,
        Work
    }

    public enum QueryResultMessage
    {
        Empty,
        Ok,
        Error
    }


    public interface IQueryResult
    {
        IDataBase CurrentDataBase { get; }
        QueryResultType Type { get; }
        QueryResultMessage Message { get; }
        string ResultText { get; }
        bool AddProperty(string Name, object value);

    }
}
