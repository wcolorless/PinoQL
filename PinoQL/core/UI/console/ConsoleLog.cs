using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib.core.DataProviders;

namespace PinoQL
{
    public class ConsoleLog : IConsoleLog
    {
        List<IConsoleLogItem> Items;
        int index = 0;

        private ConsoleLog()
        {
            Items = new List<IConsoleLogItem>();
        }


        public static IConsoleLog Create()
        {
            return new ConsoleLog();
        }
        public IConsoleLogItem GetNext()
        {
            var count = Items.Count;
            if (count > 0)
            {
                if (index < (count - 1)) index += 1;
                else index = 0;
                var Item = Items[index];
                return Item;
            }
            else return ConsoleLogItem.Create("", null, DateTime.Now);
        }

        public IConsoleLogItem GetPrev()
        {
            var count = Items.Count;
            if (count > 0)
            {
                if (index > 0) index -= 1;
                else index = count - 1;
                var Item = Items[index];
                return Item;
            }
            else return ConsoleLogItem.Create("", null, DateTime.Now);
        }

        public void AddItem(IConsoleLogItem Item)
        {
            Items.Add(Item);

        }
    }
}
