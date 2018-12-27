using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.Queries
{
    public struct Query : IQuery
    {
        public string DBName { get;  set; }

        public QueryType HType { get;  set; }

        public QueryObject LType { get;  set; }

        public string TableName { get;  set; }

        public int RowIndex { get; set; }

        public string ColumnName { get; set; }

        public string Parameter { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public int RowRange { get; set; }

        public Query(string DBName, QueryType HType, QueryObject LType, string TableName, int RowIndex, string Column, string Parameter, int RowRange = 0)
        {
            this.DBName = DBName;
            this.HType = HType;
            this.LType = LType;
            this.TableName = TableName;
            this.RowIndex = RowIndex;
            this.ColumnName = Column;
            this.Parameter = Parameter;
            this.Parameters = new Dictionary<string, string>();
            this.RowRange = RowRange;
        }

    }
}
