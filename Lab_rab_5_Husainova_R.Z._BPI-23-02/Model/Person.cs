using Lab_rab_5_Husainova_R.Z._BPI_23_02.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_rab_5_Husainova_R.Z._BPI_23_02.Model
{
    public class Person
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthday { get; set; }
        public Person() { }
        public Person(int id, int roleId, string firstName, string lastName, string birthday)
        {
            this.Id = id; this.RoleId = roleId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Birthday = birthday;
        }
        public Person CopyFromPersonDPO(PersonDpo dpo)
        {
            RoleViewModel vmRole = RoleViewModel.Instance;
            int roleId = 0;
            foreach (var r in vmRole.ListRole)
            {
                if (r.NameRole == dpo.RoleName)
                {
                    roleId = r.Id;
                    break;
                }
            }
            return new Person
            {
                Id = dpo.Id,
                RoleId = roleId,
                FirstName = dpo.FirstName,
                LastName = dpo.LastName,
                Birthday = dpo.Birthday
            };
        }
    }
}
