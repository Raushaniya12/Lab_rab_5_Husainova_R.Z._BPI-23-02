using Lab_rab_5_Husainova_R.Z._BPI_23_02.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_rab_5_Husainova_R.Z._BPI_23_02.Helper
{
    public class FindRole
    {
        private int id;
        public FindRole(int id)
        {
            this.id = id;
        }
        public bool RolePredicate(Role role)
        {
            return role.Id == id;
        }
    }
}
