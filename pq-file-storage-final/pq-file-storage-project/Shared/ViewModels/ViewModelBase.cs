using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pq_file_storage_project.Services;
using pq_file_storage_project.SessionManager;

namespace pq_file_storage_project.Shared.ViewModels

// Implemented following tutorial by Singleton Sean: https://youtu.be/adk9RonuKw0
{
    // This class is a base class for all view models
    public class ViewModelBase : INotifyPropertyChanged
    {
        // This event is raised when a property is changed
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly SupabaseService? _supabaseService;

        // This method is called when a property is changed
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task ExitCommand()
        {
            var mainPage = Application.Current?.MainPage;
            if (mainPage == null)
            {
                // Handle the case where MainPage is null, if necessary
                return;
            }

            bool result = await mainPage.DisplayAlert(
                "Exiting Lade",
                "Do you want to log out and close the app?",
                "Yes",
                "Cancel");

            if (result && _supabaseService != null)
            {
                await _supabaseService.SignOut();
                new LadeSessionHandler().DestroySession();
                Application.Current?.Quit();
            }
        }
    }
}
