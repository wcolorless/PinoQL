using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.Queries
{
    public enum QueryType
    {
        Empty,
        Create,
        Read,
        Update,
        Delete,
        Set,
        Info
        
    }

    public enum QueryObject
    {
        Empty,
        DataBase,
        Table,
        Column,
        Row 
    }

    public interface IQuery
    {
        QueryType HType { get; set; }
        QueryObject LType { get; set; }
        string DBName { get; set; }
        string TableName { get; set; }
        int RowIndex { get; set; }
        int RowRange { get; set; }
        string ColumnName { get; set; }
        string Parameter { get; set; }
        Dictionary<string, string> Parameters { get; set; }

    }
}
