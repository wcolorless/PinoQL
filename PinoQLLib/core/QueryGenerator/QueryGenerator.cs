using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using PinoQLLib.core.QueryGenerator.ConnectionString;
using PinoQLLib.core.DataBase;
using PinoQLLib.core.DataProviders;
using PinoQLLib.core.Queries;
using PinoQLLib.core.Translator;
using PinoQLLib.core.Table;
using PinoQLLib.core.Environment;

namespace PinoQLLib.core.QueryGenerator
{
    public class QueryGenerator : IQueryGenerator
    {
        IConnectionString ConnectionString;
        IDataBase dataBase;
        IDataProvider dataProvider;
        IQuery CurrentQuery;

        private QueryGenerator(IConnectionString ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            if(ConnectionString.CurrentDataBase == null)
            {
                dataBase = InitialDataBase.Load(ConnectionString);
            }
            else
            {
                dataBase = ConnectionString.CurrentDataBase;
            }
            dataProvider = DataProvider.Create(dataBase);
            WorkEnvironment.SelectedDataBasePath = ConnectionString.GetConnection();
        }


        public static IQueryGenerator Create(IConnectionString ConnectionString)
        {
            return new QueryGenerator(ConnectionString);
        }

        public bool CreateColumn(string TableName, string ColumnName, ColumnType Type)
        {
            ITable table = dataBase.Tables.Find(x => x.Name == TableName);
            if (table != null)
            {
                ITableColumn columns = table.Columns.Find(x => x.Name == ColumnName);
                if(columns == null)
                {
                    table.Columns.Add(new TableColumn(ColumnName, Type));
                }
                return false;
            }
            else return false;
        }

        public bool CreateDatabase(string Name, string PathToFile)
        {
           dataBase = InitialDataBase.Get(Name);
           return dataBase.Save(PathToFile);
        }

        public bool CreateRow(string TableName, string Value)
        {
            string tmpStr = "[" + Value + "]";
            Dictionary<string, string> parameters = CSVQueryConverter.FromCSVToListData(new CSVQueryFinderResult(0, (tmpStr.Length - 1), tmpStr));
            ITable table = dataBase.Tables.Find(x => x.Name == TableName);
            if (table != null)
            {
                var paramsV = parameters.Values.ToArray();
                for (int i = 0; i < parameters.Count; i++)
                {
                    table.Columns[i].AddRow(paramsV[i]);
                }
                if (dataBase.Save(this.ConnectionString.GetConnection())) return true;
                else return false;
            }
            else return false;
        }

        public bool CreateRow(string TableName, object NewRowObject)
        {

            ITable table = dataBase.Tables.Find(x => x.Name == TableName);
            if (table != null)
            {
                TypeInfo info = NewRowObject.GetType().GetTypeInfo();
                var properties = info.GetProperties();

                for(int i = 0; i < properties.Length; i++)
                {
                    object fromProp = properties[i].GetValue(NewRowObject);
                    table.Columns[i].Property.Add(fromProp);
                }
                return true;
            }
            else return false;
        }

        public bool SaveChanges()
        {
            if (dataBase.Save(this.ConnectionString.GetConnection())) return true;
            else return false;
        }

        public bool CreateTable(string TableName)
        {
            CurrentQuery = new Query("Current", QueryType.Create, QueryObject.Table, TableName, 0, "Empty", "Empty");
            IQueryResult result = dataProvider.RunQuery(CurrentQuery);
            if (result.Message == QueryResultMessage.Ok) return true;
            else if (result.Message == QueryResultMessage.Error) return false;
            else return false;
        }

        public bool CreateTable(string TableName, Dictionary<string, string> Properties)
        {
            CurrentQuery = new Query("Current", QueryType.Create, QueryObject.Table, TableName, 0, "Empty", "Empty");
            CurrentQuery.Parameters = Properties;
            IQueryResult result = dataProvider.RunQuery(CurrentQuery);
            if (result.Message == QueryResultMessage.Ok) return true;
            else if (result.Message == QueryResultMessage.Error) return false;
            else return false;
        }

        public string ReadRow(string TableName, int RowIndex)
        {
            CurrentQuery = new Query("Current", QueryType.Read, QueryObject.Row, TableName, RowIndex, "Empty", "Empty");
            IQueryResult result = dataProvider.RunQuery(CurrentQuery);
            if (result.Message == QueryResultMessage.Ok) return result.ResultText;
            else if (result.Message == QueryResultMessage.Error) return "Error";
            else return "Error";
        }

        public List<string> ReadRows(string TableName, int RowIndexStart, int RowIndexEnd)
        {
            List<string> rows = new List<string>();
            for(int i = RowIndexStart; i < RowIndexEnd; i++)
            {
                CurrentQuery = new Query("Current", QueryType.Read, QueryObject.Row, TableName, i, "Empty", "Empty");
                IQueryResult result = dataProvider.RunQuery(CurrentQuery);
                if (result.Message == QueryResultMessage.Ok) rows.Add(result.ResultText);
            }
            return rows;
        }

        public bool RemoveColumn(string TableName, string ColumnName)
        {
            CurrentQuery = new Query("Current", QueryType.Delete, QueryObject.Column, TableName, 0, ColumnName, "Empty");
            IQueryResult result = dataProvider.RunQuery(CurrentQuery);
            if (result.Message == QueryResultMessage.Ok) return true;
            else if (result.Message == QueryResultMessage.Error) return false;
            else return false;
        }

        public bool RemoveDatabase()
        {
            string DBPath = ConnectionString.GetConnection();
            if (DBPath != null)
            {
                if (File.Exists(DBPath))
                {
                    File.Delete(DBPath);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public bool RemoveRow(string TableName, int RowIndex)
        {
            CurrentQuery = new Query("Current", QueryType.Delete, QueryObject.Row, TableName, RowIndex, "Empty", "Empty");
            IQueryResult result = dataProvider.RunQuery(CurrentQuery);
            if (result.Message == QueryResultMessage.Ok) return true;
            else if (result.Message == QueryResultMessage.Error) return false;
            else return false;
        }

        public bool RemoveRow(string TableName, int StartRow, int Count)
        {
            CurrentQuery = new Query("Current", QueryType.Delete, QueryObject.Row, TableName, StartRow, "Empty", "Empty", Count);
            IQueryResult result = dataProvider.RunQuery(CurrentQuery);
            if (result.Message == QueryResultMessage.Ok) return true;
            else if (result.Message == QueryResultMessage.Error) return false;
            else return false;
        }

        public bool RemoveTable(string TableName)
        {
            CurrentQuery = new Query("Current", QueryType.Delete, QueryObject.Table, TableName, 0, "Empty", TableName);
            IQueryResult result = dataProvider.RunQuery(CurrentQuery);
            if (result.Message == QueryResultMessage.Ok) return true;
            else if (result.Message == QueryResultMessage.Error) return false;
            else return false;
        }

        public bool UpdateRow(string TableName, int RowIndex, object NewRowObject)
        {
            ITable table = dataBase.Tables.Find(x => x.Name == TableName);
            if (table != null)
            {
                TypeInfo info = NewRowObject.GetType().GetTypeInfo();
                var properties = info.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    object fromProp = properties[i].GetValue(NewRowObject);
                    table.Columns[i].Property[RowIndex] = fromProp;
                }
                return true;
            }
            else return false;
        }
    }
}
