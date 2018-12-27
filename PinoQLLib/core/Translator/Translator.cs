using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib.core.Queries;

namespace PinoQLLib.core.Translator
{
    public enum CommandType
    {
        Empty,
        FollowNextCommand,
        FollowParameter,
        Parameter,
        Initial,
        End
    }

    public struct CommandProperty
    {
      public  CommandType Type { get; set; }

    }


    

    public class Translator : ITranslator
    {
        private Dictionary<string, CommandProperty> Layer1Commands = new Dictionary <string, CommandProperty>(){ { "SET", new CommandProperty() { Type = CommandType.FollowNextCommand } }, { "CREATE", new CommandProperty() { Type = CommandType.FollowNextCommand } }, { "READ", new CommandProperty() { Type = CommandType.FollowNextCommand } }, { "UPDATE", new CommandProperty() { Type = CommandType.FollowNextCommand } }, { "DELETE", new CommandProperty() { Type = CommandType.FollowNextCommand } } };
        private Dictionary<string, CommandProperty> Layer2Commands = new Dictionary<string, CommandProperty>() { { "DATABASE", new CommandProperty() { Type = CommandType.FollowParameter } }, { "TABLE", new CommandProperty() { Type = CommandType.FollowParameter } }, { "COLUMN", new CommandProperty() { Type = CommandType.FollowParameter } }, { "ROW", new CommandProperty() { Type = CommandType.FollowParameter } } };
        private Dictionary<string, CommandProperty> Layer3Commands = new Dictionary<string, CommandProperty>() { { "INFO", new CommandProperty() { Type = CommandType.End } } };
        private IElementOfChainAction FirstAction;
        private IElementOfChainAction LastAction;

        private void JoinChain(int CurrentIndex, string InitialQuery, string[] words, string word, IElementOfChainAction Action)
        {
            IElementOfChainAction NewNextAction = new ElementOfChainAction(null);
            CommandProperty Property;
            word = word.ToUpper();
            var NextIndex = CurrentIndex + 1;
            if (Layer1Commands.ContainsKey(word))
            {
                Layer1Commands.TryGetValue(word, out  Property);
                Action.CommandWord = word;
                Action.Type = Property.Type;
                Action.NextAction = NewNextAction;
                FirstAction = Action;
            }
            else // Проверяем на более низком уровне команд
            {
                if (Layer2Commands.ContainsKey(word))
                {
                    Layer2Commands.TryGetValue(word, out Property);
                    Action.CommandWord = word;
                    Action.Type = Property.Type;
                    if (NextIndex < words.Length) Action.NextAction = NewNextAction;
                    else Action.NextAction = null;
                }
                else
                {
                    if(Layer3Commands.ContainsKey(word))
                    {
                        Layer3Commands.TryGetValue(word, out Property);
                        Action.CommandWord = word;
                        Action.Type = Property.Type;
                        if (NextIndex < words.Length) Action.NextAction = NewNextAction;
                        else Action.NextAction = null;
                    }
                    else
                    {
                        // Можно внедрить ограничения имён
                        var CommandProrepty = new CommandProperty() { Type = CommandType.Parameter };
                        Action.CommandWord = word;
                        Action.Type = CommandProrepty.Type;
                        if (NextIndex < words.Length) Action.NextAction = NewNextAction;
                        else Action.NextAction = null;
                        if(LastAction != null)
                        {
                            if (LastAction.Type == CommandType.FollowParameter && LastAction.CommandWord == "TABLE") // Поиск таблицы в запросе
                            {
                                JSONQueryFinderResult Result = JSONQueryFinder.Find(InitialQuery);
                                Dictionary<string, string> Parameters = JSONQueryConverter.FromJSONToListOfParameter(Result);
                                Action.Parameters = Parameters;
                                return;
                            }
                            else if(LastAction.Type == CommandType.FollowParameter && LastAction.CommandWord == "COLUMN" && FirstAction.CommandWord == "CREATE")
                            {
                                CSVQueryFinderResult Result = CSVQueryFinder.Find(InitialQuery);
                                Dictionary<string, string> Parameters = SingleQueryConverter.FromTextToListData(Result);
                                Action.Parameters = Parameters;
                                return;
                            }
                            else if(LastAction.Type == CommandType.FollowParameter && LastAction.CommandWord == "COLUMN" && FirstAction.CommandWord == "DELETE")
                            {
                                return;

                            }
                            else if(LastAction.Type == CommandType.FollowParameter && LastAction.CommandWord == "ROW" && FirstAction.CommandWord == "CREATE") // Поиск новой строки в запросе
                            {
                                CSVQueryFinderResult Result = CSVQueryFinder.Find(InitialQuery);
                                Dictionary<string, string> Parameters = CSVQueryConverter.FromCSVToListData(Result);
                                Action.Parameters = Parameters;
                                return;
                            }
                            else if (LastAction.Type == CommandType.FollowParameter && LastAction.CommandWord == "ROW" && FirstAction.CommandWord == "DELETE") // Поиск новой строки в запросе
                            {
                               
                                return;
                            }
                        }
                    }
                }
            }
           LastAction = Action;
           if (NextIndex < words.Length) JoinChain(NextIndex, InitialQuery, words, words[NextIndex], NewNextAction);
        }

