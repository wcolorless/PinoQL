using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.Table
{
    [Serializable]
    public class Table : ITable
    {
        public List<ITableColumn> Columns { get; set; }

        public string Name { get; set; }

        public Table()
        {
            Columns = new List<ITableColumn>();
        }

        public bool AddColumn(ITableColumn TableColumn)
        {
            if (Columns.FindAll(x => x.Name == TableColumn.Name).Count == 0)
            {
                if (Columns.Count > 0)
                {
                    for(int i = 0; i < Columns[0].Property.Count; i++)
                    {
                        TableColumn.Property.Add(null);
                    }
                }
                Columns.Add(TableColumn);
                return true;
            }
            else return false;
        }

        public bool RemoveColumn(string Name)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRow(int index)
        {
            if (Columns.Count > 0)
            {
                if (index > -1 && index < Columns[0].Property.Count)
                {
                    for(int i = 0; i < Columns.Count; i++)
                    {
                        Columns[i].Property.RemoveAt(index);
                    }
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public bool AddRow(List<object> newRow)
        {
            if (Columns.Count > 0)
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    Columns[i].Property.Add(newRow[i]);
                }
                return true;
            }
            else return false;
        }

        public bool UpdateRow(List<object> UpdateDataRow, int index)
        {
            if (Columns.Count > 0)
            {
                if (index > -1 && index < Columns[0].Property.Count)
                {
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        Columns[i].Property[index] = UpdateDataRow[i];
                    }
                    return true;
                }
                else return false;
            }
            else return false;
        }
    }
}
