using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.QueryGenerator
{
    public interface IQueryGenerator
    {
        bool CreateDatabase(string Name, string PathToFile);
        bool CreateTable(string TableName);
        bool CreateTable(string TableName, Dictionary<string, string> Properties);
        bool CreateColumn(string TableName, string ColumnName, ColumnType Type);
        bool CreateRow(string TableName, string Value);
        bool CreateRow(string TableName, object NewRowObject);
        bool RemoveDatabase();
        bool RemoveTable(string TableName);
        bool RemoveColumn(string TableName, string ColumnName);
        bool RemoveRow(string TableName, int RowIndex);
        bool RemoveRow(string TableName, int StartRow, int Count);
        string ReadRow(string TableName, int RowIndex);
        List<string> ReadRows(string TableName, int RowIndexStart, int RowIndexEnd);
        bool UpdateRow(string TableName, int RowIndex, object NewRowObject);
        bool SaveChanges();

    }
}
