using System;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.Model;

namespace Lab_rab_5_Husainova_R.Z._BPI_23_02.Helper
{
    public class FindPerson
    {
        private int _id;
        public FindPerson(int id)
        {
            _id = id;
        }

        public bool PersonPredicate(Person person)
        {
            return person.Id == _id;
        }
    }
}
