using Supabase;
using pq_file_storage_project.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pq_file_storage_project.Services;

namespace pq_file_storage_project.Features.Otp
{
    public class VerifyOtpCommand(OtpFormViewModel viewModel, SupabaseService supabaseService) : AsyncCommandBase
    {
        private readonly OtpFormViewModel viewModel = viewModel;
        private readonly SupabaseService _supabaseService = supabaseService;

        protected override async Task ExecuteAsync(object? parameter)
        {
            // initialize Supabase client
            await _supabaseService.InitializeAsync();
            try
            {
                // verify OTP code and login
                if (await _supabaseService.CheckOtpCode(viewModel.Email, viewModel.TOKEN) != null)
                {
                    await viewModel.OnVerifiedOtpSuccess();
                }
                else
                {
                    var session = _supabaseService.CheckUserSession();
                    var mainPage = Application.Current?.MainPage;
                    if (mainPage != null)
                    {
                        await mainPage.DisplayAlert("Error", "Failed to send OTP. Please try again.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert("Error", $"Failed to verify OTP. Please try again. {ex.Message}", "OK");
                }
            }
        }
    }
}