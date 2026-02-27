using Lab_rab_5_Husainova_R.Z._BPI_23_02.Helper;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.Model;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Lab_rab_5_Husainova_R.Z._BPI_23_02.ViewModel
{

    public class RoleEditContext : INotifyPropertyChanged
    {
        public string Error { get; set; }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        readonly string path = @"DataModels\RoleData.json";
       
        private Role selectedRole;
        public Role SelectedRole
        {
            get => selectedRole;
            set
            {
                selectedRole = value;
                OnPropertyChanged();
            }
        }
        
        private void RefreshRoleList()
        {
            OnPropertyChanged("ListRole");
            var tempList = new ObservableCollection<Role>(ListRole);
            ListRole.Clear();
            foreach (var item in tempList)
            {
                ListRole.Add(item);
            }
        }
        public int MaxId() => ListRole.Count == 0 ? 0 : ListRole.Max(r => r.Id);

        private RelayCommand addRole;
        public RelayCommand AddRole => addRole = new RelayCommand(_ =>
        {
            var wnRole = new WindowNewRole { Title = "Новая должность" };
            var role = new Role { Id = MaxId() + 1 };

            var context = new RoleEditContext(role, () =>
            {
                wnRole.DialogResult = true;
                wnRole.Close();
            });

            wnRole.DataContext = context;

            if (wnRole.ShowDialog() == true)
            {
                ListRole.Add(role);
                SelectedRole = role;
            }
        });

        private RelayCommand editRole;
        public RelayCommand EditRole => editRole = new RelayCommand(_ =>
        {
            var wnRole = new WindowNewRole { Title = "Редактирование должности" };
            var tempRole = SelectedRole.ShallowCopy();

            var context = new RoleEditContext(tempRole, () =>
            {
                wnRole.DialogResult = true;
                wnRole.Close();
            });

            wnRole.DataContext = context;

            if (wnRole.ShowDialog() == true)
            {
                SelectedRole.NameRole = tempRole.NameRole;
            }
        }, _ => SelectedRole != null);

        private RelayCommand deleteRole;
        public RelayCommand DeleteRole => deleteRole = new RelayCommand(_ =>
        {
            var role = SelectedRole;
            if (role == null) return;

            var result = MessageBox.Show($"Удалить должность: {role.NameRole}?", "Подтверждение",
                MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                ListRole.Remove(role);
                SelectedRole = null;
            }
        }, _ => SelectedRole != null);

        string _jsonRoles = String.Empty;
        public ObservableCollection<Role> ListRole { get; set; } = new ObservableCollection<Role>();
        public ObservableCollection<Role> LoadRole()
        {
            _jsonRoles = File.ReadAllText(path);

            if (_jsonRoles != null)
            {
                ListRole = JsonConvert.DeserializeObject<ObservableCollection<Role>>(_jsonRoles);
                return ListRole;
            }
            else
            {
                return null;
            }
        }
        
        private void SaveChanges(ObservableCollection<Role> listRole)
        {
            var jsonRole = JsonConvert.SerializeObject(listRole);

            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(jsonRole);
                }
            }
            catch (IOException e)
            {
                Error = "Ошибка записи json файла\n" + e.Message;
            }
        }
        public Role Role { get; }
        public ICommand SaveCommand { get; }

        public RoleEditContext(Role role, Action saveAction)
        {
            Role = role;
            SaveCommand = new RelayCommand(_ => saveAction(), _ => !string.IsNullOrWhiteSpace(role.NameRole?.Trim()));
        }

        public int Id
        {
            get => Role.Id;
            set => Role.Id = value;
        }

        public string NameRole
        {
            get => Role.NameRole;
            set => Role.NameRole = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
    }
    

    public class RoleViewModel : INotifyPropertyChanged
    {
        private static RoleViewModel _instance;
        public static RoleViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RoleViewModel();
                }
                return _instance;
            }
        }

        

        public ObservableCollection<Role> ListRole { get; set; } = new ObservableCollection<Role>();

        
        

        public string GetRoleNameById(int id) => ListRole.FirstOrDefault(r => r.Id == id)?.NameRole ?? string.Empty;
        public int GetRoleIdByName(string name) => ListRole.FirstOrDefault(r => r.NameRole == name)?.Id ?? 0;

        

        private static RelayCommand switchLightTheme;
        public static RelayCommand SwitchLightTheme => switchLightTheme = new RelayCommand(_ => ThemeManager.ApplyTheme("LightTheme"));

        private static RelayCommand switchDarkTheme;
        public static RelayCommand SwitchDarkTheme => switchDarkTheme = new RelayCommand(_ => ThemeManager.ApplyTheme("DarkTheme"));

        public event PropertyChangedEventHandler PropertyChanged;
        
    }
}