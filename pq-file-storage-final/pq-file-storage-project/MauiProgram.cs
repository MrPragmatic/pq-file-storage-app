using DataAccessClassLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using pq_file_storage_project.Features.Register;
using pq_file_storage_project.Features.Login;
using pq_file_storage_project.Features.Otp;
using pq_file_storage_project.Features.Userspace;
using pq_file_storage_project.Pages;
using pq_file_storage_project.Services;
using pq_file_storage_project.SessionManager;
using Supabase;
using System.Reflection;

namespace pq_file_storage_project
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<RegisterView>();
            builder.Services.AddTransient<OtpView>();
            builder.Services.AddTransient<UserSpaceView>();

            builder.Services.AddTransient<RegisterFormViewModel>();
            builder.Services.AddTransient<LoginFormViewModel>();
            builder.Services.AddTransient<OtpFormViewModel>();
            builder.Services.AddTransient<UserSpaceViewModel>();

#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(windowsLifecycleBuilder =>
                {
                    windowsLifecycleBuilder.OnWindowCreated(window =>
                    {
                        window.ExtendsContentIntoTitleBar = false;
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

                        if (appWindow is not null)
                        {
                            Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(id, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
                            if (displayArea is not null)
                            {
                                var CenteredPosition = appWindow.Position;
                                CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                                CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                                appWindow.Move(CenteredPosition);
                            }
                        }
                    });
                });
            });


            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(windowsLifecycleBuilder =>
                {
                    windowsLifecycleBuilder.OnWindowCreated(window =>
                    {
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                        appWindow.Closing += async (s, e) =>
                        {
                            e.Cancel = true;

                            try
                            {
                                var mainPage = App.Current?.MainPage;
                                if (mainPage != null)
                                {
                                    bool result = await mainPage.DisplayAlert(
                                        "Exiting Lade",
                                        "Are you sure you want to log out and close the application?",
                                        "Yes",
                                        "Cancel");

                                    if (result)
                                    {
                                        var serviceProvider = builder.Services.BuildServiceProvider();
                                        var supabaseService = serviceProvider.GetService<SupabaseService>();
                                        if (supabaseService != null)
                                        {
                                            await supabaseService.SignOut();
                                            new LadeSessionHandler().DestroySession();
                                        }
                                        App.Current?.Quit();
                                    }
                                }
                                else
                                {
                                    // display error if main page is null
                                    await mainPage?.DisplayAlert("Error", "MainPage is null. Cannot display alert.", "OK");
                                }
                            }
                            catch (Exception ex)
                            {
                                // handle unexpected error
                                if (App.Current?.MainPage != null)
                                {
                                    await App.Current.MainPage.DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
                                }
                            }
                        };
                    });
                });
            });
#endif
#if DEBUG
            // Add logging
            builder.Logging.AddDebug();
#endif
            // Create Supabase service as a singleton.
            builder.Services.AddSingleton<SupabaseService>();

            builder.Services.AddSingleton<EmailService>();
            return builder.Build();
        }
    }
}