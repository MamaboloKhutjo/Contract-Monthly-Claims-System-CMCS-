using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Contract_Monthly_Claims_System__CMCS_
{
    public static class UserRepository
    {
        private static List<User> _users = new List<User>();

        public static List<User> Users
        {
            get => _users;
            private set => _users = value;
        }

        public static User CurrentUser { get; set; }

        // Initialize users from file
        public static void InitializeUsers()
        {
            Users = ClaimsDataService.LoadUsers();
        }

        // Save users to file
        public static void SaveUsers()
        {
            ClaimsDataService.SaveUsers(Users);
        }

        public class User
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }

            [JsonIgnore]
            public string FullName => $"{Name} {Surname}";

            [JsonIgnore]
            public string FormalName => $"{Name} {Surname}";
        }
    }

    public static class ClaimRepository
    {
        private static ObservableCollection<Claim> _claims = new ObservableCollection<Claim>();

        public static ObservableCollection<Claim> Claims
        {
            get => _claims;
            private set => _claims = value;
        }

        static ClaimRepository()
        {// Load claims from file on initialization
            InitializeClaims();
        }

      
        

        // Initialize claims from file
        public static void InitializeClaims()
        {
            var loadedClaims = ClaimsDataService.LoadClaims();
            Claims.Clear();
            foreach (var claim in loadedClaims)
            {
                Claims.Add(claim);
            }
        }

        // Save claims to file
        public static void SaveClaims()
        {
            ClaimsDataService.SaveClaims(Claims);
        }

        // Update a claim (for status changes)
        public static void UpdateClaim(Claim updatedClaim)
        {
            var existingClaim = Claims.FirstOrDefault(c => c.ClaimId == updatedClaim.ClaimId);
            if (existingClaim != null)
            {
                existingClaim.Status = updatedClaim.Status;
                SaveClaims();
            }
        }
    }

    public class Claim : INotifyPropertyChanged
    {
        private string _status;

        public string ClaimId { get; set; }
        public string ContractName { get; set; }
        public DateTime SubmittedDate { get; set; }
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

        // For JSON serialization - computed properties need to be ignored
        [JsonIgnore]
        public string FormattedAmount => $"R {Amount:N2}";

        [JsonIgnore]
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
