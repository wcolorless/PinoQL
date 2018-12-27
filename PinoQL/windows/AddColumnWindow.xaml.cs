using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PinoQLLib.core;
using PinoQLLib.core.Table;

namespace PinoQL
{
    /// <summary>
    /// Логика взаимодействия для AddColumn.xaml
    /// </summary>
    public partial class AddColumnWindow : Window
    {
        ITable table;
        string[] TypesComboBoxSourece = { "Int32", "Int64", "Logic", "Text" };
        public AddColumnWindow(ITable table)
        {
            InitializeComponent();
            this.table = table;
            ColumnTypeBox.ItemsSource = TypesComboBoxSourece;
            ColumnTypeBox.SelectedIndex = 0;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }



        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Close();

        }

        private void AddColumnBtn(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(ColumnNameBox.Text))
            {
                ITableColumn newColumn = new TableColumn(ColumnNameBox.Text, ColumnType.empty);
                switch (ColumnTypeBox.SelectedIndex)
                {
                    case 0:
                        newColumn.Type = ColumnType.Int32;
                        break;
                    case 1:
                        newColumn.Type = ColumnType.Int64;
                        break;
                    case 2:
                        newColumn.Type = ColumnType.Logic;
                        break;
                    case 3:
                        newColumn.Type = ColumnType.Text;
                        break;
                    default: break;
                }
                table.AddColumn(newColumn);
                Close();
            }
        }
    }
}
