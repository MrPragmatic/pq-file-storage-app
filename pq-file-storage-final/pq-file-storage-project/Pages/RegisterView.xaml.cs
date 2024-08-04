using pq_file_storage_project.Features.Register;

namespace pq_file_storage_project.Pages
{
    public partial class RegisterView : ContentPage
    {
        public RegisterView(RegisterFormViewModel rfvm)
        {
            InitializeComponent();
            BindingContext = rfvm;

        }

        // Event handler for Register link tapped
        private async void ToLoginView(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(LoginView));
        }
    }
}