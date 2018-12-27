using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.Translator
{
    public class JSONQueryConverter
    {
        public static Dictionary<string, string> FromJSONToListOfParameter(JSONQueryFinderResult JSON)
        {
            if(JSON.InitialQuery != "Error")
            {
                Dictionary<string, string> TableHeader = new Dictionary<string, string>();
                string workString = JSON.InitialQuery.Substring(JSON.StartIndex, (JSON.EndIndex - (JSON.StartIndex - 1)));
                int ColumnNameStartIndex = -1;
                int ColumnNameEndIndex = -1;
                int ColumnTypeStartIndex = -1;
                int ColumnTypeEndIndex = -1;
                string ColumnName = "";
                string ColumnType = "";
                for(int i = 0; i < workString.Length; i++)
                {
                    if(ColumnNameStartIndex < 0 || ColumnNameEndIndex < 0) // Поиск имени параметра
                    {
                        if (ColumnNameStartIndex == -1)
                        {
                            if (workString[i] == '"')
                            {
                                ColumnNameStartIndex = i + 1;
                            }
                        }
                        else
                        {
                            if (ColumnNameEndIndex == -1)
                            {
                                if (workString[i] == '"')
                                {
                                    if (i > 0)
                                    {
                                        ColumnNameEndIndex = i - 1;
                                        if (workString[ColumnNameEndIndex + 2] != ':') break;
                                    }
                                }
                            }
                        }
                    }
                    else // Поиск имени типа параметра
                    {
                        if(ColumnTypeStartIndex < 0 || ColumnTypeEndIndex < 0)
                        {
                            if (ColumnTypeStartIndex == -1)
                            {
                                if (workString[i] == '"')
                                {
                                    ColumnTypeStartIndex = i + 1;
                                }
                            }
                            else
                            {
                                if (ColumnTypeEndIndex == -1)
                                {
                                    if (workString[i] == '"')
                                    {
                                        if (i > 0)
                                        {
                                            ColumnTypeEndIndex = i - 1;
                                        }
                                    }
                                }
                            }
                        }
                        if ((ColumnNameStartIndex > 0 && ColumnNameEndIndex > 0) && (ColumnTypeStartIndex > 0 && ColumnTypeEndIndex > 0))
                        {
                            ColumnName = workString.Substring(ColumnNameStartIndex, ColumnNameEndIndex - (ColumnNameStartIndex - 1));
                            ColumnType = workString.Substring(ColumnTypeStartIndex, ColumnTypeEndIndex - (ColumnTypeStartIndex - 1)); 
                            try
                            {
                                TableHeader.Add(ColumnName, ColumnType);
                            }
                            catch(Exception e)
                            {
                                throw new InvalidOperationException(e.Message);
                            }
                            ColumnNameStartIndex = -1;
                            ColumnNameEndIndex = -1;
                            ColumnTypeStartIndex = -1;
                            ColumnTypeEndIndex = -1;
                            ColumnName = "";
                            ColumnType = "";
                        }
                    } 
                }
                return TableHeader;
            }
            else
            {
                return null;
            }
        }
    }
}
