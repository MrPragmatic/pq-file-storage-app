using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Session handler using this example as template:
// https://gist.github.com/Phantom-KNA/0eabbbe52076370489d0ecbf73f0a6c6

namespace pq_file_storage_project.SessionManager
{
    public class LadeSessionHandler : IGotrueSessionPersistence<Session>
    {
        private const string CacheFileName = ".gotrue.cache";
        // lock object to prevent multiple threads from accessing the file at the same time
        private static readonly object fileLock = new object();

        public async void SaveSession(Session session)
        {
            try
            {
                var cacheDir = FileSystem.CacheDirectory;
                var path = Path.Combine(cacheDir, CacheFileName);
                var sessionData = JsonConvert.SerializeObject(session);

                // Using FileStream and StreamWriter to ensure async writing
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(fileStream))
                {
                    await writer.WriteAsync(sessionData);
                }
            }
            catch (Exception ex)
            {
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert("Error", $"Error saving session: {ex.Message}", "OK");
                }
            }
        }

        public async void DestroySession()
        {
            lock (fileLock)
            {
                try
                {
                    var cacheDir = FileSystem.CacheDirectory;
                    var path = Path.Combine(cacheDir, CacheFileName);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                catch (Exception ex)
                {
                    var mainPage = Application.Current?.MainPage;
                    if (mainPage != null)
                    {
                        mainPage.DisplayAlert("Error", $"Error destroying session: {ex.Message}", "OK");
                    }
                }
            }
        }

        public Session LoadSession()
        {
            lock (fileLock)
            {
                try
                {
                    var cacheDir = FileSystem.CacheDirectory;
                    var path = Path.Combine(cacheDir, CacheFileName);

                    if (File.Exists(path))
                    {
                        var sessionData = File.ReadAllText(path);
                        var session = JsonConvert.DeserializeObject<Session>(sessionData);
                        return session;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    var mainPage = Application.Current?.MainPage;
                    if (mainPage != null)
                    {
                        mainPage.DisplayAlert("Error", $"Error loading session: {ex.Message}", "OK").ConfigureAwait(false);
                    }
                    return null;
                }
            }
        }
    }
}
