using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.Translator
{
    public struct JSONQueryFinderResult
    {
       public int StartIndex { get;}
       public int EndIndex { get; }
       public string InitialQuery { get; }

        public JSONQueryFinderResult(int StartIndex, int EndIndex, string InitialQuery)
        {
            this.StartIndex = StartIndex;
            this.EndIndex = EndIndex;
            this.InitialQuery = InitialQuery;
        }
    }

    public class JSONQueryFinder
    {

        public static JSONQueryFinderResult Find(string Query)
        {
            int StartIndex = -1;
            int EndIndex = -1;
            var QueryBuff = Query;
            JSONQueryFinderResult Result;

            for(int i = 0; i < QueryBuff.Length; i++)
            {
                if(QueryBuff[i] == '{')
                {
                    StartIndex = i;
                }
                if(StartIndex != -1)
                {
                    if (QueryBuff[i] == '}')
                    {
                        EndIndex = i;
                    }
                }
                if (StartIndex != -1 && EndIndex != -1) break;
            }
            if (StartIndex != -1 && EndIndex != -1) Result = new JSONQueryFinderResult(StartIndex, EndIndex, Query);
            else Result = new JSONQueryFinderResult(-1, -1, "Error");
            return Result;
        }


    }
}
