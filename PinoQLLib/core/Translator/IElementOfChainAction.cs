using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.Translator
{
    public interface IElementOfChainAction
    {
        IElementOfChainAction NextAction { get; set; }
        CommandType Type { get; set; }
        string CommandWord { get; set; }

        Dictionary<string, string> Parameters { get; set;}

    }
}
