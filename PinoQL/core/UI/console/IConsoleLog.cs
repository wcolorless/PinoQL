using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQL
{
    public interface IConsoleLog
    {
        IConsoleLogItem GetPrev();
        IConsoleLogItem GetNext();
        void AddItem(IConsoleLogItem Item);
    }
}
