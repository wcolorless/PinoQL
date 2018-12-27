using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace PinoQLLib.core.SaveLoad
{

    public class ReadWriteObject<T>
    {
        public static void Save(T Structure, string PathToFile)
        {
            using (FileStream fs = new FileStream(PathToFile, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Structure);
                fs.Close();
            }
        }

        public static T Load(string PathToFile)
        {
            using (FileStream fs = new FileStream(PathToFile, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                var tmp = (T)bf.Deserialize(fs);
                fs.Close();
                return tmp;
            }
        }
    }
}
