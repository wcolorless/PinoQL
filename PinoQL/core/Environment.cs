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
    public class Environment
    {
        public static List<object> GlobalRows { get; set; }
        public static ITable GlovalTable { get; set; }


        public static List<object> ReGenRowForTable(ITable Table)
        {
            var rows = new List<object>();
            for (int r = 0; r < Table.Columns[0].Property.Count; r++)
            {
                dynamic expdObj = new ExpandoObject();
                for (int i = 0; i < Table.Columns.Count; i++)
                {
                    ((IDictionary<string, object>)expdObj)[Table.Columns[i].Name] = Table.Columns[i].Property[r];
                }
                rows.Add((object)expdObj);
            }
            Environment.GlobalRows = rows;
            return rows;
        }
    }
}
