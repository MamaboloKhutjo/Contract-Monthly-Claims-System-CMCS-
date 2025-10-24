using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private ClaimsSummary _claimsSummary;
        private ObservableCollection<Claim> _pendingClaims;
        private string _currentUserName;

        public ClaimsSummary ClaimsSummary
        {
            get => _claimsSummary;
            set
            {
                _claimsSummary = value;
                OnPropertyChanged(nameof(ClaimsSummary));
            }
        }

        public ObservableCollection<Claim> PendingClaims
        {
            get => _pendingClaims;
            set
            {
                _pendingClaims = value;
                OnPropertyChanged(nameof(PendingClaims));
            }
        }

        public string CurrentUserName
        {
            get => _currentUserName;
            set
            {
                _currentUserName = value;
                OnPropertyChanged(nameof(CurrentUserName));
            }
        }

        public Window1()
        {
            InitializeComponent();

            // Set the current user name
            if (UserRepository.CurrentUser != null)
            {
                CurrentUserName = UserRepository.CurrentUser.FormalName;
            }
            else
            {
                CurrentUserName = "Manager"; // Fallback if no user is logged in
            }

            InitializeDashboard();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();   
            mainWindow.Show();
            this.Close();   
        }

        private void InitializeDashboard()
        {
            // Initialize claims summary
            ClaimsSummary = new ClaimsSummary();

            // Initialize pending claims collection
            PendingClaims = new ObservableCollection<Claim>();

            // Load data
            LoadClaimsData();

            // Set data context for binding
            this.DataContext = this;

            // Subscribe to claims changes
            ClaimRepository.Claims.CollectionChanged += (s, e) => LoadClaimsData();
        }

        private void LoadClaimsData()
        {
            // Update claims summary
            ClaimsSummary.UpdateSummary(ClaimRepository.Claims);

            // Update pending claims
            var pending = ClaimRepository.Claims
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            PendingClaims.Clear();
            foreach (var claim in pending)
            {
                PendingClaims.Add(claim);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        // Approve claim
        private void ApproveClaim_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Claim claim)
            {
                var result = MessageBox.Show($"Are you sure you want to APPROVE claim {claim.ClaimId}?",
                    "Confirm Approval", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    claim.Status = "Approved";
                    LoadClaimsData(); 

                    MessageBox.Show($"Claim {claim.ClaimId} has been approved successfully.",
                        "Approval Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void RejectClaim_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Claim claim)
            {
                var result = MessageBox.Show($"Are you sure you want to REJECT claim {claim.ClaimId}?",
                    "Confirm Rejection", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    claim.Status = "Rejected";
                    LoadClaimsData();

                    MessageBox.Show($"Claim {claim.ClaimId} has been rejected.",
                        "Rejection Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ViewClaim_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Claim claim)
            {
                ShowClaimDetails(claim);
            }
        }

        private void ShowClaimDetails(Claim claim)
        {
            var detailWindow = new Window()
            {
                Title = $"Claim Details - {claim.ClaimId}",
                Width = 600, // Increased width for attachments
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.CanResize,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F7F9"))
            };

            var mainGrid = new Grid { Margin = new Thickness(0) };
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(70) });

            // Header
            var headerBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#001a4d")),
                Height = 60
            };

            var headerText = new TextBlock
            {
                Text = $"Claim Details - {claim.ClaimId}",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            headerBorder.Child = headerText;
            Grid.SetRow(headerBorder, 0);

            // Content
            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Padding = new Thickness(20)
            };
            Grid.SetRow(scrollViewer, 1);

            var contentStack = new StackPanel();

            // Claim details
            var details = new[]
            {
        new { Label = "Claim ID:", Value = claim.ClaimId },
        new { Label = "Lecturer:", Value = claim.LecturerName },
        new { Label = "Contract:", Value = claim.ContractName },
        new { Label = "Submitted:", Value = claim.SubmittedDate.ToString("dd/MM/yyyy HH:mm") },
        new { Label = "Amount:", Value = claim.FormattedAmount },
        new { Label = "Status:", Value = claim.Status },
        new { Label = "Attachments:", Value = claim.AttachmentSummary }
    };

            foreach (var detail in details)
            {
                var detailGrid = new Grid();
                detailGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
                detailGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                detailGrid.Margin = new Thickness(0, 0, 0, 10);

                var label = new TextBlock
                {
                    Text = detail.Label,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"))
                };
                Grid.SetColumn(label, 0);

                var value = new TextBlock
                {
                    Text = detail.Value,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666"))
                };
                Grid.SetColumn(value, 1);

                detailGrid.Children.Add(label);
                detailGrid.Children.Add(value);
                contentStack.Children.Add(detailGrid);
            }

            // Description
            var descLabel = new TextBlock
            {
                Text = "Description:",
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                Margin = new Thickness(0, 10, 0, 5)
            };
            contentStack.Children.Add(descLabel);

            var descBox = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD")),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(0, 0, 0, 15)
            };

            var descText = new TextBlock
            {
                Text = string.IsNullOrEmpty(claim.Description) ? "No description provided." : claim.Description,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666"))
            };

            descBox.Child = descText;
            contentStack.Children.Add(descBox);

            // Attachments Section
            if (claim.HasAttachments)
            {
                var attachmentsLabel = new TextBlock
                {
                    Text = "Attached Files:",
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                    Margin = new Thickness(0, 10, 0, 10)
                };
                contentStack.Children.Add(attachmentsLabel);

                foreach (var attachment in claim.Attachments)
                {
                    var attachmentBorder = new Border
                    {
                        Background = Brushes.White,
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD")),
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(10),
                        CornerRadius = new CornerRadius(4),
                        Margin = new Thickness(0, 0, 0, 5)
                    };

                    var attachmentGrid = new Grid();
                    attachmentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    attachmentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

                    var fileInfoText = new TextBlock
                    {
                        Text = $"{attachment.FileIcon} {attachment.FileName} ({attachment.FormattedFileSize})",
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666")),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetColumn(fileInfoText, 0);

                    var downloadButton = new Button
                    {
                        Content = "📥 Download",
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#001a4d")),
                        Foreground = Brushes.White,
 
                        FontSize = 11,
                        FontWeight = FontWeights.Medium,
                        Tag = attachment,
                        Cursor = Cursors.Hand
                    };
                    downloadButton.Click += (s, e) =>
                    {
                        DownloadAttachment(attachment);
                    };
                    Grid.SetColumn(downloadButton, 1);

                    attachmentGrid.Children.Add(fileInfoText);
                    attachmentGrid.Children.Add(downloadButton);
                    attachmentBorder.Child = attachmentGrid;
                    contentStack.Children.Add(attachmentBorder);
                }

                // Add download all button
                var downloadAllButton = new Button
                {
                    Content = "📦 Download All Files as ZIP",
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a86b")),
                    Foreground = Brushes.White,
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 15, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Cursor = Cursors.Hand
                };
                downloadAllButton.Click += (s, e) =>
                {
                    DownloadAllAttachments(claim);
                };
                contentStack.Children.Add(downloadAllButton);
            }

            scrollViewer.Content = contentStack;

            // Footer with Close button
            var footerBorder = new Border
            {
                Background = Brushes.White,
                BorderThickness = new Thickness(1, 0, 0, 0),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD"))
            };
            Grid.SetRow(footerBorder, 2);

            var closeButton = new Button
            {
                Content = "Close",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6c757d")),
                Foreground = Brushes.White,
                Width = 80,
                Height = 35,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 20, 0)
            };
            closeButton.Click += (s, e) => detailWindow.Close();

            footerBorder.Child = closeButton;

            mainGrid.Children.Add(headerBorder);
            mainGrid.Children.Add(scrollViewer);
            mainGrid.Children.Add(footerBorder);

            detailWindow.Content = mainGrid;
            detailWindow.ShowDialog();
        }

        // Add these new methods for file downloads
        private void DownloadAttachment(ClaimAttachment attachment)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = attachment.FileName,
                Filter = $"All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllBytes(saveFileDialog.FileName, attachment.FileData);
                    MessageBox.Show($"File '{attachment.FileName}' downloaded successfully!", "Download Complete",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error downloading file: {ex.Message}", "Download Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void DownloadAllAttachments(Claim claim)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"Claim_{claim.ClaimId}_Attachments.zip",
                Filter = "ZIP files (*.zip)|*.zip"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                        {
                            foreach (var attachment in claim.Attachments)
                            {
                                var entry = archive.CreateEntry(attachment.FileName, System.IO.Compression.CompressionLevel.Optimal);
                                using (var entryStream = entry.Open())
                                {
                                    entryStream.Write(attachment.FileData, 0, attachment.FileData.Length);
                                }
                            }
                        }

                        File.WriteAllBytes(saveFileDialog.FileName, memoryStream.ToArray());
                    }

                    MessageBox.Show($"All {claim.Attachments.Count} files downloaded successfully as ZIP!", "Download Complete",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating ZIP file: {ex.Message}", "Download Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        

        private void ViewAllPendingClaims_Click(object sender, RoutedEventArgs e)
        {
            var allPendingClaims = ClaimRepository.Claims
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            if (!allPendingClaims.Any())
            {
                MessageBox.Show("There are no pending claims to display.", "No Pending Claims",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ShowAllPendingClaimsWindow(allPendingClaims);
        }

        private void ShowAllPendingClaimsWindow(List<Claim> claims)
        {
            var window = new Window
            {
                Title = "All Pending Claims",
                Width = 900,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var header = new TextBlock
            {
                Text = $"All Pending Claims ({claims.Count})",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(20),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#001a4d"))
            };
            Grid.SetRow(header, 0);

            var dataGrid = new DataGrid
            {
                ItemsSource = claims,
                AutoGenerateColumns = false,
                IsReadOnly = true,
                Margin = new Thickness(20, 0, 20, 0)
            };
            Grid.SetRow(dataGrid, 1);

            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Claim ID", Binding = new Binding("ClaimId"), Width = 100 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Lecturer", Binding = new Binding("LecturerName"), Width = 150 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Contract", Binding = new Binding("ContractName"), Width = 200 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Submitted", Binding = new Binding("FormattedDate"), Width = 120 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Amount", Binding = new Binding("FormattedAmount"), Width = 120 });

            var closeButton = new Button
            {
                Content = "Close",
                Width = 80,
                Height = 35,
                Margin = new Thickness(0, 10, 20, 20),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            closeButton.Click += (s, e) => window.Close();
            Grid.SetRow(closeButton, 2);

            grid.Children.Add(header);
            grid.Children.Add(dataGrid);
            grid.Children.Add(closeButton);

            window.Content = grid;
            window.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
        MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                UserRepository.CurrentUser = null;
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}
