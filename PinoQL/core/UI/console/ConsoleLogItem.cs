using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib.core.DataProviders;

namespace PinoQL
{
    public class ConsoleLogItem : IConsoleLogItem
    {
        public string CommandText { get; private set; }

        public IQueryResult Result { get; private set; }

        public DateTime Time { get; private set; }


        private ConsoleLogItem(string CommandText, IQueryResult Result, DateTime Time)
        {
            this.CommandText = CommandText;
            this.Result = Result;
            this.Time = Time;
        }

        public static IConsoleLogItem Create(string CommandText, IQueryResult Result, DateTime Time)
        {
            return new ConsoleLogItem(CommandText, Result, Time);
        }

    }
}
