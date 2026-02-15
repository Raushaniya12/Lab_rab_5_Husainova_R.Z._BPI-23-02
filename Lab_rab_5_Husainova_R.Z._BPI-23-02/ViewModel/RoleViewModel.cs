using Lab_rab_5_Husainova_R.Z._BPI_23_02.Helper;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.Model;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.View;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.Helper;
using Lab_rab_5_Husainova_R.Z._BPI_23_02.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Lab_rab_5_Husainova_R.Z._BPI_23_02.ViewModel
{
    public class RoleEditContext : INotifyPropertyChanged
    {
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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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

        public ObservableCollection<Role> ListRole { get; set; } = new ObservableCollection<Role>();

        public RoleViewModel()
        {
            ListRole.Add(new Role { Id = 1, NameRole = "Директор" });
            ListRole.Add(new Role { Id = 2, NameRole = "Бухгалтер" });
            ListRole.Add(new Role { Id = 3, NameRole = "Менеджер" });
        }

        public int MaxId() => ListRole.Count == 0 ? 0 : ListRole.Max(r => r.Id);

        public string GetRoleNameById(int id) => ListRole.FirstOrDefault(r => r.Id == id)?.NameRole ?? string.Empty;
        public int GetRoleIdByName(string name) => ListRole.FirstOrDefault(r => r.NameRole == name)?.Id ?? 0;

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

        private static RelayCommand switchLightTheme;
        public static RelayCommand SwitchLightTheme => switchLightTheme = new RelayCommand(_ => ThemeManager.ApplyTheme("LightTheme"));

        private static RelayCommand switchDarkTheme;
        public static RelayCommand SwitchDarkTheme => switchDarkTheme = new RelayCommand(_ => ThemeManager.ApplyTheme("DarkTheme"));

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

