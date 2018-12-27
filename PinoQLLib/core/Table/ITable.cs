using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core
{
    public interface ITable
    {
        string Name { get; set; }
        List<ITableColumn> Columns { get; set; }
        bool AddColumn(ITableColumn TableColumn);
        bool AddRow(List<object> newRow);
        bool RemoveColumn(string Name);
        bool RemoveRow(int index);
        bool UpdateRow(List<object> UpdateDataRow, int index);
    }
}
