using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using pq_file_storage_project.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pq_file_storage_project.ViewModel;

public partial class RegisterViewModel : ObservableObject
{

    [RelayCommand]
    async Task ToLoginView()
    {
        await Shell.Current.GoToAsync(nameof(LoginView));
    }
}