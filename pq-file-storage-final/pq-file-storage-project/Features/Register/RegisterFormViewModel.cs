using Supabase;
using pq_file_storage_project.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using pq_file_storage_project.Services;

namespace pq_file_storage_project.Features.Register
{
    public class RegisterFormViewModel : ViewModelBase
    {
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;

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

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ConfirmPassword
        {
            get
            {
                return _confirmPassword;
            }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public ICommand RegisterCommand { get; }
        public new ICommand ExitCommand { get; }
        public RegisterFormViewModel(SupabaseService supabaseService)
        {
            RegisterCommand = new RegisterCommand(this, supabaseService);
            ExitCommand = new Command(async () => await ExitCommand());
        }
    }
}