        private IQuery ConvertToQuery(IElementOfChainAction Action, IQuery newQuery)
        {
            switch(Action.Type)
            {
                case CommandType.Initial:
                    if (Action.NextAction != null) ConvertToQuery(Action.NextAction, newQuery);
                    break;
                case CommandType.FollowNextCommand:
                    QueryType CurrentType;
                        switch(Action.CommandWord)
                        {
                            case "SET":
                                CurrentType = QueryType.Set;
                                break;
                            case "CREATE":
                                CurrentType = QueryType.Create;
                                break;
                            case "READ":
                                CurrentType = QueryType.Read;
                                break;
                            case "UPDATE":
                                CurrentType = QueryType.Update;
                                break;
                            case "DELETE":
                                CurrentType = QueryType.Delete;
                                break;
                        default:
                            CurrentType = QueryType.Empty;
                            break;
                        }
                    newQuery.HType = CurrentType;
                    break;
                case CommandType.FollowParameter:
                    QueryObject CurrentParameterType;
                    switch (Action.CommandWord)
                    {
                        case "COLUMN":
                            CurrentParameterType = QueryObject.Column;
                            break;
                        case "DATABASE":
                            CurrentParameterType = QueryObject.DataBase;
                            break;
                        case "ROW":
                            CurrentParameterType = QueryObject.Row;
                            break;
                        case "TABLE":
                            CurrentParameterType = QueryObject.Table;
                            break;
                        default:
                            CurrentParameterType = QueryObject.Empty;
                            break;
                    }
                    newQuery.LType = CurrentParameterType;
                    break;
                case CommandType.End:
                    switch(Action.CommandWord)
                    {
                        case "INFO":
                            newQuery.HType = QueryType.Info;
                            break;
                        case "SET":
                            newQuery.HType = QueryType.Set;
                            break;
                        default: break;
                    }
                    break;
                case CommandType.Parameter:
                    // Конвертация числа или запись строки
                    if(newQuery.HType == QueryType.Info)
                    {
                        newQuery.Parameter = Action.CommandWord;
                        break;
                    }
                    else if(newQuery.HType == QueryType.Set)
                    {
                        newQuery.Parameter = Action.CommandWord;
                        break;
                    }
                    else if(newQuery.HType == QueryType.Delete)
                    {
                        if (newQuery.LType == QueryObject.Row) newQuery.RowIndex = Convert.ToInt32(Action.CommandWord);
                        else if (newQuery.LType == QueryObject.Column) newQuery.ColumnName = Action.CommandWord;
                    }

                    if (newQuery.LType == QueryObject.DataBase)
                    {
                        newQuery.DBName = Action.CommandWord;
                    }
                    else if (newQuery.LType == QueryObject.Table)
                    {
                        newQuery.TableName = Action.CommandWord;
                        if (Action.Parameters != null)
                        {
                            newQuery.Parameters = Action.Parameters;
                        }
                            
                    }
                    else if (newQuery.LType == QueryObject.Column)
                    {
                        var symbols = Action.CommandWord.ToArray();
                        if(symbols.Contains(',') || symbols.Contains('[') || symbols.Contains(']'))
                        {
                            newQuery.ColumnName = null;
                        }
                        else newQuery.ColumnName = Action.CommandWord;
                        newQuery.Parameters = Action.Parameters;
                    }
                    else if (newQuery.LType == QueryObject.Row)
                    {
                        //newQuery.RowName = Action.CommandWord;
                        newQuery.Parameters = Action.Parameters;
                    }
                    else
                    {
                       
                    }
                    break;
                default: break;
            }
            if (Action.NextAction != null) ConvertToQuery(Action.NextAction, newQuery);
            return newQuery;
        }

        public IQuery GetQuery(string Command)
        {
            IElementOfChainAction InitialAction = new ElementOfChainAction(null);
            InitialAction.Type = CommandType.Initial;
            InitialAction.CommandWord = "init";
            InitialAction.NextAction = null;
            string[] words = Command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            JoinChain(0, Command, words, words[0], InitialAction);
            IQuery newQuery = new Queries.Query();
            newQuery = ConvertToQuery(InitialAction, newQuery);
            return newQuery;
        }
    }
}
