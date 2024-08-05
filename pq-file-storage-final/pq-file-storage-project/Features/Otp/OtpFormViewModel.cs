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
using Microsoft.Maui.ApplicationModel.Communication;
using pq_file_storage_project.Features.Login;

namespace pq_file_storage_project.Features.Otp
{
    public class OtpFormViewModel : ViewModelBase
    {
        private string _email = string.Empty;
        private string token = string.Empty;
        private readonly EmailService _emailService;
        private readonly SupabaseService _supabaseService;

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

        public string Email => _emailService.Email;

        public ICommand VerifyOtpCommand { get; }
        public new ICommand ExitCommand { get; }
        public OtpFormViewModel(SupabaseService supabaseService, EmailService emailService)
        {
            VerifyOtpCommand = new VerifyOtpCommand(this, supabaseService);
            ExitCommand = new Command(async () => await ExitCommand());
            _emailService = emailService;
        }

        public async Task OnVerifiedOtpSuccess()
        {
            // Navigate to OTP view
            await Shell.Current.GoToAsync(nameof(UserSpaceView));
        }
    }
}