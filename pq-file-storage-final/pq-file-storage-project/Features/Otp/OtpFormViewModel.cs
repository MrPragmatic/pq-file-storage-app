using Supabase;
using pq_file_storage_project.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using pq_file_storage_project.Pages;
using pq_file_storage_project.Services;
using Windows.Media.AppBroadcasting;

namespace pq_file_storage_project.Features.Otp
{
    public class OtpFormViewModel : ViewModelBase
    {
        private string _email = string.Empty;
        private string token = string.Empty;

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

        public string TOKEN
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
                OnPropertyChanged(nameof(TOKEN));
            }
        }

        public ICommand VerifyOtpCommand { get; }
        public new ICommand ExitCommand { get; }
        public OtpFormViewModel(SupabaseService supabaseService)
        {
            VerifyOtpCommand = new VerifyOtpCommand(this, supabaseService);
            ExitCommand = new Command(async () => await ExitCommand());
        }

        public async Task OnVerifiedOtpSuccess()
        {
            // Navigate to OTP view
            await Shell.Current.GoToAsync(nameof(UserSpaceView));
        }

    }
}