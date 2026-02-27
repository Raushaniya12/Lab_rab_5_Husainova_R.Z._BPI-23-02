using Lab_rab_5_Husainova_R.Z._BPI_23_02.Helper;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.Model;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Lab_rab_5_Husainova_R.Z._BPI_23_02.ViewModel
{
    public class PersonEditContext : INotifyPropertyChanged
    {
        public ObservableCollection<Role> Roles => RoleViewModel.Instance.ListRole;
        public PersonDpo Person { get; }
        public ICommand SaveCommand { get; }

        public PersonEditContext(PersonDpo person, Action saveAction)
        {
            Person = person;
            SaveCommand = new RelayCommand(
                _ => saveAction(),
                _ => !string.IsNullOrWhiteSpace(person.FirstName?.Trim()) &&
                     !string.IsNullOrWhiteSpace(person.LastName?.Trim()) &&
                     !string.IsNullOrWhiteSpace(person.RoleName?.Trim())
            );
        }

        public int Id
        {
            get => Person.Id;
            set => Person.Id = value;
        }

        public string RoleName
        {
            get => Person.RoleName;
            set => Person.RoleName = value;
        }

        public string FirstName
        {
            get => Person.FirstName;
            set => Person.FirstName = value;
        }

        public string LastName
        {
            get => Person.LastName;
            set => Person.LastName = value;
        }

        public DateTime Birthday
        {
            get => Person.Birthday;
            set => Person.Birthday = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PersonViewModel : INotifyPropertyChanged
    {
        private static PersonViewModel _instance;
        public static PersonViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PersonViewModel();
                }
                return _instance;
            }
        }

        private PersonDpo selectedPersonDpo;
        public PersonDpo SelectedPersonDpo
        {
            get => selectedPersonDpo;
            set
            {
                selectedPersonDpo = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Person> ListPerson { get; set; } = new ObservableCollection<Person>();
        public ObservableCollection<PersonDpo> ListPersonDpo { get; set; } = new ObservableCollection<PersonDpo>();

        public PersonViewModel()
        {
            ListPerson.Add(new Person { Id = 1, RoleId = 1, FirstName = "Иван", LastName = "Иванов", Birthday = new DateTime(1980, 2, 28) });
            ListPerson.Add(new Person { Id = 2, RoleId = 2, FirstName = "Петр", LastName = "Петров", Birthday = new DateTime(1981, 3, 20) });
            ListPerson.Add(new Person { Id = 3, RoleId = 3, FirstName = "Виктор", LastName = "Викторов", Birthday = new DateTime(1982, 4, 15) });
            ListPerson.Add(new Person { Id = 4, RoleId = 3, FirstName = "Сидор", LastName = "Сидоров", Birthday = new DateTime(1983, 5, 10) });
            RefreshListPersonDpo();
        }

        private void RefreshListPersonDpo()
        {
            ListPersonDpo.Clear();
            foreach (var person in ListPerson)
            {
                ListPersonDpo.Add(new PersonDpo().CopyFromPerson(person));
            }
        }

        public int MaxId() => ListPerson.Count == 0 ? 0 : ListPerson.Max(p => p.Id);

        private RelayCommand addPerson;
        public RelayCommand AddPerson => addPerson = new RelayCommand(_ =>
        {
            var wnPerson = new WindowNewEmployee { Title = "Новый сотрудник" };
            var personDpo = new PersonDpo
            {
                Id = MaxId() + 1,
                Birthday = DateTime.Now
            };

            var context = new PersonEditContext(personDpo, () =>
            {
                wnPerson.DialogResult = true;
                wnPerson.Close();
            });

            wnPerson.DataContext = context;

            if (wnPerson.ShowDialog() == true)
            {
                ListPersonDpo.Add(personDpo);
                ListPerson.Add(new Person().CopyFromPersonDPO(personDpo));
            }
        });

        private RelayCommand editPerson;
        public RelayCommand EditPerson => editPerson = new RelayCommand(_ =>
        {
            var wnPerson = new WindowNewEmployee { Title = "Редактирование данных сотрудника" };
            var tempPerson = SelectedPersonDpo.ShallowCopy();

            var context = new PersonEditContext(tempPerson, () =>
            {
                wnPerson.DialogResult = true;
                wnPerson.Close();
            });

            wnPerson.DataContext = context;

            if (wnPerson.ShowDialog() == true)
            {
                SelectedPersonDpo.RoleName = tempPerson.RoleName;
                SelectedPersonDpo.FirstName = tempPerson.FirstName;
                SelectedPersonDpo.LastName = tempPerson.LastName;
                SelectedPersonDpo.Birthday = tempPerson.Birthday;

                var existingPerson = ListPerson.FirstOrDefault(p => p.Id == SelectedPersonDpo.Id);
                if (existingPerson != null)
                {
                    existingPerson.CopyFromPersonDPO(SelectedPersonDpo);
                }
            }
        }, _ => SelectedPersonDpo != null);

        private RelayCommand deletePerson;
        public RelayCommand DeletePerson => deletePerson = new RelayCommand(_ =>
        {
            var dpo = SelectedPersonDpo;
            if (dpo == null) return;

            var result = MessageBox.Show($"Удалить сотрудника:\n{dpo.LastName} {dpo.FirstName}",
                "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                ListPersonDpo.Remove(dpo);
                var person = ListPerson.FirstOrDefault(p => p.Id == dpo.Id);
                if (person != null)
                    ListPerson.Remove(person);

                SelectedPersonDpo = null;
            }
        }, _ => SelectedPersonDpo != null);

        private RelayCommand openEmployeeWindow;
        public RelayCommand OpenEmployeeWindow => openEmployeeWindow = new RelayCommand(_ =>
        {
            new WindowEmployee { DataContext = PersonViewModel.Instance }.Show();
        });

        private RelayCommand openRoleWindow;
        public RelayCommand OpenRoleWindow => openRoleWindow = new RelayCommand(_ =>
        {
            new WindowRole { DataContext = RoleViewModel.Instance }.Show();
        });

        private static RelayCommand switchLightTheme;
        public static RelayCommand SwitchLightTheme => switchLightTheme = new RelayCommand(_ => ThemeManager.ApplyTheme("LightTheme"));

        private static RelayCommand switchDarkTheme;
        public static RelayCommand SwitchDarkTheme => switchDarkTheme = new RelayCommand(_ => ThemeManager.ApplyTheme("DarkTheme"));

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}