using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib.core.DataBase;
using PinoQLLib.core.Queries;
using PinoQLLib.core.Environment;
using PinoQLLib.core.Table;
using System.IO;

namespace PinoQLLib.core.DataProviders
{
    public class DataProvider : IDataProvider
    {
        private IDataBase DataBase;

        private DataProvider(IDataBase DataBase)
        {
            this.DataBase = DataBase;
        }

        public static IDataProvider Create(IDataBase DataBase)
        {
            return new DataProvider(DataBase);
        }

        public void SelectDataBase(IDataBase DataBase)
        {
            this.DataBase = DataBase;
        }


        public IQueryResult RunQuery(IQuery Query)
        {
            IQueryResult Result = null;
            switch (Query.HType)
            {
                case QueryType.Set:
                    switch(Query.LType)
                    {
                        case QueryObject.DataBase:
                            WorkEnvironment.SelectedDataBaseName = Query.Parameter;
                            var PathToDb = WorkEnvironment.GetPathToDB(WorkEnvironment.SelectedDataBaseName);
                            DataBase = InitialDataBase.Load(PathToDb);
                            Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Ok, WorkEnvironment.GetAllRows());
                            break;
                        case QueryObject.Table:
                            WorkEnvironment.SelectedTableName = Query.Parameter;
                            Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Ok, WorkEnvironment.GetAllRows());
                            break;
                        default: break;
                    }
                    break;
                case QueryType.Create:
                    switch(Query.LType)
                    {
                        case QueryObject.Column:
                            var CreateColumnTableName = string.IsNullOrEmpty(Query.TableName) ? WorkEnvironment.SelectedTableName : Query.TableName;
                            var CreateColumnTableHeader = Query.Parameters;
                            ITable CreateColumnUsingTable = DataBase.Tables.Find(x => x.Name == CreateColumnTableName);
                            if (CreateColumnTableHeader != null)
                            {
                                var Headers = CreateColumnTableHeader.ToList();
                                for (int i = 0; i < CreateColumnTableHeader.Count; i++)
                                {
                                    ITableColumn newColumnForTable;
                                    if (Headers[i].Value.ToUpper() == "STRING" || Headers[i].Value.ToUpper() == "TEXT")
                                    {
                                        newColumnForTable = new TableColumn(Headers[i].Key.ToUpper(), ColumnType.Text);
                                        if(CreateColumnUsingTable.Columns.Count > 0)
                                        {
                                            for (int s = 0; s < CreateColumnUsingTable.Columns[0].Property.Count; s++)
                                            {
                                                newColumnForTable.Property.Add("");
                                            }
                                        }
                                        CreateColumnUsingTable.Columns.Add(newColumnForTable);
                                    }
                                    else if (Headers[i].Value.ToUpper() == "INT" || Headers[i].Value.ToUpper() == "INT32")
                                    {
                                        newColumnForTable = new TableColumn(Headers[i].Key.ToUpper(), ColumnType.Int32);
                                        if (CreateColumnUsingTable.Columns.Count > 0)
                                        {
                                            for (int s = 0; s < CreateColumnUsingTable.Columns[0].Property.Count; s++)
                                            {
                                                newColumnForTable.Property.Add(null);
                                            }
                                        }
                                        CreateColumnUsingTable.Columns.Add(newColumnForTable);
                                    }
                                    else if (Headers[i].Value.ToUpper() == "LONG" || Headers[i].Value.ToUpper() == "INT64")
                                    {
                                        newColumnForTable = new TableColumn(Headers[i].Key.ToUpper(), ColumnType.Int64);
                                        if (CreateColumnUsingTable.Columns.Count > 0)
                                        {
                                            for (int s = 0; s < CreateColumnUsingTable.Columns[0].Property.Count; s++)
                                            {
                                                newColumnForTable.Property.Add(null);
                                            }
                                        }
                                        CreateColumnUsingTable.Columns.Add(newColumnForTable);
                                    }
                                    else if (Headers[i].Value.ToUpper() == "BOOL" || Headers[i].Value.ToUpper() == "BOOLEAN")
                                    {
                                        newColumnForTable = new TableColumn(Headers[i].Key.ToUpper(), ColumnType.Logic);
                                        if (CreateColumnUsingTable.Columns.Count > 0)
                                        {
                                            for (int s = 0; s < CreateColumnUsingTable.Columns[0].Property.Count; s++)
                                            {
                                                newColumnForTable.Property.Add(null);
                                            }
                                        }
                                        CreateColumnUsingTable.Columns.Add(newColumnForTable);
                                    }
                                    else
                                    {
                                        newColumnForTable = (new TableColumn(Headers[i].Key, ColumnType.empty));
                                        if (CreateColumnUsingTable.Columns.Count > 0)
                                        {
                                            for (int s = 0; s < CreateColumnUsingTable.Columns[0].Property.Count; s++)
                                            {
                                                newColumnForTable.Property.Add(null);
                                            }
                                        }
                                        CreateColumnUsingTable.Columns.Add(newColumnForTable);
                                    }
                                    WorkEnvironment.SelectedTableName = CreateColumnTableName;
                                }
                            }
                            if (DataBase.Save(WorkEnvironment.SelectedDataBasePath))     Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Ok);
                            else     Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            if (WorkEnvironment.AddNewColumnHandler != null) WorkEnvironment.AddNewColumnHandler();
                            break;
                        case QueryObject.DataBase:
                            var Names = WorkEnvironment.GetAllDataBaseNames();
                            if(!Names.Contains(Query.DBName))
                            {
                                IDataBase newDataBase = InitialDataBase.Get(Query.DBName);
                                string SavePath = WorkEnvironment.LocalWorkPath + "\\" + Query.DBName + ".pdbf";
                                WorkEnvironment.LastCreatedDataBase = SavePath;
                                newDataBase.Save(SavePath);
                                WorkEnvironment.SelectedDataBaseName = Query.DBName;
                                DataBase = InitialDataBase.Load(SavePath);
                                Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Ok, WorkEnvironment.GetAllRows());
                                WorkEnvironment.AddNewDBHandler(SavePath);
                            }
                            else   Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error, WorkEnvironment.GetAllRows());
                            break;
                        case QueryObject.Row:
                            if(Query.Parameters.Count > 0)
                            {
                                if(!string.IsNullOrEmpty(WorkEnvironment.SelectedTableName))
                                {
                                    ITable table = DataBase.Tables.Find(x => x.Name == WorkEnvironment.SelectedTableName);
                                    for (int i = 0; i < Query.Parameters.Count; i++)
                                    {
                                        table.Columns[i].AddRow(Query.Parameters[i.ToString()]);
                                    }
                                    if (DataBase.Save(WorkEnvironment.GetPathToDB(DataBase.Name)))  Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Ok);
                                    else  Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                                }
                                else Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);

                            }
                            break;
                        case QueryObject.Table:
                            if(DataBase == null)
                            {
                                Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                                break;
                            }
                            var TableName = Query.TableName;
                            var TableHeader = Query.Parameters;
                            ITable NewTable = new PinoQLLib.core.Table.Table();
                            NewTable.Name = TableName;
                            DataBase.Tables.Add(NewTable);
                            if (TableHeader != null)
                            {
                                var Headers = TableHeader.ToList();
                                for (int i = 0; i < TableHeader.Count; i++)
                                {
                                    if (Headers[i].Value.ToUpper() == "STRING" || Headers[i].Value.ToUpper() == "TEXT")
                                    {
                                        NewTable.Columns.Add(new TableColumn(Headers[i].Key, ColumnType.Text));
                                    }
                                    else if (Headers[i].Value.ToUpper() == "INT" || Headers[i].Value.ToUpper() == "INT32")
                                    {
                                        NewTable.Columns.Add(new TableColumn(Headers[i].Key, ColumnType.Int32));
                                    }
                                    else if (Headers[i].Value.ToUpper() == "LOGN" || Headers[i].Value.ToUpper() == "INT64")
                                    {
                                        NewTable.Columns.Add(new TableColumn(Headers[i].Key, ColumnType.Int64));
                                    }
                                    else if (Headers[i].Value.ToUpper() == "BOOL" || Headers[i].Value.ToUpper() == "BOOLEAN")
                                    {
                                        NewTable.Columns.Add(new TableColumn(Headers[i].Key, ColumnType.Logic));
                                    }
                                    else
                                    {
                                        NewTable.Columns.Add((new TableColumn(Headers[i].Key, ColumnType.empty)));
                                    }
                                    WorkEnvironment.SelectedTableName = NewTable.Name;
                                }
                            }
                            if (DataBase.Save(WorkEnvironment.SelectedDataBasePath))   Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Ok);
                            else    Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            if(WorkEnvironment.AddNewTableHandler != null) WorkEnvironment.AddNewTableHandler();
                            break;
                        default: break;
                    }
                    break;
                case QueryType.Delete:
                    switch (Query.LType)
                    {
                        case QueryObject.Column:
                            if(DataBase.RemoveColumn((string.IsNullOrEmpty(Query.TableName) ? WorkEnvironment.SelectedTableName : Query.TableName), Query.ColumnName))
                            {
                                if (DataBase.Save()) Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Ok);
                                else Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            }
                            else Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            break;
                        case QueryObject.DataBase:
                            var RemoveFilePath = WorkEnvironment.GetPathToDB(Query.DBName);
                            if(DataBase.Remove(RemoveFilePath))
                            {
                                Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Ok);
                                WorkEnvironment.RemoveDBHandler();
                            }
                            else Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            break;
                        case QueryObject.Row:
                            string DeleteRowTableName = (string.IsNullOrEmpty(Query.TableName) ? WorkEnvironment.SelectedTableName : Query.TableName);
                            bool DeleteRowResult = false;
                            if (Query.RowRange > 0) DeleteRowResult = DataBase.RemoveRow(DeleteRowTableName, Query.RowIndex, Query.RowRange);
                            else DeleteRowResult = DataBase.RemoveRow(DeleteRowTableName, Query.RowIndex);
                            if (DeleteRowResult)
                            {
                                if (DataBase.Save()) Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Ok);
                                else Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            }
                            else Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            break;
                        case QueryObject.Table:
                            if(!string.IsNullOrEmpty(Query.Parameter))
                            {
                               if (DataBase.RemoveTable(Query.Parameter)) Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Ok);
                               else Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            }
                            else Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            break;
                        default: break;
                    }
                    break;
                case QueryType.Read:
                    switch(Query.LType)
                    {
                        case QueryObject.Column:
                            break;
                        case QueryObject.DataBase:
                            break;
                        case QueryObject.Row:
                            ITable tableReadRow = DataBase.Tables.Find(x => x.Name == Query.TableName);
                            if (tableReadRow != null)
                            {
                                if (Query.RowIndex > -1 && Query.RowIndex < tableReadRow.Columns[0].Property.Count)
                                {
                                    StringBuilder sbRowResult = new StringBuilder();
                                    for (int i = 0; i < tableReadRow.Columns.Count; i++)
                                    {
                                        sbRowResult.Append(tableReadRow.Columns[i].Property[Query.RowIndex] + (i == (tableReadRow.Columns.Count - 1) ? "" : ","));
                                    }
                                    Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Ok, sbRowResult.ToString());
                                }
                                else Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            }
                            else Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            break;
                        case QueryObject.Table:
                            break;
                        default: break;
                    }
                    break;
                case QueryType.Update:
                    switch(Query.LType)
                    {
                        case QueryObject.Column:
                            break;
                        case QueryObject.DataBase:
                            break;
                        case QueryObject.Row:
                            ITable tableUpdateRow = DataBase.Tables.Find(x => x.Name == Query.TableName);
                            if(tableUpdateRow != null)
                            {

                            }
                            else  Result = new RowQueryResult(DataBase, QueryResultType.Work, QueryResultMessage.Error);
                            break;
                        case QueryObject.Table:
                            break;
                        default: break;
                    }
                    break;
                case QueryType.Info:
                    switch(Query.Parameter)
                    {
                        case "ENVR":
                            Result = new RowQueryResult(DataBase, QueryResultType.Info, QueryResultMessage.Ok, WorkEnvironment.GetAllRows());
                            break;
                        case "TABLE":
                            break;
                        default: break;
                    }
                    break;
                default: break;
            }
            return Result;
        }
    }
}
