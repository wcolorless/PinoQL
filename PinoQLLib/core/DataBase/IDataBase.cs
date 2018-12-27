using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.DataBase
{
    public interface IDataBase
    {
        string Name { get; }
        List<ITable> Tables { get; set; }
        AddTableResult AddTable(ITable Table);
        bool RemoveTable(string Name);
        bool RemoveColumn(string TableName, string ColumnName);
        bool RemoveRow(string TableName, int indexRow);
        bool RemoveRow(string TableName, int StartRow, int Count);
        bool Save(string PathToFile = "");
        bool Remove(string PathToFile);
    }
}
