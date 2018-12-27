using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.Translator
{
    public class SingleQueryConverter
    {
        public static Dictionary<string, string> FromTextToListData(CSVQueryFinderResult CSV)
        {
            Dictionary<string, string> Properties = new Dictionary<string, string>();
            var CSVStrings = CSV.InitialQuery.Substring(CSV.StartIndex, (CSV.EndIndex - (CSV.StartIndex - 1)));
            CSVStrings = CSVStrings.Replace(" ", "");
            CSVStrings = CSVStrings.Replace("[", "");
            CSVStrings = CSVStrings.Replace("]", "");
            var result = CSVStrings.Split(new char[] { ',' });
            Properties.Add(result[0], result[1]);
            return Properties;
        }
    }
}
