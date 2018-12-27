using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Dynamic;
using PinoQLLib.core;
using PinoQLLib.core.Table;


namespace PinoQL
{
    public class TableToGridTranslator
    {
        public static void Translate(ITable Table, DataGrid dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            Environment.GlovalTable = Table;
            for (int c = 0; c < Table.Columns.Count; c++)
            {
                Binding columnBinding = new Binding(Table.Columns[c].Name);
                DataGridColumn newColumn = new DataGridTextColumn() { Header = Table.Columns[c].Name, IsReadOnly = false, Binding = columnBinding };
                switch (Table.Columns[c].Type)
                {
                    case ColumnType.Int32:
                    case ColumnType.Int64:
                        newColumn = new DataGridTextColumn() { Header = Table.Columns[c].Name, IsReadOnly = false, Binding = columnBinding };
                        break;
                    case ColumnType.Logic:
                        newColumn = new DataGridTextColumn() { Header = Table.Columns[c].Name, IsReadOnly = false, Binding = columnBinding };
                        break;
                    case ColumnType.Text:
                        break;
                    default:
                        newColumn = new DataGridTextColumn() { Header = Table.Columns[c].Name, IsReadOnly = false, Binding = columnBinding };
                        break;
                }
               dataGrid.Columns.Add(newColumn);
            }
            var rows =  new List<object>();
            if(Table.Columns != null && Table.Columns.Count > 0)
            {
                for (int r = 0; r < Table.Columns[0].Property.Count; r++)
                {
                    dynamic expdObj = new ExpandoObject();
                    for (int i = 0; i < Table.Columns.Count; i++)
                    {
                        ((IDictionary<string, object>)expdObj)[Table.Columns[i].Name] = Table.Columns[i].Property[r];
                    }
                    rows.Add((object)expdObj);
                }
            }

            dataGrid.ItemsSource = rows;
            Environment.GlobalRows = rows;
        }

        public static void BackTranslate()
        {
            if(Environment.GlovalTable != null)
            {
                for (int i = 0; i < Environment.GlobalRows.Count; i++)
                {
                    IDictionary<string, object> expdObj = Environment.GlobalRows[i] as IDictionary<string, object>;
                    Environment.GlovalTable.UpdateRow(expdObj.Values.ToList(), i);
                }
            }

        }
    }
}
