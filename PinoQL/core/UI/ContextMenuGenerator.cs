using System;
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
using Microsoft.Win32;
using System.IO;
using PinoQLLib;
using PinoQLLib.core;
using PinoQLLib.core.Environment;
using PinoQLLib.core.Translator;
using PinoQLLib.core.Queries;
using PinoQLLib.core.DataProviders;
using PinoQLLib.core.DataBase;

namespace PinoQL
{
    public class ContextMenuGenerator
    {
        public static ContextMenu GetForDataGrid(DataGrid dataGrid)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem AddItem = new MenuItem() { Header = "Добавить запись" };
            AddItem.DataContext = dataGrid;
            AddItem.Click += AddItem_Click;
            MenuItem RemoveItem = new MenuItem() { Header = "Удалить запись" };
            RemoveItem.DataContext = dataGrid;
            RemoveItem.Click += RemoveItem_Click;
            contextMenu.Items.Add(AddItem);
            contextMenu.Items.Add(RemoveItem);
            return contextMenu;
        }

        private static void AddItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            DataGrid dataGrid = item.DataContext as DataGrid;
            List<object> newObjectProperties = new List<object>();
            for (int i = 0; i < Environment.GlovalTable.Columns.Count; i++)
            {
                switch(Environment.GlovalTable.Columns[i].Type)
                {
                    case ColumnType.Int32:
                        newObjectProperties.Add(0);
                        break;
                    case ColumnType.Int64:
                        newObjectProperties.Add(0);
                        break;
                    case ColumnType.Logic:
                        newObjectProperties.Add(false);
                        break;
                    case ColumnType.Text:
                        newObjectProperties.Add("");
                        break;
                    default: break;
                }
            }
            Environment.GlovalTable.AddRow(newObjectProperties);
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Environment.ReGenRowForTable(Environment.GlovalTable);
            dataGrid.ScrollIntoView(Environment.GlobalRows.Last());
        }

        private static void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            DataGrid dataGrid = item.DataContext as DataGrid;
            if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedIndex < (dataGrid.Items.Count - 1))
            {
                if(Environment.GlovalTable != null && Environment.GlobalRows != null)
                {
                    Environment.GlobalRows.RemoveAt(dataGrid.SelectedIndex);
                    Environment.GlovalTable.RemoveRow(dataGrid.SelectedIndex);
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = Environment.GlobalRows;
                }
            }

            
        }
    }
}
