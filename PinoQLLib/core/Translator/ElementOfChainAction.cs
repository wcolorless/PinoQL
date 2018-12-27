using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.Translator
{
    public class ElementOfChainAction : IElementOfChainAction
    {
        public IElementOfChainAction NextAction { get; set; }

        public CommandType Type { get; set; }

        public string CommandWord { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public ElementOfChainAction(IElementOfChainAction NextAction)
        {
            this.NextAction = NextAction;
        }
    }
}
