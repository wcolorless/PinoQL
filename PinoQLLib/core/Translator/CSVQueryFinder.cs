using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.Translator
{

    public struct CSVQueryFinderResult
    {
        public int StartIndex { get; }
        public int EndIndex { get; }
        public string InitialQuery { get; }

        public CSVQueryFinderResult(int StartIndex, int EndIndex, string InitialQuery)
        {
            this.StartIndex = StartIndex;
            this.EndIndex = EndIndex;
            this.InitialQuery = InitialQuery;
        }
    }

    public class CSVQueryFinder
    {
        public static CSVQueryFinderResult Find(string Query)
        {
            int StartIndex = -1;
            int EndIndex = -1;
            var QueryBuff = Query;
            CSVQueryFinderResult Result;

            for (int i = 0; i < QueryBuff.Length; i++)
            {
                if (QueryBuff[i] == '[')
                {
                    StartIndex = i;
                }
                if (StartIndex != -1)
                {
                    if (QueryBuff[i] == ']')
                    {
                        EndIndex = i;
                    }
                }
                if (StartIndex != -1 && EndIndex != -1) break;
            }
            if (StartIndex != -1 && EndIndex != -1) Result = new CSVQueryFinderResult(StartIndex, EndIndex, Query);
            else Result = new CSVQueryFinderResult(-1, -1, "Error");
            return Result;
        }
    }
}
