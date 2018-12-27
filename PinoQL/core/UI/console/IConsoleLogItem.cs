using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib.core.DataProviders;

namespace PinoQL
{
    public interface IConsoleLogItem
    {
        DateTime Time { get; }
        string CommandText { get;}
        IQueryResult Result { get; }
    }
}
