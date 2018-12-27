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
    public class PinoQLContext
    {
        IConnectionString ConnectionString;
        object ContextObject;
        public PinoQLContext(IConnectionString ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public void RunInit(object ContextObject)
        {
            this.ContextObject = ContextObject;
            Set();
        }

        private Dictionary<string, string> GetTableColumns(DBPropertyInfo[] properties)
        {
            Dictionary<string, string> properiesList = new Dictionary<string, string>();
            for(int i = 0; i < properties.Length; i++)
            {
                properiesList.Add(properties[i].Name, properties[i].Type);
            }
            return properiesList;
        }

        private void Set() // Создание
        {
            System.Type typeInformation = ContextObject.GetType();
            var properties = typeInformation.GetTypeInfo().GetProperties();
            IQueryGenerator queryGenerator = QueryGenerator.Create(ConnectionString);
            for (int i = 0; i < properties.Length; i++)
            {
                var PropertyType = properties[i].PropertyType;
                if (properties[i].PropertyType.Name == typeof(PinoQLDBSet<>).Name)
                {
                    var FindTable = ConnectionString.CurrentDataBase.Tables.Find(x => x.Name == properties[i].Name.ToUpper());
                    if(FindTable == null) // Создаём таблицу
                    {
                        var propertiesDictionary = ((properties[i].GetValue(ContextObject)) as IDBProperties).GetNameProperties();
                        queryGenerator.CreateTable(properties[i].Name.ToUpper(), GetTableColumns(propertiesDictionary));
                        var FindNewTable = ConnectionString.CurrentDataBase.Tables.Find(x => x.Name == properties[i].Name.ToUpper());
                        if(FindNewTable != null)
                        {
                            var PropertyObject = properties[i].GetValue(ContextObject);
                            var DataSetObject = (PropertyObject as IDBSetFunctions);
                            DataSetObject.SetTable(FindNewTable);
                        }
                    }
                    else // 
                    {
                        var PropertyObject = properties[i].GetValue(ContextObject);
                        var GetGeneric = (PropertyObject as IDBSetFunctions).GetGenericType();
                        var Adder = (PropertyObject as IDBSetFunctions);
                        Adder.SetTable(FindTable);
                        for (int s = 0; s < FindTable.Columns[0].Property.Count; s++) // Rows
                        {
                            object[] paramsForNewObject = new object[GetGeneric.GetProperties().Length];
                            for(int z = 0; z < paramsForNewObject.Length; z++) // Columns
                            {
                                object fromDBObj = FindTable.Columns[z].Property[s];
                                paramsForNewObject[z] = fromDBObj;
                            }
                            var constructor = GetGeneric.GetConstructors();
                            object newPropObje = Activator.CreateInstance(GetGeneric, paramsForNewObject);
                            Adder.AddItemElement(newPropObje);
                        }
                    }
                }
            }
        }

    }
    
}
