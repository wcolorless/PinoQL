using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PinoQLLib.core.Environment
{
    public class WorkEnvironment
    {
        public static string LocalWorkPath { get; set; } 
        public static string SelectedDataBaseName { get; set;}
        public static string SelectedDataBasePath { get; set; }
        public static string SelectedTableName { get; set; }
        public static string LastCreatedDataBase { get; set; }
        public static string LastCreatedTable { get; set; }
        public static string LastOpenedDataBase { get; set; }
        public static int DatabaseCount { get; set; }
        public static Action<string> AddNewDBHandler;
        public static Action AddNewTableHandler;
        public static Action AddNewColumnHandler;
        public static Action RemoveDBHandler;


        public static string GetAllRows()
        {
            string LocalDBPath = Directory.GetCurrentDirectory() + "\\WorkDir";
            string[] Files = Directory.GetFiles(LocalDBPath);
            int value = 0;
            for(int i = 0; i < Files.Length; i++)
            {
                if (Path.GetExtension(Files[i]) == ".pdbf")  value++;
            }
            DatabaseCount = value;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Выбранная база данных: " + SelectedDataBaseName);
            sb.AppendLine("Выбранная таблица: " + SelectedTableName);
            sb.AppendLine("Всего баз данных: " + DatabaseCount.ToString());
            return sb.ToString();
        }

        public static string GetPathToDB(string Name)
        {
            Name = Name.ToUpper();
            string LocalDBPath = Directory.GetCurrentDirectory() + "\\WorkDir";
            string FullPath = LocalDBPath + "\\" + Name + ".pdbf";
            if (File.Exists(FullPath))
            {
                return FullPath;
            }
            else return null;
        }

        public static string[] GetAllDataBaseNames()
        {
            string LocalDBPath = Directory.GetCurrentDirectory() + "\\WorkDir";
            string[] Files = Directory.GetFiles(LocalDBPath);
            string[] Result = new string[Files.Length];
            for(int i = 0; i < Files.Length; i++)
            {
                Result[i] = Path.GetFileNameWithoutExtension(Files[i]);
            }
            return Result;
        }


        public static void Update()
        {
            string WorkDirPath = Directory.GetCurrentDirectory() + "\\WorkDir";
            if (!Directory.Exists(WorkDirPath)) Directory.CreateDirectory(WorkDirPath);
            WorkEnvironment.LocalWorkPath = WorkDirPath;
        }
    }
}
