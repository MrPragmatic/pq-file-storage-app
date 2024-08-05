using pq_file_storage_project.Pages;
using pq_file_storage_project.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pq_file_storage_project.SessionManager
{
    public class SessionRedirector
    {
        private readonly SupabaseService _supabaseService;

        public SessionRedirector(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task StayOrRedirectToLogin()
        {
            try
            {
                await _supabaseService.InitializeAsync();
                var session = _supabaseService.GetClient().Auth.CurrentSession;

                // get current page from shell
                var currentPage = Shell.Current.CurrentPage;

                if (session != null)
                {
                    // User is signed in do nothing
                    return;
                }
                else if (currentPage != null)
                {
                    // Do nothing as the user is on the login page already
                    return;
                }
                else
                {
                    var mainPage = Application.Current?.MainPage;
                    if (mainPage != null)
                    {
                        // display alert to user that they are not logged in and redirect to login page
                        await mainPage.DisplayAlert("Error", "You are not logged in. Redirecting to login page.", "OK");
                        // if session is null and user is not on login page, redirect to login page
                        await Shell.Current.GoToAsync(nameof(LoginView));
                    }
                }
            } catch (Exception ex)
            {
                // Handle exception
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    // display alert to user that they are not logged in and redirect to login page
                    await mainPage.DisplayAlert("Error", "You are not logged in. Redirecting to login page.", "OK");
                    // if session is null and user is not on login page, redirect to login page
                    await Shell.Current.GoToAsync(nameof(LoginView));
                }
            }
        }
    }
}
