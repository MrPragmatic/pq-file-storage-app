using pq_file_storage_project.Features.Userspace;
using pq_file_storage_project.Services;

namespace pq_file_storage_project.Pages
{
    public partial class UserSpaceView : ContentPage
    {
        public UserSpaceView(UserSpaceViewModel usvm)
        {
            InitializeComponent();
            // Assign the instance of UserSpaceViewModel to BindingContext
            BindingContext = new UserSpaceViewModel(new SupabaseService());
        }

        // Event handler for OTP link tapped
        private async void ToLoginView(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(LoginView));
        }
    }
}