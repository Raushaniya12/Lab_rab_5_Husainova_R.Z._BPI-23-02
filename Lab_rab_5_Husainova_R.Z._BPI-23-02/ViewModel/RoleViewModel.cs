using Lab_rab_5_Husainova_R.Z._BPI_23_02.Helper;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.Model;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.View;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Lab_rab_5_Husainova_R.Z._BPI_23_02.ViewModel
{
    public class RoleEditContext : INotifyPropertyChanged
    {
        public string Error { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
        readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataModel", "RoleData.json");
        string _jsonRoles = String.Empty;
        public string Error { get; set; }

        private ObservableCollection<Role> _listRole;
        public ObservableCollection<Role> ListRole
        {
            get => _listRole;
            set
            {
                _listRole = value;
                OnPropertyChanged();
            }
        }

        private Role _selectedRole;
        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
            }
        }

        public RoleViewModel()
        {
            _listRole = new ObservableCollection<Role>();
            ListRole = LoadRole();
        }

        public string GetRoleNameById(int id) => ListRole?.FirstOrDefault(r => r.Id == id)?.NameRole ?? string.Empty;
        public int GetRoleIdByName(string name) => ListRole?.FirstOrDefault(r => r.NameRole == name)?.Id ?? 0;

        private static RelayCommand switchLightTheme;
        public static RelayCommand SwitchLightTheme => switchLightTheme = new RelayCommand(_ => ThemeManager.ApplyTheme("LightTheme"));

        private static RelayCommand switchDarkTheme;
        public static RelayCommand SwitchDarkTheme => switchDarkTheme = new RelayCommand(_ => ThemeManager.ApplyTheme("DarkTheme"));

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<Role> LoadRole()
        {
            try
            {
                if (File.Exists(path))
                {
                    _jsonRoles = File.ReadAllText(path);
                    if (!string.IsNullOrEmpty(_jsonRoles))
                    {
                        var result = JsonConvert.DeserializeObject<ObservableCollection<Role>>(_jsonRoles);
                        return result ?? new ObservableCollection<Role>();
                    }
                }
                return new ObservableCollection<Role>();
            }
            catch (Exception ex)
            {
                Error = $"Ошибка чтения JSON-файла: {ex.Message}";
                return new ObservableCollection<Role>();
            }
        }

        
        public void SaveChanges(ObservableCollection<Role> listRole)
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

        public int MaxId() => RoleViewModel.Instance.ListRole.Count == 0 ? 0 : RoleViewModel.Instance.ListRole.Max(r => r.Id);

        private RelayCommand addRole;
        public RelayCommand AddRole => addRole = new RelayCommand(_ =>
        {
            var wnRole = new WindowNewRole { Title = "Новая должность" };

            int maxIdRole = MaxId() + 1;
            Role role = new Role { Id = maxIdRole };

            wnRole.DataContext = role;

            if (wnRole.ShowDialog() == true)
            {
                ListRole.Add(role);
                SaveChanges(ListRole);
                SelectedRole = role;
            }
        });


        private RelayCommand editRole;
        public RelayCommand EditRole => editRole = new RelayCommand(_ =>
        {
            var wnRole = new WindowNewRole { Title = "Редактирование должности" };

            Role role = SelectedRole;
            var tempRole = role.ShallowCopy();

            wnRole.DataContext = tempRole;

            if (wnRole.ShowDialog() == true)
            {
                role.NameRole = tempRole.NameRole;
                SaveChanges(ListRole);
            }
        }, _ => SelectedRole != null);


        private RelayCommand deleteRole;
        public RelayCommand DeleteRole => deleteRole = new RelayCommand(_ =>
        {
            var role = SelectedRole;
            if (role == null) return;

            var result = MessageBox.Show($"Удалить должность: {role.NameRole}?", "Предупреждение",
                MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                ListRole.Remove(role);
                SaveChanges(ListRole);
                SelectedRole = null;
            }
        }, _ => SelectedRole != null);

    }
}