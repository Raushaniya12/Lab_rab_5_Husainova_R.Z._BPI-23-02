using Lab_rab_5_Husainova_R.Z._BPI_23_02.Helper;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.Model;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Lab_rab_5_Husainova_R.Z._BPI_23_02.View
{
    public partial class WindowEmployee : Window
    {
        public WindowEmployee()
        {
            InitializeComponent();
            DataContext = PersonViewModel.Instance;
        }
    }
}
