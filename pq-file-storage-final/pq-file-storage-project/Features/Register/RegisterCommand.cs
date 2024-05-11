using Firebase.Auth;
using pq_file_storage_project.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pq_file_storage_project.Features.Register
{
    public class RegisterCommand : AsyncCommandBase
    {
        private readonly RegisterFormViewModel viewModel;
        private readonly FirebaseAuthClient _authClient;

        public RegisterCommand(RegisterFormViewModel viewModel, FirebaseAuthClient authClient)
        {
            _authClient = authClient;
            this.viewModel = viewModel;
        }
        protected override async Task ExecuteAsync(object parameter)
        {
            // Check if passwords match
            if (viewModel.Password != viewModel.ConfirmPassword)
            {
                // Error handling
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            try
            {
                // authenticate user
                await _authClient.CreateUserWithEmailAndPasswordAsync(viewModel.Email, viewModel.Password);
                // Handle success
                await Application.Current.MainPage.DisplayAlert("Success", "Registered successfully!", "OK");
            }
            catch (Exception ex)
            {
                // Handle exception
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to sign up. Please try again.", "OK");
            }        }
    }
}
