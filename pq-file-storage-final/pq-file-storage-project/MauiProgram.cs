using DataAccessClassLibrary;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using pq_file_storage_project.Features.Login;
using pq_file_storage_project.Features.Register;
using pq_file_storage_project.Pages;
using pq_file_storage_project.ViewModel;

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
            builder.Services.AddTransient<LoginViewModel>();

            builder.Services.AddTransient<RegisterView>();
            builder.Services.AddTransient<RegisterViewModel>();

            builder.Services.AddTransient<LoginFormViewModel>();
            builder.Services.AddTransient<RegisterFormViewModel>();

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
                            bool result = await App.Current.MainPage.DisplayAlert(
                                "Exiting Lade",
                                "Are you sure you want to close the application?",
                                "Yes",
                                "Cancel");

                            if (result)
                            {
                                App.Current.Quit();
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
            // Add database context
            builder.Services.AddDbContext<ApplicationDbContext>(
                // Use the PostgreSQL provider to store the data
                options => options.UseNpgsql($"Filename={GetDatabasePath()}", x => x.MigrationsAssembly(nameof(DataAccessClassLibrary))));

            // Add Firebase authentication
            builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = "FIREBASE_API_KEY",
                AuthDomain = "FIREBASE_AUTH_DOMAIN",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                    {
                        
                    }
                }
            }));
            
            return builder.Build();
        }

        public static string GetDatabasePath()
        {
            var databasePath = "POSTGRES_DB_PATH";
            var databaseName = "POSTGRES_DB_NAME";

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                databasePath = Path.Combine(FileSystem.AppDataDirectory, databaseName);
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // SQLitePCL.Batteries_V2.Init();
                databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", databaseName); ;         }
            else
            {
                throw new NotImplementedException("Platform not supported");
            }
            return databasePath;
        }
    }
}
