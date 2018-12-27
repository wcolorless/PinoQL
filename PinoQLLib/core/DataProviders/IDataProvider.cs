using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib.core.DataBase;
using PinoQLLib.core.Queries;

namespace PinoQLLib.core.DataProviders
{
    public interface IDataProvider
    {
        IQueryResult RunQuery(IQuery Query);
        void SelectDataBase(IDataBase DataBase);

    }
}
