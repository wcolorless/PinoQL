using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using PinoQLLib.core;
using PinoQLLib.core.DataBase;
using PinoQLLib.core.QueryGenerator.ConnectionString;
using PinoQLLib.core.QueryGenerator;
using PinoQLLib.core.DataProviders;
using PinoQLLib.core.Table;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace PinoQLLib
{

    public interface IDBProperties
    {
        DBPropertyInfo[] GetNameProperties();
    }

    public interface IDBSetDataLoader
    {
        void LoadData();
    }

    public interface IDBSetFunctions
    {
        Type GetGenericType();
        void AddItemElement(object RowObject);
        void SetTable(ITable Table);
    }

    public enum WorkBehavior
    {
        NotSave,
        AutoSave
    }

    public class DBPropertyInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class PinoQLSetEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        private int index;
        private List<T> Objects;
        public PinoQLSetEnumerator(List<T> Objects)
        {
            index = 0;
            this.Objects = Objects;
        }

        public T Current
        {
            get
            {
                return Objects[index];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Objects[index];
            }
        }

        public void Dispose()
        {

        }

        public void Reset()
        {
            index = 0;
        }

        public bool MoveNext()
        {
            if ((Objects.Count - 1) >= index + 1)
            {
                index += 1;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class PinoQLDBSet<T> : IDBProperties, IDBSetFunctions, IEnumerable<T>
    {
        IConnectionString Connection;
        ITable MyTable;
        List<T> Objects;
        Dictionary<int, byte[]> ChangeObjectList;
        int SaveIndex = 0;
        DBPropertyInfo[] InfoCollection;
        MD5 hashGenerator;
        WorkBehavior workBehavior = WorkBehavior.NotSave;

        private void InitObject(IConnectionString Connection)
        {
            this.Connection = Connection;
            Objects = new List<T>();
            ChangeObjectList = new Dictionary<int, byte[]>();
            hashGenerator = MD5.Create();
            GetInfo();
        }

        public PinoQLDBSet(IConnectionString Connection)
        {
            InitObject(Connection);
        }

        public PinoQLDBSet(IConnectionString Connection, WorkBehavior workBehavior)
        {
            InitObject(Connection);
            this.workBehavior = workBehavior;
        }

        void GetInfo()
        {
            var info = typeof(T).GetTypeInfo();
            var properties = info.GetProperties();
            InfoCollection = new DBPropertyInfo[properties.Length];
            for (int i = 0; i < InfoCollection.Length; i++)
            {
                InfoCollection[i] = new DBPropertyInfo() { Name = properties[i].Name, Type = properties[i].PropertyType.Name };
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new PinoQLSetEnumerator<T>(Objects);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count()
        {
            return Objects.Count;
        }

        public void AddItemElement(object RowObject)
        {
            Objects.Add((T)RowObject);
            SaveIndex = Objects.Count;
        }

        public void Add(T Item)
        {
            if (workBehavior == WorkBehavior.AutoSave)
            {
                Objects.Add(Item);
                SaveRowToDataBase(Item);
            }
            else if (workBehavior == WorkBehavior.NotSave)
            {
                Objects.Add(Item);
            }
        }

        public void Remove(int index)
        {
            Objects.RemoveAt(index);
            IQueryGenerator queryGenerator = QueryGenerator.Create(Connection);
            var result = queryGenerator.RemoveRow(MyTable.Name, index);
        }

        public void Remove(int StartIndex, int EndIndex)
        {
            Objects.RemoveRange(StartIndex, EndIndex);
            IQueryGenerator queryGenerator = QueryGenerator.Create(Connection);
            var result = queryGenerator.RemoveRow(MyTable.Name, StartIndex, EndIndex);
        }

        private byte[] SerializeRow(T AnyStruct)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, AnyStruct);
            return ms.ToArray();
        }

        public DBPropertyInfo[] GetNameProperties()
        {
            return InfoCollection;
        }

        public Type GetGenericType()
        {
            Type type = typeof(T);
            return type;
        }

        public void SaveChanges()
        {
            IQueryGenerator queryGenerator = QueryGenerator.Create(Connection);
            int size = Objects.Count;
            if(size < SaveIndex)
            {
                SaveIndex = Objects.Count;
                queryGenerator.SaveChanges();
            }
            if (size > SaveIndex)
            {
                for (int i = SaveIndex; i < size; i++)
                {
                    if(i <= (Objects.Count - 1))
                    {
                        T item = Objects[i];
                        SaveRowToDataBase(item);
                    }
                }
                queryGenerator.SaveChanges();
                SaveIndex = Objects.Count;
            }
            //
            if (ChangeObjectList.Count > 0)
            {
                var listKeys = ChangeObjectList.Keys.ToList();
                for (int i = 0; i < listKeys.Count; i++)
                {
                    T existObject = Objects[listKeys[i]];
                    byte[] objectBinary = SerializeRow(existObject);
                    byte[] hashBinary = hashGenerator.ComputeHash(objectBinary);
                    byte[] initialHash = ChangeObjectList[listKeys[i]];
                    for (int s = 0; s < 16; s++)
                    {
                        if (initialHash[s] != hashBinary[s])
                        {
                            queryGenerator.UpdateRow(MyTable.Name, listKeys[i], existObject);
                            break;
                        }
                    }
                }
                queryGenerator.SaveChanges();
                ChangeObjectList.Clear();
            }
        }

        public T this[int index]
        {
            get
            {
                T getObject;
                if (index <= (Objects.Count - 1))
                {
                    getObject = Objects[index];
                }
                else return default(T);
                var result = ChangeObjectList.ContainsKey(index);
                if (!result)
                {
                    byte[] objectBinary = SerializeRow(getObject);
                    byte[] hashBinary = hashGenerator.ComputeHash(objectBinary);
                    ChangeObjectList.Add(index, hashBinary);
                }
                return getObject;
            }
            set
            {
                if (index <= (Objects.Count - 1))
                {
                    Objects[index] = value;
                }
                else
                {
                    this.Add(value);
                }
            }
        }

        private void SaveRowToDataBase(object NewRowObject)
        {
            IQueryGenerator queryGenerator = QueryGenerator.Create(Connection);
            queryGenerator.CreateRow(MyTable.Name, NewRowObject);
        }

        public void SetTable(ITable Table)
        {
            this.MyTable = Table;
        }

    }
}
