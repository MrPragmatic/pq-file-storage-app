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
using Microsoft.Maui.ApplicationModel.Communication;
using Amazon.S3.Model;
using Microsoft.UI.Xaml.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.DirectoryServices.ActiveDirectory;

namespace pq_file_storage_project.Features.Login
{
    public class LoginFormViewModel : ViewModelBase
    {
        private string _email = string.Empty;
        private string _emailError = string.Empty;
        private readonly SupabaseService _supabaseService;
        private readonly EmailService _emailService;

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

        public string EmailError
        {
            get => _emailError;
            private set
            {
                _emailError = value;
                OnPropertyChanged(nameof(EmailError));
            }
        }

        public ICommand LoginCommand { get; }
        public new ICommand ExitCommand { get; }

        public LoginFormViewModel(SupabaseService supabaseService, EmailService emailService)
        {
            _supabaseService = supabaseService;
            _emailService = emailService;
            LoginCommand = new LoginCommand(this, supabaseService);
            ExitCommand = new Command(async () => await ExitCommand());
        }

        // validates null or emtry email and calls IsValidEmail() for other validation rules
        private void ValidateEmail()
        {
            if (string.IsNullOrEmpty(Email) || !IsValidEmail(Email))
            {
                EmailError = "Please enter a valid email address.";
            }
            else
            {
                EmailError = string.Empty;
            }
        }

        // checks if the provided email address is in a valid format using 
        public bool IsValidEmail(string email)
        {
            try
            {
                // checks for the presence of an "@" symbol, a valid domain, and other structural requirements
                var validEmailAddress = new System.Net.Mail.MailAddress(email);
                // If the email address is valid, Mailaddress type of object is returned stored in validEmailAddress
                return validEmailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
        // public asynchronous method to navigate to OTP view
        public async Task OnSentOtpSuccess()
        {
            _emailService.Email = Email;
            // Navigate to OTP view
            await Shell.Current.GoToAsync(nameof(OtpView));
        }
    }
}