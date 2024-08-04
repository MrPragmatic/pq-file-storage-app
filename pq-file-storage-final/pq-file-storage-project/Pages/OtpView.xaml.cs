using pq_file_storage_project.Features.Login;
using pq_file_storage_project.Features.Otp;

namespace pq_file_storage_project.Pages;

public partial class OtpView : ContentPage
{
    public OtpView(OtpFormViewModel ofvm)
    {
        InitializeComponent();
        // Assign the instance of OtpFormViewModel to BindingContext
        BindingContext = ofvm;
    }

    // Event handler for OTP link tapped
    private async void ToLoginView(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(LoginView));
    }
}