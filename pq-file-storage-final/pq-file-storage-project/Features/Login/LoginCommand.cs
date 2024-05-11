using Firebase.Auth;
using pq_file_storage_project.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pq_file_storage_project.Features.Login
{
    public class LoginCommand : AsyncCommandBase
    {
        private readonly LoginFormViewModel viewModel;
        private readonly FirebaseAuthClient _authClient;

        public LoginCommand(LoginFormViewModel viewModel, FirebaseAuthClient authClient)
        {
            _authClient = authClient;
            this.viewModel = viewModel;
        }
        protected override async Task ExecuteAsync(object parameter)
        {
            try
            {
                // authenticate user
                await _authClient.SignInWithEmailAndPasswordAsync(viewModel.Email, viewModel.Password);
                // Handle success
                await Application.Current.MainPage.DisplayAlert("Success", "Logged in successfully!", "OK");
            }
            catch (Exception ex)
            {
                // Handle exception
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to log in. Please try again.", "OK");
            }        }
    }
}
