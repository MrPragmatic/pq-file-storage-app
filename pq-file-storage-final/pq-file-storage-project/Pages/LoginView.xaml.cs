using pq_file_storage_project.Features.Login;

namespace pq_file_storage_project.Pages
{
    public partial class LoginView : ContentPage
    {

        public LoginView(LoginFormViewModel lfvm)
        {
            InitializeComponent();

            // Assign the instance of LoginFormViewModel to BindingContext
            BindingContext = lfvm;
        }

        // Event handler for Register link tapped
        private async void ToRegisterView(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegisterView));
        }
    }

}
