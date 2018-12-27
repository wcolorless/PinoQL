using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib.core.DataBase;

namespace PinoQLLib.core.DataProviders
{
    public class RowQueryResult : IQueryResult
    {
        private  Dictionary<string, object> Properties;

        public RowQueryResult(IDataBase CurrentDataBase, QueryResultType Type, QueryResultMessage Message, string ResultText = "Empty")
        {
            Properties = new Dictionary<string, object>();
            this.Type = Type;
            this.Message = Message;
            this.ResultText = ResultText;
            this.CurrentDataBase = CurrentDataBase;
        }

        public IDataBase CurrentDataBase { get; }

        public QueryResultMessage Message { get; }

        public string ResultText { get; private set; }

        public QueryResultType Type { get; private set; }

        public bool AddProperty(string Name, object value)
        {
            if (Properties.ContainsKey(Name)) return false;
            else
            {
                Properties.Add(Name, value);
                return true;
            }
            
        }

    }
}
