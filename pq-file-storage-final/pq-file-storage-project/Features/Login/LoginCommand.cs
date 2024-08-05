using Supabase;
using pq_file_storage_project.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pq_file_storage_project.Services;
using pq_file_storage_project.Pages;
using static Supabase.Gotrue.Constants;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel.Communication;
using Supabase.Interfaces;
using pq_file_storage_project.Features.Otp;
using System.Diagnostics;

namespace pq_file_storage_project.Features.Login
{
    // LoginCommand class is responsible for handling the login logic
    public class LoginCommand(LoginFormViewModel viewModel, SupabaseService supabaseService) : AsyncCommandBase
    {
        // Initialize private readonly view model and Supabase service
        private readonly LoginFormViewModel viewModel = viewModel;
        private readonly SupabaseService _supabaseService = supabaseService;

        // overriding protected asynchronous ExecuteAsync method to execute LoginCommand inherited from AsyncCommandBase
        protected override async Task ExecuteAsync(object? parameter)
        {
            // initialize Supabase client
            await _supabaseService.InitializeAsync();

            // checking if the email is empyty or not valid or whether the email is not valid
            if (!viewModel.IsValidEmail(viewModel.Email))
            {
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert("Error", "Email address is not valid. Please enter right email address.", "OK");
                }
                return;
            }
            
            // if the email is valid, send OTP code through the Supabase service
            bool OtpIsSent = await _supabaseService.SendOtpCode(viewModel.Email);
            // if OTP is sent, create a new OtpFormViewModel with user email and call OnSentOtpSuccess method
            if (OtpIsSent)
            {
                await viewModel.OnSentOtpSuccess();
            } else
            {
                // if OTP is not sent, display an alert with the message "Failed to send OTP code. Please try again."
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert("Error", "Failed to send OTP code. Please try again.", "OK");
                }
            }
        }
    }
}