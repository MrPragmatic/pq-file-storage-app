using pq_file_storage_project.Services;
using pq_file_storage_project.SessionManager;
using System.Runtime.InteropServices;

namespace pq_file_storage_project
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            // Handle when the app starts, opens the app on the login view
            await Shell.Current.GoToAsync("//login");

            // calling session handler
            var supabaseService = new SupabaseService();
            var loginView = new SessionRedirector(supabaseService);
            await loginView.StayOrRedirectToLogin();

            base.OnStart();
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState) ?? throw new InvalidOperationException("Failed to create window.");
            window.Title = "Lade";
            const int newWidth = 1000;
            const int newHeight = 800;

            window.X = 100;
            window.Y = 200;

            window.Width = newWidth;
            window.Height = newHeight;

            window.MinimumHeight = newHeight;
            window.MinimumWidth = newWidth;

            return window;
        }
    }
}
