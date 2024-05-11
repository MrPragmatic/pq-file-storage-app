using Firebase.Auth;
using pq_file_storage_project.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pq_file_storage_project.Features.Login
{
    public class LoginFormViewModel : ViewModelBase
    {
        private string _email;
        private string _password;

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

        public ICommand LoginCommand { get; }
        public LoginFormViewModel(FirebaseAuthClient authClient)
        {
            LoginCommand = new LoginCommand(this, authClient);
        }

    }
}
