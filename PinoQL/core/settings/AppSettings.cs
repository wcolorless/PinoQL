using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PinoQLLib.core.SaveLoad;

namespace PinoQL
{
    [Serializable]
    public class AppSettings
    {
        private static AppSettings _Instance;

        private AppSettings()
        {
            _AllKnownDataBases = new List<string>();
        }

        public static AppSettings GetInstance()
        {
            if(_Instance == null)
            {
                string PathToSettingsFile = Directory.GetCurrentDirectory() + "\\settings.bin";
                if (File.Exists(PathToSettingsFile))
                {
                    _Instance = ReadWriteObject<AppSettings>.Load(PathToSettingsFile);
                }
                else
                {
                    _Instance = new AppSettings();
                    ReadWriteObject<AppSettings>.Save(_Instance, PathToSettingsFile);
                }
            }
            return _Instance;
        }

        public void Update()
        {
            string PathToSettingsFile = Directory.GetCurrentDirectory() + "\\settings.bin";
            ReadWriteObject<AppSettings>.Save(_Instance, PathToSettingsFile);
        }
        //////////////////////////////////////////////////////////////////////////////////

        private string _LastDataBase;
        private List<string> _AllKnownDataBases;



        public string LastDataBase
        {
            get
            {
                return _LastDataBase;
            }
            set
            {
                _LastDataBase = value;
            }
        }

        public string[] AllKnownDataBases
        {
            get
            {
                return _AllKnownDataBases.ToArray();
            }
        }

        public void AddNewDataBase(string PathToDataBase)
        {
            if(!_AllKnownDataBases.Contains(PathToDataBase))
            {
                _AllKnownDataBases.Add(PathToDataBase);
                UpdateKnowDBList();
            }
        }

        public void UpdateKnowDBList()
        {
            _AllKnownDataBases = _AllKnownDataBases.Where(x => File.Exists(x)).ToList();
            Update();
        }
    }
}
