using System.Configuration;
using System.Data;
using System.Windows;

namespace Contract_Monthly_Claims_System__CMCS_
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize users and claims from local storage
            UserRepository.InitializeUsers();
            ClaimRepository.InitializeClaims();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Ensure all data is saved before exit
            UserRepository.SaveUsers();
            ClaimRepository.SaveClaims();

            base.OnExit(e);
        }

    }

}
