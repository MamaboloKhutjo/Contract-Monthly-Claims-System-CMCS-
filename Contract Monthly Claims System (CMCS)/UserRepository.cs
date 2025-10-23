using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract_Monthly_Claims_System__CMCS_
{
    public static class UserRepository
    {
        public static List<User> Users { get; } = new List<User>();
        public static User CurrentUser { get; set; }

        public class User
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }

            public string FullName => $"{Name} {Surname}";
            public string FormalName => $"{Name} {Surname}";
        }
    }

    public static class ClaimRepository
    {
        public static ObservableCollection<Claim> Claims { get; } = new ObservableCollection<Claim>();

        static ClaimRepository()
        {
            // Initialize with empty claims
        }
    }

    public class Claim : INotifyPropertyChanged
    {
        private string _status;

        public string ClaimId { get; set; }
        public string ContractName { get; set; }
        public System.DateTime SubmittedDate { get; set; }
        public decimal Amount { get; set; }
        public string LecturerName { get; set; }
        public string Description { get; set; }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string FormattedAmount => $"R {Amount:N2}";
        public string FormattedDate => SubmittedDate.ToString("dd/MM/yyyy");

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ClaimsSummary : INotifyPropertyChanged
    {
        private int _totalClaims;
        private int _pendingClaims;
        private int _approvedClaims;
        private int _rejectedClaims;
        private decimal _approvedAmount;

        public int TotalClaims
        {
            get => _totalClaims;
            set { _totalClaims = value; OnPropertyChanged(nameof(TotalClaims)); }
        }

        public int PendingClaims
        {
            get => _pendingClaims;
            set { _pendingClaims = value; OnPropertyChanged(nameof(PendingClaims)); }
        }

        public int ApprovedClaims
        {
            get => _approvedClaims;
            set { _approvedClaims = value; OnPropertyChanged(nameof(ApprovedClaims)); }
        }

        public int RejectedClaims
        {
            get => _rejectedClaims;
            set { _rejectedClaims = value; OnPropertyChanged(nameof(RejectedClaims)); }
        }

        public decimal ApprovedAmount
        {
            get => _approvedAmount;
            set { _approvedAmount = value; OnPropertyChanged(nameof(ApprovedAmount)); }
        }

        public string FormattedApprovedAmount => $"R {ApprovedAmount:N2}";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateSummary(System.Collections.ObjectModel.ObservableCollection<Claim> claims, string lecturerName = null)
        {
            var userClaims = claims;

            if (!string.IsNullOrEmpty(lecturerName))
            {
                userClaims = new System.Collections.ObjectModel.ObservableCollection<Claim>(
                    claims.Where(c => c.LecturerName == lecturerName));
            }

            TotalClaims = userClaims.Count;
            PendingClaims = userClaims.Count(c => c.Status == "Pending");
            ApprovedClaims = userClaims.Count(c => c.Status == "Approved");
            RejectedClaims = userClaims.Count(c => c.Status == "Rejected");
            ApprovedAmount = userClaims.Where(c => c.Status == "Approved").Sum(c => c.Amount);
        }
    }
}
