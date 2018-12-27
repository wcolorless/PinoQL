using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core
{
    [Serializable]
    public enum ColumnType
    {
        empty,
        Text,
        Int32,
        Int64,
        Logic
        
    }

    [Serializable]
    public enum RestrictionType
    {
        PK,
        NOTNULL,
        UNIQUE,
        AUTOINC
    }

    public interface ITableColumn
    {
        string Name { get; set; }
        ColumnType Type { get; set; }
        List<object> Property { get; set; }
        List<RestrictionType> Restrictions { get; set; }
        bool AddRestriction(RestrictionType restriction);
        bool AddRow(object Cell);
    }
}
