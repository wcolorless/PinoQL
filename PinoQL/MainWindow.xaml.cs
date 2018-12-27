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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        AppSettings Settings;
        IConsoleLog LogConsole;
        PinoQLLib.core.DataBase.IDataBase DataBase;
        ITranslator Translator;
        IDataProvider dataProvider;
        bool WasEditing = false;

        
        public MainWindow()
        {
            InitializeComponent();
            LogConsole =  ConsoleLog.Create();
            WorkEnvironment.Update();
            WorkEnvironment.AddNewDBHandler += AddNewDb;
            WorkEnvironment.AddNewTableHandler += ReDraw;
            WorkEnvironment.RemoveDBHandler += DeleteDB;
            Settings = AppSettings.GetInstance();
            DataBase = InitialDataBase.Get("Initial");
            UpdateTreeDrawAllDataBases(Settings.AllKnownDataBases); 
            Translator = new Translator();
            dataProvider = DataProvider.Create(DataBase);
            PrimeDataGrid.ContextMenu = ContextMenuGenerator.GetForDataGrid(PrimeDataGrid);
        }

        void DeleteDB() //
        {
            Settings.UpdateKnowDBList();
            ReDraw();
        }

        void ReDraw()
        {
            UpdateTreeDrawAllDataBases();
        }

        void AddNewDb(string NewPath)
        {
            if (Settings != null)
            {
                Settings.AddNewDataBase(NewPath);
                UpdateTreeDrawAllDataBases(Settings.AllKnownDataBases);
            }
        }

        void UpdateTreeDrawAllDataBases(string[] Paths = null)
        {
            string[] DataBases;
            if (Paths == null) DataBases = Settings.AllKnownDataBases;
            else DataBases = Paths;
            IDataBase dataBase = InitialDataBase.Get("Initial");
            treeViewDB.Items.Clear();
            for (int i = 0; i < DataBases.Length; i++)
            {
                dataBase = InitialDataBase.Load(DataBases[i]);
                CreateTreeConcreteDB(dataBase);
                if (DataBases.Length == 1)
                {
                    DataBase = dataBase;
                }
            }
        }

        void UpdateTreeDrawConcreteDataBases(IDataBase dataBase)
        {
            treeViewDB.Items.Clear();
            CreateTreeConcreteDB(dataBase);
        }

        void CreateTreeConcreteDB(IDataBase dataBase)
        {
            if (dataBase != null)
            {
                var L1Tree = new TreeViewItem() { Header = dataBase.Name, FontWeight = FontWeights.Bold };
                ContextMenu DBContextMenu = new ContextMenu();
                MenuItem SaveDataBase = new MenuItem() { Header = "Сохранить как" };
                SaveDataBase.DataContext = dataBase;
                SaveDataBase.Click += SaveDataBase_Click;
                MenuItem RemoveDataBase = new MenuItem() { Header = "Удалить" };
                RemoveDataBase.DataContext = dataBase;
                RemoveDataBase.Click += RemoveDataBase_Click;
                DBContextMenu.Items.Add(SaveDataBase);
                DBContextMenu.Items.Add(RemoveDataBase);
                L1Tree.ContextMenu = DBContextMenu;
                var L2Tree_Tables = new TreeViewItem() { Header = "Taблицы", Foreground = Brushes.BlueViolet };
                ContextMenu TablesContextMenu = new ContextMenu();
                MenuItem AddTable = new MenuItem() { Header = "Добавить" };
                TablesContextMenu.Items.Add(AddTable);
                L2Tree_Tables.ContextMenu = TablesContextMenu;
                for (int i = 0; i < dataBase.Tables.Count; i++)
                {
                    TreeViewItem newTable = new TreeViewItem() { Header = dataBase.Tables[i].Name };
                    ContextMenu TableContextMenu = new ContextMenu();
                    MenuItem OpenTable = new MenuItem() { Header = "Просмотр" };
                    OpenTable.DataContext = dataBase.Tables[i];
                    OpenTable.Click += OpenTable_Click;
                    MenuItem AddColumnTable = new MenuItem() { Header = "Добавить столбец" };
                    AddColumnTable.DataContext = new UIContextDataContainer() { DataBase = dataBase, Table = dataBase.Tables[i] };
                    AddColumnTable.Click += AddColumnTable_Click;
                    MenuItem RemoveTable = new MenuItem() { Header = "Удалить" };
                    TableContextMenu.Items.Add(OpenTable);
                    TableContextMenu.Items.Add(AddColumnTable);
                    TableContextMenu.Items.Add(RemoveTable);
                    newTable.ContextMenu = TableContextMenu;
                    for (int j = 0; j < dataBase.Tables[i].Columns.Count; j++)
                    {
                        TreeViewItem newColumn = new TreeViewItem() { Header = dataBase.Tables[i].Columns[j].Name };
                        newTable.Items.Add(newColumn);
                    }
                    L2Tree_Tables.Items.Add(newTable);
                }
                L1Tree.Items.Add(L2Tree_Tables);
                treeViewDB.Items.Add(L1Tree);
            }
        }

        private void AddColumnTable_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            UIContextDataContainer ContextContainer = item.DataContext as UIContextDataContainer;
            AddColumnWindow addColumnWindow = new AddColumnWindow(ContextContainer.Table);
            addColumnWindow.ShowDialog();
            ContextContainer.DataBase.Save();
            treeViewDB.Items.Clear();
            CreateTreeConcreteDB(ContextContainer.DataBase);
            TableToGridTranslator.Translate(ContextContainer.Table, PrimeDataGrid);
        }

        private void SaveDataBase_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            IDataBase database = menuItem.DataContext as IDataBase;
            SaveFileDialog sf = new SaveFileDialog() { Filter = "Pino DataBase File|*.pdbf" };
            if (sf.ShowDialog() == true)
            {
                PinoQLLib.core.SaveLoad.ReadWriteObject<PinoQLLib.core.DataBase.IDataBase>.Save(database, sf.FileName);
            }
        }

        private void RemoveDataBase_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            IDataBase database = menuItem.DataContext as IDataBase;
            IQuery query = new Query(database.Name, QueryType.Delete, QueryObject.DataBase, "", 0, "", "");
            IQueryResult Result = dataProvider.RunQuery(query);
        }

        private void OpenTable_Click(object sender, RoutedEventArgs e)
        {
            MenuItem Item = sender as MenuItem;
            ITable Table = Item.DataContext as ITable;
            if(Table != null)
            {
                //PrimeDataGrid.ItemsSource = Table.Columns;
                TableToGridTranslator.Translate(Table, PrimeDataGrid);
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
           
        }

        private void CloseAppBtn(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateNewDBBtn(object sender, RoutedEventArgs e)
        {
            DataBase = PinoQLLib.core.DataBase.InitialDataBase.Get("123");
        }

        private void OpenDBBtn(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog() { Filter = "Pino DataBase File|*.pdbf"};
            if(of.ShowDialog() == true)
            {
                DataBase = PinoQLLib.core.SaveLoad.ReadWriteObject<PinoQLLib.core.DataBase.IDataBase>.Load(of.FileName);
                (DataBase as InitialDataBase).SetLoadSavePath(of.FileName);
                UpdateTreeDrawConcreteDataBases(DataBase);
            }
            //PinoQLLib.core.DataBase.InitialDataBase test = DataBase as PinoQLLib.core.DataBase.InitialDataBase;
        }

        private void SaveDBBtn(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog() { Filter = "Pino DataBase File|*.pdbf" };
            if (sf.ShowDialog() == true)
            {
                PinoQLLib.core.SaveLoad.ReadWriteObject<PinoQLLib.core.DataBase.IDataBase>.Save(DataBase, sf.FileName);
            }
            
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (e.Key == Key.Enter)
            {
                IQuery query = Translator.GetQuery(box.Text);
                IQueryResult Result = dataProvider.RunQuery(query);
                LogConsole.AddItem(ConsoleLogItem.Create(box.Text, Result, DateTime.Now));
                ConsoleResultDraw.DrawResult(Result, ConsoleBox);
                if (Result != null)
                {
                    UpdateTreeDrawAllDataBases();
                    if (Result.Type == QueryResultType.Info)   DrawConsoleBlock.InfoBlock(ConsolePanel, Result);
                    ConsolePanelScroll.ScrollToEnd();
                }
            }
        }

        private void ConsoleBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (e.Key == Key.Down)
            {
                IConsoleLogItem Item = LogConsole.GetNext();
                box.Text = Item.CommandText;
            }
            else if (e.Key == Key.Up)
            {
                IConsoleLogItem Item = LogConsole.GetPrev();
                box.Text = Item.CommandText;
            }
        }

        private void PrimeDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            WasEditing = true;
        }

        private void PrimeDataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
        
        }

        private void SaveAsBtn(object sender, RoutedEventArgs e)
        {
            if(DataBase != null)
            {
                if(WasEditing)
                {
                    TableToGridTranslator.BackTranslate();
                    DataBase.Save();
                }
            }
        }
    }
}
