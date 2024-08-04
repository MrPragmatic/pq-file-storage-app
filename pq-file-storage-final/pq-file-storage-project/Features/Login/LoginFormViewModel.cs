using Supabase;
using pq_file_storage_project.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using pq_file_storage_project.Services;
using pq_file_storage_project.Pages;
using pq_file_storage_project.Features.Otp;

namespace pq_file_storage_project.Features.Login
{
    public class LoginFormViewModel : ViewModelBase
    {
        private string _email = string.Empty;

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public ICommand LoginCommand { get; }
        public new ICommand ExitCommand { get; }

        public LoginFormViewModel(SupabaseService supabaseService)
        {
            LoginCommand = new LoginCommand(this, supabaseService);
            ExitCommand = new Command(async () => await ExitCommand());
        }

        // public asynchronous method to navigate to OTP view
        public async Task OnSentOtpSuccess()
        {
            // Navigate to OTP view
            await Shell.Current.GoToAsync(nameof(OtpView));
        }

    }
}