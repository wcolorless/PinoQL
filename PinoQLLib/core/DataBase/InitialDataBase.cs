using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using PinoQLLib.core.QueryGenerator.ConnectionString;

namespace PinoQLLib.core.DataBase
{
    [Serializable]
    public enum AddTableResult
    {
        empty,
        ok,
        exist,
        error
    }

    [Serializable]
    public class InitialDataBase : IDataBase
    {

        private string lastPathToFileDB;
        private InitialDataBase(string Name)
        {
            this.Name = Name;
            this.Tables = new List<ITable>();
        }

        public static IDataBase Get(string Name)
        {
            IDataBase newDataBase = new InitialDataBase(Name);
            return newDataBase;
        }


        public List<ITable> Tables { get; set; }

        public string Name { get; private set; }

        public AddTableResult AddTable(ITable Table)
        {
            if (Tables != null)
            {
                
                if(Tables.Where(x => x.Name == Table.Name).ToList().Count > 0)
                {
                    return AddTableResult.exist;
                }
                else
                {
                    Tables.Add(Table);
                    return AddTableResult.ok;
                } 
            }
            else return AddTableResult.error;
        }

        public void SetLoadSavePath(string PathToFile)
        {
            this.lastPathToFileDB = PathToFile;
        }

        public static IDataBase Load(string PathToFile)
        {

            var LoadDB = PinoQLLib.core.SaveLoad.ReadWriteObject<InitialDataBase>.Load(PathToFile);
            LoadDB.SetLoadSavePath(PathToFile);
            return LoadDB;
        }

        public static IDataBase Load(IConnectionString ConnectionString)
        {
            string PathToFile = ConnectionString.GetConnection();
            var LoadDB = PinoQLLib.core.SaveLoad.ReadWriteObject<InitialDataBase>.Load(PathToFile);
            LoadDB.SetLoadSavePath(PathToFile);
            return LoadDB;
        }

        public bool Save(string PathToFile = "")
        {
            if (PathToFile != null)
            {
                if(string.IsNullOrEmpty(PathToFile))
                {
                    PinoQLLib.core.SaveLoad.ReadWriteObject<InitialDataBase>.Save(this, lastPathToFileDB);
                }
                else
                {
                    SetLoadSavePath(PathToFile);
                    PinoQLLib.core.SaveLoad.ReadWriteObject<InitialDataBase>.Save(this, PathToFile);
                }
                return true;
            }
            else
            {
                if(!string.IsNullOrEmpty(lastPathToFileDB)) PinoQLLib.core.SaveLoad.ReadWriteObject<InitialDataBase>.Save(this, lastPathToFileDB);
                return false;
            }
        }

        public bool RemoveTable(string Name)
        {
            ITable tableRemove = Tables.Find(x => x.Name == Name);
            if (tableRemove != null)
            {
                Tables.Remove(tableRemove);
                return true;
            }
            else return false;
        }

        public bool RemoveColumn(string TableName, string ColumnName)
        {
            ITable tableDeleteColumn = Tables.Find(x => x.Name == TableName);
            if (tableDeleteColumn != null)
            {
                ITableColumn tableColumn = tableDeleteColumn.Columns.Find(x => x.Name == ColumnName);
                if (tableColumn != null)
                {
                    tableDeleteColumn.Columns.Remove(tableColumn);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public bool Remove(string PathToFile)
        {
            if (File.Exists(PathToFile))
            {
                File.Delete(PathToFile);
                return true;
            }
            else return false;
        }

        public bool RemoveRow(string TableName, int indexRow)
        {
            ITable tableRemoveRow = Tables.Find(x => x.Name == TableName);
            if (tableRemoveRow != null)
            {
                if (tableRemoveRow.Columns.Count > 0)
                {
                    if (indexRow > -1 && indexRow < tableRemoveRow.Columns[0].Property.Count)
                    {
                        for (int i = 0; i < tableRemoveRow.Columns.Count; i++)
                        {
                            tableRemoveRow.Columns[i].Property.RemoveAt(indexRow);
                        }
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }


        public bool RemoveRow(string TableName, int StartRow, int Count)
        {
            ITable tableRemoveRow = Tables.Find(x => x.Name == TableName);
            if (tableRemoveRow != null)
            {
                if (tableRemoveRow.Columns.Count > 0)
                {
                    if ((StartRow > -1 && StartRow < tableRemoveRow.Columns[0].Property.Count) && (Count > -1 && Count < tableRemoveRow.Columns[0].Property.Count))
                    {
                        if ((StartRow + Count) <= tableRemoveRow.Columns[0].Property.Count)
                        {
                            for(int i = 0; i < tableRemoveRow.Columns.Count; i++)
                            {
                                tableRemoveRow.Columns[i].Property.RemoveRange(StartRow, Count);
                            }
                            return true;
                        }
                        else return false;   
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }
    }
}
