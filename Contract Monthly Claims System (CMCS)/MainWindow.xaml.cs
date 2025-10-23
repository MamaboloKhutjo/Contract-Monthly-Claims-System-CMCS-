using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Contract_Monthly_Claims_System__CMCS_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Window2 signupWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin(); 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window2 window2 = new Window2();
            window2.Show();
            this.Close();
        }

        private void PerformLogin()
        {
            string username = LoginNameTextBox.Text; 
            string password = LoginPasswordBox.Password; 

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if user exists in Window2 data
            bool loginSuccessful = CheckCredentials(username, password);

            if (loginSuccessful)
            {
                MessageBox.Show("Login successful!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Navigate to appropriate window based on user role
                NavigateToUserWindow(username);
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool CheckCredentials(string username, string password)
        {
            foreach (var user in UserRepository.Users)
            {
                string fullUsername = $"{user.Name} {user.Surname}";
                if (fullUsername == username && user.Password == password)
                {
                    UserRepository.CurrentUser = user;
                    return true;
                }
            }
            return false;
        }

        private string GetUserRole(string username)
        {
            foreach (var user in UserRepository.Users)
            {
                string fullUsername = $"{user.Name} {user.Surname}";
                if (fullUsername == username)
                {
                    return user.Role;
                }
            }
            return null;
        }

        private void NavigateToUserWindow(string username)
        {
            string role = GetUserRole(username);

            switch (role?.ToLower())
            {
                case "lecturer":
                    Window3 window3 = new Window3();
                    window3.Show();
                    break;
                case "manager":
                case "program coordinator":
                    Window1 window1 = new Window1();
                    window1.Show();
                    break;
                default:
                    MessageBox.Show("Unknown user role.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            this.Close();
        }
    }
}