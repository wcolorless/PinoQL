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


namespace PinoQL
{
    public class DrawConsoleBlock
    {
        public static void InfoBlock(WrapPanel Panel, IQueryResult Result)
        {
            var Text = new TextBlock() { Text = Result.ResultText, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Stretch, Margin = new Thickness(10) };
            Grid grid = new Grid() { Width = Panel.ActualWidth - 20, HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(10), Height = Text.Height };
            grid.Children.Add(new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) });
            grid.Children.Add(Text);
            Panel.Children.Add(grid);
        }
    }
}
