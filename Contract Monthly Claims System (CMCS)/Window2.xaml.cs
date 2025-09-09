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
        public Window2()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            // Get the ComboBox from XAML by its Name
            if (this.FindName("RoleComboBox") is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedRole = selectedItem.Content.ToString();

                if (selectedRole == "Lecturer")
                {
                    // Open Window3 for Lecturer
                    Window3 window3 = new Window3();
                    window3.Show();
                }
                else if (selectedRole == "Manager" || selectedRole == "Program Coordinator")
                {
                    // Open Window1 for Manager/Program Coordinator
                    Window1 window1 = new Window1();
                    window1.Show();
                }

                // Close login window
                this.Close();
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
