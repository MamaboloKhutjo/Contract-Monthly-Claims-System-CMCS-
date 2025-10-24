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
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        private string _currentUserName;
        public Window4(string currentUserName)
        {
            InitializeComponent();
            _currentUserName = currentUserName;
            LoadClaims();
        }

        private void LoadClaims()
        {

            var userClaims = ClaimRepository.Claims
                .Where(c => c.LecturerName == _currentUserName)
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            ClaimsDataGrid.ItemsSource = userClaims;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClaimsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
