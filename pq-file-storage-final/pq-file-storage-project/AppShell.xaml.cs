using pq_file_storage_project.Pages;

namespace pq_file_storage_project

{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
            Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
            Routing.RegisterRoute(nameof(OtpView), typeof(OtpView));
            Routing.RegisterRoute(nameof(UserSpaceView), typeof(UserSpaceView));
        }
    }
}