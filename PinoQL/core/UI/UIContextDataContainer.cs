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
    public class UIContextDataContainer
    {
        public IDataBase DataBase { get; set; }
        public ITable Table { get; set; }

    }
}
