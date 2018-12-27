using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib.core.Queries;

namespace PinoQLLib.core.Translator
{
    public interface ITranslator
    {
        IQuery GetQuery(string Command);
    }
}
