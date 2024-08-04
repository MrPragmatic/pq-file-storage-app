using Supabase;
using pq_file_storage_project.Shared.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pq_file_storage_project.Services;
using System.Diagnostics;

namespace pq_file_storage_project.Features.Register
{
    // RegisterCommand class that inherits from the AsyncCommandBase class
    public class RegisterCommand : AsyncCommandBase
    {
        // private radonly variables for the RegisterFormViewModel and SupabaseService
        private readonly RegisterFormViewModel viewModel;
        private readonly SupabaseService _supabaseService;

        // public constructor for the RegisterCommand class that takes
        // registerFormViewModel and SupabaseService as parameters
        public RegisterCommand(RegisterFormViewModel viewModel, SupabaseService supabaseService)
        {
            // set the private variables to the values passed in the constructor
            _supabaseService = supabaseService;
            this.viewModel = viewModel;
        }

        public async Task ExecuteAsyncWrapper(object? parameter)
        {
            await ExecuteAsync(parameter);
        }

        // protected function to execute async task taking optional object parameter
        // which can pass data from UI (view) to viewmodel, for example registration data
        protected override async Task ExecuteAsync(object? parameter)
        {
            // if the main page exists, get it
            var mainPage = Application.Current?.MainPage;
            // Check if passwords match in the password and confirm password fields
            if (viewModel.Password != viewModel.ConfirmPassword)
            {
                // if the main page exists, display an alert
                if (mainPage != null)
                {
                    // display an alert when the passwords do not match
                    await mainPage.DisplayAlert("Error", "Passwords do not match", "OK");
                }
                // if the passwords do not match, exit the method
                return;
            }

            // validate email format of the user entered email address
            var emailValidationResult = ValidateEmail(viewModel.Email);
            // check if the email is not valid
            if (!emailValidationResult.isValid)
            {
                // if the main page exists, display an alert
                if (mainPage != null)
                {
                    // display error message
                    await mainPage.DisplayAlert("Error", emailValidationResult.errorMessage, "OK");
                }
                return;
            }

            try
            {
                // attempt to initialize Supabase client
                await _supabaseService.InitializeAsync();

                // register user
                await _supabaseService.SignUpAsync(viewModel.Email, viewModel.Password);

                // Handle success
                if (mainPage != null)
                {
                    // display an alert when the registration is successful
                    await mainPage.DisplayAlert("Success", "Registered successfully! Check your email for confirmation link and confirm your user account.", "OK");
                }

                // Clear fields after successful registration
                viewModel.Email = string.Empty;
                viewModel.Password = string.Empty;
                viewModel.ConfirmPassword = string.Empty;
            }
            catch (Exception ex)
            {
                // Handle exception
                if (mainPage != null)
                {
                    // display an alert when the registration fails
                    await mainPage.DisplayAlert("Error", $"Failed to sign up. Please try again. {ex.Message}", "OK");
                }
            }
        }

        // private function to validate email
        private (bool isValid, string errorMessage) ValidateEmail(string email)
        {
            // checking if the entered email is null or empty
            if (string.IsNullOrWhiteSpace(email))
            {
                // return false and error message
                return (false, "Email is required.");
            }

            // check if the email is in a valid email format
            var emailAttribute = new EmailAddressAttribute();
            
            // checking if the email is not in a valid email format
            if (!emailAttribute.IsValid(email))
            {
                // return false and error message
                return (false, "The email you entered is not a valid e-mail address. Please write a valid email to register.");
            }
            // return true and empty string
            return (true, string.Empty);
        }
    }
}