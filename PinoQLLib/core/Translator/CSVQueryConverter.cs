using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.Translator
{
    public class CSVQueryConverter
    {
        public static Dictionary<string, string> FromCSVToListData(CSVQueryFinderResult CSV)
        {
            Dictionary<string, string> Properties = new Dictionary<string, string>();
            var CSVStrings = CSV.InitialQuery.Substring(CSV.StartIndex, (CSV.EndIndex - (CSV.StartIndex - 1)));
            CSVStrings = CSVStrings.Replace(" ", "");
            CSVStrings = CSVStrings.Replace("[", "");
            CSVStrings = CSVStrings.Replace("]", "");
            var result = CSVStrings.Split(new char[]{','});
            for (int i = 0; i < result.Length; i++) Properties.Add(i.ToString(), result[i]);
            return Properties;
        }
    }
}
