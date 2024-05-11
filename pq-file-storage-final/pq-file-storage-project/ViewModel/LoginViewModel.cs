using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.LifecycleEvents;
using pq_file_storage_project.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace pq_file_storage_project.ViewModel;

public partial class LoginViewModel : ObservableObject 
{

    [RelayCommand]
    async Task Register()
    {
        await Shell.Current.GoToAsync(nameof(RegisterView));
    }

    [RelayCommand]
    async Task ExitCommand()
    {
        bool result = await App.Current.MainPage.DisplayAlert(
        "Alert title",
        "Do you want to close app?",
        "Yes",
        "Cancel");
        if (result)
        {
            App.Current.Quit();
        }
    }

    [RelayCommand]
    async Task LoginCommand()
    {
        bool result = await App.Current.MainPage.DisplayAlert(
        "Alert title",
        "Do you want to close app?",
        "Yes",
        "Cancel");
        if (result)
        {
            App.Current.Quit();
        }
    }
}
