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
using System.Diagnostics;
using PinoQLLib.core.DataBase;
using PinoQLLib.core.QueryGenerator.ConnectionString;

namespace ExampleProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string CString = @"C:\testdb\testdb.pdbf";
        DataContext dataContext;
        Random Ran;
        public MainWindow()
        {
            InitializeComponent();
           
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            dataContext = new DataContext(ConnectionString.Parse(CString));
            sw.Stop();
            LoadTimeText.Text = sw.ElapsedMilliseconds.ToString();
            TotalObjectText.Text = dataContext.ModelEmployees.Count().ToString();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Ran = new Random();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for(int i = 0; i < 1000; i++)
            {
                dataContext.ModelEmployees.Add(new ModelEmployee(Ran.Next(), Int32.Parse(NumberBox.Text), Int32.Parse(LocalBox.Text)));
            }
            sw.Stop();
            AddTimeText.Text = sw.ElapsedMilliseconds.ToString();
            TotalObjectText.Text = dataContext.ModelEmployees.Count().ToString();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            dataContext.ModelEmployees.SaveChanges();
            sw.Stop();
            SaveTimeText.Text = sw.ElapsedMilliseconds.ToString();
            TotalObjectText.Text = dataContext.ModelEmployees.Count().ToString();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {




        }

        private void button3_Copy_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if(dataContext.ModelEmployees.Count() > 0)
            {
                /* for (int i = 0; i < 1000; i++)
                 {
                     dataContext.ModelEmployees.Remove(i);
                 }*/
                dataContext.ModelEmployees.Remove(0, 1000);
            }
            dataContext.ModelEmployees.SaveChanges();
            sw.Stop();
            RemoveSaveTimeText.Text = sw.ElapsedMilliseconds.ToString();
            TotalObjectText.Text = dataContext.ModelEmployees.Count().ToString();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Ran = new Random();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (dataContext.ModelEmployees.Count() > 0)
            {
                for (int i = 0; i < 1000; i++)
                {
                    dataContext.ModelEmployees[i].Id = Ran.Next();
                }
            }
            dataContext.ModelEmployees.SaveChanges();
            sw.Stop();
            ModifySaveTimeText.Text = sw.ElapsedMilliseconds.ToString();
            TotalObjectText.Text = dataContext.ModelEmployees.Count().ToString();
            
        }
    }
}
