using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Contract_Monthly_Claims_System__CMCS_
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private string[,] usersArray = new string[200, 4]; 
        private int userCount = 0;

        // Public properties to access the data
        public string[,] UsersArray => usersArray;
        public int UserCount => userCount;
        public List<User> UsersList { get; private set; } = new List<User>();

        public class User
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
        }


        public Window2()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string surname = SurnameTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UserRepository.Users.Add(new UserRepository.User
            {
                Name = name,
                Surname = surname,
                Password = password,
                Role = role
            });

            if (userCount < usersArray.GetLength(0))
            {
                usersArray[userCount, 0] = name;
                usersArray[userCount, 1] = surname;
                usersArray[userCount, 2] = password;
                usersArray[userCount, 3] = role;
                userCount++;

                // Also add to the list for easier access
                UsersList.Add(new User
                {
                    Name = name,
                    Surname = surname,
                    Password = password,
                    Role = role
                });

                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Navigate based on role
                if (role == "Lecturer")
                {
                    MainWindow login = new MainWindow();
                    login.Show();
                }
                else if (role == "Manager" || role == "Program Coordinator")
                {
                    MainWindow loggin = new MainWindow();
                    loggin.Show();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("User limit reached. Cannot register more users.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();  
            mainWindow.Show();
            this.Close();
        }
    }
}
