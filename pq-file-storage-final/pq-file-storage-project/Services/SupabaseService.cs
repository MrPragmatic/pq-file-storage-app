using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase;
using Supabase.Gotrue;
using DataAccessClassLibrary.Models;
using static Supabase.Gotrue.Constants;
using Supabase.Gotrue.Interfaces;
using Supabase.Interfaces;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using pq_file_storage_project.Features.Login;
using Supabase.Postgrest;
using Microsoft.Maui.ApplicationModel.Communication;
using System.Security.Cryptography;
using System.Reflection.Metadata;
using static Supabase.Postgrest.Constants;
using Microsoft.Extensions.Configuration;

namespace pq_file_storage_project.Services
{
    public class SupabaseService
    {
        private readonly Supabase.Client _supabaseClient;

        public SupabaseService()
        {
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
            };

            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            var supabaseUrl = configuration["Supabase:Url"];
            var supabaseKey = configuration["Supabase:Key"];

            if (string.IsNullOrEmpty(supabaseUrl))
            {
                throw new ArgumentNullException(nameof(supabaseUrl), "Supabase URL environment variable is not set.");
            }

            if (string.IsNullOrEmpty(supabaseKey))
            {
                throw new ArgumentNullException(nameof(supabaseKey), "Supabase Key environment variable is not set.");
            }

            _supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey, options);
        }

        public async Task InitializeAsync()
        {
            await _supabaseClient.InitializeAsync();
        }

        public async Task AfterInitialSignUpListenToAuthEvents()
        {
            var session = _supabaseClient.Auth.CurrentSession;
            var user =  _supabaseClient.Auth.CurrentUser;
            _supabaseClient.Auth.AddStateChangedListener((user, session) =>
            {
                switch (session)
                {
                    case AuthState.SignedIn:
                        break;
                    case AuthState.SignedOut:
                        break;
                    case AuthState.UserUpdated:
                        break;
                    case AuthState.PasswordRecovery:
                        break;
                    case AuthState.TokenRefreshed:
                        break;
                }
            });
        }

        public async Task<bool> SendOtpCode(string email)
        {   
            try
            {
                var result = await _supabaseClient
                    .From<DataAccessClassLibrary.Models.Profile>()
                    .Select("*")
                    .Filter("email", Operator.Equals, email)
                    .Where(x => x.EmailConfirmedAt == null)
                    .Get();

                if (result != null)
                {
                    await _supabaseClient.Auth.SendMagicLink(email);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert("Error", $"Failed to send OTP. Please try again. {ex.Message}", "OK");
                }
                else
                {
                    // Fallback logging or notification if DisplayAlert cannot be used
                    Console.WriteLine($"Failed to send OTP. Please try again. {ex.Message}");
                }
                return false;
            }
        }

        public async Task<Session?> CheckOtpCode(string email, string TOKEN)
        {
            var session = await _supabaseClient.Auth.VerifyOTP("michaelgreed2024@gmail.com", TOKEN, EmailOtpType.Email);
            return session;
        }

        public async Task CheckUserSession()
        {
            var session = _supabaseClient.Auth.CurrentSession;
            if (session != null)
            {
                // User is signed in, do nothing
            }
            else
            {
                // User is signed out because the session has expired or user internet connection has been lost
                // Logic to return to login page
            }
        }

        // public asynchronous function to log out the user from the system using Supabase Auth service
        public async Task SignOut()
        {
            try
            {
                // call Supabase Auth.SignOut method to log out the user
                await _supabaseClient.Auth.SignOut();
            }
            // catch exception if the sign out fails
            catch (Exception ex)
            {
                // display an alert to the user if the sign out fails
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    // display an alert to the user if the sign out fails
                    await mainPage.DisplayAlert("Error", $"Failed to sign out. Please try again. {ex.Message}", "OK");
                }
            }
        }

        // asynchronous public function to sign up a new user to Supabase PostgreSQL database
        public async Task SignUpAsync(string email, string password)
        {
            try
            {
                // attempt to call Supabase Auth.SignUp method to create a new user
                await _supabaseClient.Auth.SignUp(email, password);
            }
            // catch exception if the user already exists in the system
            catch (Exception ex)
            {
                // if the exception message contains the string "23505", the user already exists in the system
                if(ex.Message.Contains("23505")){
                    throw new Exception($"The user exists in the system already - try again.");
                }else
                {
                    // in any other error scenario, throw a generic exception to the end user
                    throw new Exception($"Failed to create a new user.");
                }
            }
        }
    }
}
