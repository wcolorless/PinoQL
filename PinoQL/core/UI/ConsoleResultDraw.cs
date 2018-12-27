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
using PinoQLLib.core.DataProviders;
using System.Timers;

namespace PinoQL
{
    public class ConsoleResultDraw
    {
        public static TextBox textBox { get; set; }
        private static Timer timer;
        public static void DrawResult(IQueryResult result, TextBox textBox)
        {
            if(result != null && textBox != null)
            {
                ConsoleResultDraw.textBox = textBox;
                textBox.Text = "";
                if (result.Type == QueryResultType.Work)
                {
                    if (result.Message == QueryResultMessage.Ok)
                    {
                        textBox.Text = "OK";
                        textBox.Foreground = Brushes.GreenYellow;
                    }
                    else if (result.Message == QueryResultMessage.Empty)
                    {
                        textBox.Text = "Error";
                        textBox.Foreground = Brushes.Red;
                    }
                }
                else if(result.Type == QueryResultType.Info)
                {
                    textBox.Text = "get info";
                    textBox.Foreground = Brushes.GreenYellow;
                }
                timer = new Timer(500);
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }

           
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Timer timer = sender as Timer;
            timer.Stop();
            textBox.Dispatcher.Invoke(() => { ConsoleResultDraw.textBox.Text = ""; ConsoleResultDraw.textBox.Foreground = Brushes.Black; });

        }
    }
}
