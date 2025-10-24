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
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace Contract_Monthly_Claims_System__CMCS_
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        private ClaimsSummary _claimsSummary;
        private string _currentUserName;

        public string CurrentUserName
        {
            get => _currentUserName;
            set
            {
                _currentUserName = value;
                OnPropertyChanged(nameof(CurrentUserName));
            }
        }

        public ClaimsSummary ClaimsSummary
        {
            get => _claimsSummary;
            set
            {
                _claimsSummary = value;
                OnPropertyChanged(nameof(ClaimsSummary));
            }
        }
        public ObservableCollection<Claim> Claims => ClaimRepository.Claims;



        public Window3()
        {
            InitializeComponent();

            // Set the current user name
            if (UserRepository.CurrentUser != null)
            {
                CurrentUserName = UserRepository.CurrentUser.FormalName;
            }
            else
            {
                CurrentUserName = "Guest User";
            }

            // Initialize claims summary
            ClaimsSummary = new ClaimsSummary();
            UpdateClaimsSummary();

            // Subscribe to claims collection changes
            ClaimRepository.Claims.CollectionChanged += (s, e) => UpdateClaimsSummary();

            // Set the data context for binding
            this.DataContext = this; ;

        }

        private void UpdateClaimsSummary()
        {
            ClaimsSummary.UpdateSummary(ClaimRepository.Claims, UserRepository.CurrentUser?.FullName);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SubmitNewClaim();
        }

        private void SubmitNewClaim()
        {
            var submitClaimWindow = new Window()
            {
                Title = "Submit New Claim - CMCS",
                Width = 450,
                Height = 550,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F7F9")),
                Icon = this.Icon // Use same icon as main window
            };

            var mainGrid = new Grid { Margin = new Thickness(0) };
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var headerBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#001a4d")),
                Height = 80,
                VerticalAlignment = VerticalAlignment.Top
            };

            var headerText = new TextBlock
            {
                Text = "Submit New Claim",
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            headerBorder.Child = headerText;
            Grid.SetRow(headerBorder, 0);

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Padding = new Thickness(25)
            };
            Grid.SetRow(scrollViewer, 1);

            var contentStackPanel = new StackPanel();

            var claimIdSection = CreateFormSection("Claim ID");
            var claimIdTextBox = new TextBox
            {
                Text = $"CLM{ClaimRepository.Claims.Count + 1:000}",
                Height = 40,
                FontSize = 14,
                IsEnabled = false,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f8f9fa")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dee2e6")),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#495057"))
            };
            claimIdSection.Children.Add(claimIdTextBox);
            contentStackPanel.Children.Add(claimIdSection);

            var contractSection = CreateFormSection("Contract Name");
            var contractComboBox = new ComboBox
            {
                Height = 40,
                FontSize = 14,
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dee2e6"))
            };
            contractComboBox.Items.Add("Software Development 101");
            contractComboBox.Items.Add("Mathematics 202");
            contractComboBox.Items.Add("Physics 301");
            contractComboBox.Items.Add("Engineering 401");
            contractComboBox.Items.Add("Data Structures 205");
            contractComboBox.Items.Add("Algorithms 310");
            contractComboBox.SelectedIndex = 0;

            contractComboBox.ItemContainerStyle = new Style(typeof(ComboBoxItem));
            contractComboBox.ItemContainerStyle.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(10)));
            contractComboBox.ItemContainerStyle.Setters.Add(new Setter(Control.FontSizeProperty, 14.0));

            contractSection.Children.Add(contractComboBox);
            contentStackPanel.Children.Add(contractSection);

            var amountSection = CreateFormSection("Amount (R)");
            var amountTextBox = new TextBox
            {
                Height = 40,
                FontSize = 14,
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dee2e6")),
                Padding = new Thickness(10, 0, 10, 0)
            };
            amountTextBox.PreviewTextInput += (s, e) =>
            {
                e.Handled = !char.IsDigit(e.Text, 0) && e.Text != ".";
            };

            var watermarkText = new TextBlock
            {
                Text = "Enter amount...",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6c757d")),
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(12, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false
            };

            amountTextBox.TextChanged += (s, e) =>
            {
                watermarkText.Visibility = string.IsNullOrEmpty(amountTextBox.Text) ?
                    Visibility.Visible : Visibility.Collapsed;
            };

            var amountGrid = new Grid();
            amountGrid.Children.Add(amountTextBox);
            amountGrid.Children.Add(watermarkText);
            amountSection.Children.Add(amountGrid);
            contentStackPanel.Children.Add(amountSection);

            var descriptionSection = CreateFormSection("Description");
            var descriptionTextBox = new TextBox
            {
                Height = 100,
                FontSize = 14,
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dee2e6")),
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Padding = new Thickness(10)
            };

            var descWatermark = new TextBlock
            {
                Text = "Enter claim description...",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6c757d")),
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(12, 8, 0, 0),
                IsHitTestVisible = false
            };

            descriptionTextBox.TextChanged += (s, e) =>
            {
                descWatermark.Visibility = string.IsNullOrEmpty(descriptionTextBox.Text) ?
                    Visibility.Visible : Visibility.Collapsed;
            };

            var descGrid = new Grid();
            descGrid.Children.Add(descriptionTextBox);
            descGrid.Children.Add(descWatermark);
            descriptionSection.Children.Add(descGrid);
            contentStackPanel.Children.Add(descriptionSection);

            var buttonsStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 25, 0, 0)
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6c757d")),
                Foreground = Brushes.White,
                Height = 40,
                Width = 100,
                FontSize = 14,
                FontWeight = FontWeights.Medium,
                Margin = new Thickness(0, 0, 15, 0),
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            cancelButton.Click += (s, e) =>
            {
                submitClaimWindow.Close();
            };

            var submitButton = new Button
            {
                Content = "Submit Claim",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#001a4d")),
                Foreground = Brushes.White,
                Height = 40,
                Width = 120,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            submitButton.Click += (s, args) =>
            {
                if (decimal.TryParse(amountTextBox.Text, out decimal amount) && amount > 0)
                {
                    var newClaim = new Claim
                    {
                        ClaimId = claimIdTextBox.Text,
                        ContractName = contractComboBox.SelectedItem?.ToString() ?? "Unknown Contract",
                        SubmittedDate = DateTime.Now,
                        Amount = amount,
                        Status = "Pending",
                        LecturerName = UserRepository.CurrentUser?.FullName ?? "Unknown",
                        Description = descriptionTextBox.Text
                    };

                    ClaimRepository.Claims.Add(newClaim);
                    // No need to call SaveClaims() manually - it's handled by CollectionChanged event
                    submitClaimWindow.Close();

                    MessageBox.Show("Claim submitted successfully!\n\nIt will now be reviewed by the program coordinator.",
                        "Claim Submitted",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Please enter a valid amount greater than 0.",
                        "Invalid Amount",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    amountTextBox.Focus();
                    amountTextBox.SelectAll();
                }
            };

            buttonsStackPanel.Children.Add(cancelButton);
            buttonsStackPanel.Children.Add(submitButton);
            contentStackPanel.Children.Add(buttonsStackPanel);

            scrollViewer.Content = contentStackPanel;

            mainGrid.Children.Add(headerBorder);
            mainGrid.Children.Add(scrollViewer);

            submitClaimWindow.Content = mainGrid;

            submitClaimWindow.Loaded += (s, e) => amountTextBox.Focus();
            submitClaimWindow.ShowDialog();
        }

        private StackPanel CreateFormSection(string title)
        {
            var section = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };

            var titleText = new TextBlock
            {
                Text = title,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#495057")),
                Margin = new Thickness(0, 0, 0, 8)
            };

            section.Children.Add(titleText);
            return section;
        }

        private void RetractClaim_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Claim claim)
            {
                if (claim.Status == "Pending")
                {
                    var result = MessageBox.Show($"Are you sure you want to retract claim {claim.ClaimId}?",
                        "Confirm Retract", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        ClaimRepository.Claims.Remove(claim);
                        MessageBox.Show("Claim retracted successfully.", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Only pending claims can be retracted.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ViewMyClaims_Click(object sender, RoutedEventArgs e)
        {
            ViewMyClaims();
        }

        private void ViewMyClaims()
        {
            var viewClaimsWindow = new Window4(UserRepository.CurrentUser?.FullName ?? "Unknown");
            viewClaimsWindow.Owner = this;
            viewClaimsWindow.ShowDialog();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
