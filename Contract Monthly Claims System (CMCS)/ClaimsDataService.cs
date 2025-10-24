using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace Contract_Monthly_Claims_System__CMCS_
{
    public static class ClaimsDataService
    {
        private static readonly string DataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CMCS");

        private static readonly string ClaimsFilePath = Path.Combine(DataFolder, "claims.json");
        private static readonly string UsersFilePath = Path.Combine(DataFolder, "users.json");

        static ClaimsDataService()
        {
            // Ensure data directory exists
            if (!Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }
        }
        // JSON serialization options
        private static JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        // Save claims to file
        public static void SaveClaims(ObservableCollection<Claim> claims)
        {
            try
            {
                var claimsList = new List<Claim>(claims);
                string json = JsonSerializer.Serialize(claimsList, GetJsonOptions());
                File.WriteAllText(ClaimsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving claims: {ex.Message}", "Save Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Load claims from file
        public static List<Claim> LoadClaims()
        {
            try
            {
                if (File.Exists(ClaimsFilePath))
                {
                    string json = File.ReadAllText(ClaimsFilePath);
                    var claims = JsonSerializer.Deserialize<List<Claim>>(json, GetJsonOptions());
                    return claims ?? new List<Claim>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading claims: {ex.Message}", "Load Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return new List<Claim>();
        }

        // Save users to file
        public static void SaveUsers(List<UserRepository.User> users)
        {
            try
            {
                string json = JsonSerializer.Serialize(users, GetJsonOptions());
                File.WriteAllText(UsersFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving users: {ex.Message}", "Save Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Load users from file
        public static List<UserRepository.User> LoadUsers()
        {
            try
            {
                if (File.Exists(UsersFilePath))
                {
                    string json = File.ReadAllText(UsersFilePath);
                    var users = JsonSerializer.Deserialize<List<UserRepository.User>>(json, GetJsonOptions());
                    return users ?? new List<UserRepository.User>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Load Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return new List<UserRepository.User>();
        }
    }
}
