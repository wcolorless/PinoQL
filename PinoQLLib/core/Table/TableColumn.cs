using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinoQLLib.core.Table
{
    [Serializable]
    public class TableColumn : ITableColumn
    {
        public string Name { get; set; }

        public ColumnType Type { get; set; }

        public List<object> Property { get; set; }

        public List<RestrictionType> Restrictions { get; set; }

        public TableColumn(string Name, ColumnType Type)
        {
            this.Name = Name;
            this.Type = Type;
            Property = new List<object>();
            Restrictions = new List<RestrictionType>();
        }

        public bool AddRow(object Cell)
        {
            if(Restrictions.Contains(RestrictionType.UNIQUE))
            {
                switch(Type)
                {
                    case ColumnType.Int32:
                    case ColumnType.Int64:
                        long newCellInt64 = (long)Cell;
                        for(int i = 0; i < Property.Count; i++)
                        {
                            long innerCell = (long)Property[i];
                            if (innerCell == newCellInt64) return false; // Результат
                        }
                        break;
                    case ColumnType.Logic:
                        bool newCellLogic = (bool)Cell;
                        for (int i = 0; i < Property.Count; i++)
                        {
                            bool innerCell = (bool)Property[i];
                            if (innerCell == newCellLogic) return false; // Результат
                        }
                        break;
                    case ColumnType.Text:
                        string newCellText = (string)Cell;
                        for (int i = 0; i < Property.Count; i++)
                        {
                            string innerCell = (string)Property[i];
                            if (innerCell == newCellText) return false; // Результат
                        }
                        break;
                    default: break;
                }
            }
            if(Restrictions.Contains(RestrictionType.NOTNULL))
            {
                if (Cell == null) return false;
            }
            if (Restrictions.Contains(RestrictionType.AUTOINC))
            {
                if (Type == ColumnType.Int32)
                {
                    object buffObjInt32 = Property.Last();
                    if (buffObjInt32 != null)
                    {
                        long newCellInt32 = (long)buffObjInt32;
                        newCellInt32++;
                        Property.Add(newCellInt32);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (Type == ColumnType.Int64)
                {
                    object buffObjInt64 = Property.Last();
                    if (buffObjInt64 != null)
                    {
                        int newCellInt64 = (int)buffObjInt64;
                        newCellInt64++;
                        Property.Add(newCellInt64);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (Type == ColumnType.Logic) return false;
                else if (Type == ColumnType.Text) return false;
                else return false;
            }
            Property.Add(Cell);

            return false;
        }

        public bool AddRestriction(RestrictionType restriction)
        {
            if(Type == ColumnType.Logic || Type == ColumnType.Text)
            {
                if(restriction == RestrictionType.AUTOINC || restriction == RestrictionType.PK)
                {
                    return false;
                }
            }
            if (!Restrictions.Contains(restriction))
            {
                Restrictions.Add(restriction);
                return true;
            }
            else return false;
        }
    }
}
