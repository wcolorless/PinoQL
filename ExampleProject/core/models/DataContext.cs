using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinoQLLib;
using PinoQLLib.core.QueryGenerator.ConnectionString;

namespace ExampleProject
{
    [Serializable]
    public class ModelUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRegistered { get; set; }

        public ModelUser(int Id, string Name, bool IsRegistered)
        {
            this.Id = Id;
            this.Name = Name;
            this.IsRegistered = IsRegistered;
        }
    }

    [Serializable]
    public class ModelEmployee
    {
        public int Id { get; set; }
        public int NumberPhone { get; set; }
        public int LocalNumber { get; set; }

        public ModelEmployee(int Id, int NumberPhone, int LocalNumber)
        {
            this.Id = Id;
            this.NumberPhone = NumberPhone;
            this.LocalNumber = LocalNumber;
        }

        public ModelEmployee(object Id, object NumberPhone, object LocalNumber)
        {
            this.Id = (int)Id;
            this.NumberPhone = (int)NumberPhone;
            this.LocalNumber = (int)LocalNumber;
        }
    }

    public class DataContext : PinoQLContext
    {
        public DataContext(IConnectionString Connection) : base(Connection)
        {
            Users = new PinoQLDBSet<ModelUser>(Connection, WorkBehavior.NotSave);
            ModelEmployees = new PinoQLDBSet<ModelEmployee>(Connection, WorkBehavior.NotSave);
            RunInit(this);
            
        }

        public PinoQLDBSet<ModelUser> Users { get; set; }
        public PinoQLDBSet<ModelEmployee> ModelEmployees { get; set; }

    }
}
