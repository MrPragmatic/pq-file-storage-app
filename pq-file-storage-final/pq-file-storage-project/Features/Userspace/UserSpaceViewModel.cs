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
using pq_file_storage_project.Features.Login;
using DataAccessClassLibrary.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO;
using Windows.Foundation.Metadata;
using System.Security.Cryptography;
using pq_file_storage_project.Features.Encryption;
using pq_file_storage_project.Features.Backup;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Storage;
using System.Security.Cryptography.Xml;
using Amazon.Util.Internal;
using Amazon.Runtime.SharedInterfaces;
using pq_file_storage_project.SessionManager;

namespace pq_file_storage_project.Features.Userspace
{
    public class UserSpaceViewModel : ViewModelBase
    {
        private readonly SupabaseService _supabaseService;
        private readonly SessionRedirector _sessionRedirector;
        public ObservableCollection<FolderItem> Folders { get; set; }

        private FolderItem? _selectedFolder;
        public FolderItem? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
                LoadFilesForSelectedFolder();
            }
        }
        
        private FileItem? _selectedFile;

        public FileItem? SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
            }
        }


        private ObservableCollection<FileItem> _selectedFiles;
        public ObservableCollection<FileItem> SelectedFiles
        {
            get => _selectedFiles;
            set
            {
                if (_selectedFiles != value)
                {
                    _selectedFiles = value;
                    OnPropertyChanged(nameof(SelectedFiles));
                }
            }
        }

        public new ICommand ExitCommand { get; }
        public ICommand UserSpaceCommand { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand EncryptAndSyncCommand { get; }
        public ICommand DecryptAndSyncCommand { get; }
        public ICommand BackUpCommand { get; }

        public ICommand CreateNewFileCommand { get; }

        public ICommand CreateNewFolderCommand { get; }

        public ICommand DeleteCommand { get; }

        public UserSpaceViewModel(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
            _sessionRedirector = new SessionRedirector(_supabaseService);
            Folders = new ObservableCollection<FolderItem>();
            SelectedFiles = new ObservableCollection<FileItem>();

            UserSpaceCommand = new UserSpaceCommand(this, supabaseService);
            ExitCommand = new Command(async () => await ExitCommand());
            OpenFileCommand = new Command<FileItem>(OpenFile);
            EncryptAndSyncCommand = new Command(async () => await Encrypt());
            DecryptAndSyncCommand = new Command(async () => await Decrypt());
            BackUpCommand = new Command(async () => await BackUp());
            CreateNewFileCommand = new Command(CreateNewFile);
            CreateNewFolderCommand = new Command(CreateNewFolder);
            DeleteCommand = new Command(async () => await Delete());

            LoadFolders();
            LoadRootFiles();
        }

        private async void CreateNewFile()
        {
            await _sessionRedirector.StayOrRedirectToLogin();

            // the list of allowed file extensions
            string[] allowedExtensions = { ".doc", ".docx", ".rtf", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".txt", ".encrypted", ".key", ".base64", ".iv" };

            var mainPage = Application.Current?.MainPage;
            string fileName = await mainPage.DisplayPromptAsync("Create a new file", "What's the name and extension of the file, for example, test.doc?");

           
            // get file extension
            string fileExtension = Path.GetExtension(fileName).ToLower();

            if (allowedExtensions.Contains(fileExtension))
            {
                try
                {
                    if (SelectedFolder == null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "No folder selected.", "OK");
                        return;
                    }
                    string userBaseFilesPath = AppDomain.CurrentDomain.BaseDirectory;
                    string folder = Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\UserFiles\");
                    string newFilePath = Path.Combine(folder, fileName);

                    // checking if the file already exists
                    if (File.Exists(newFilePath))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", $"File {newFilePath} already exists.", "OK");
                        return;
                    }

                    // create file
                    File.Create(newFilePath).Dispose();

                    SelectedFolder.Items.Add(new FileItem
                    {
                        Name = fileName,
                        Path = newFilePath,
                        Icon = GetIconForFile(fileName)
                    });

                    var innerMainPage = Application.Current?.MainPage;
                    if (innerMainPage != null)
                    {
                        await innerMainPage.DisplayAlert("Success", $"File {fileName} created successfully.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    var innerMainPage = Application.Current?.MainPage;
                    if (innerMainPage != null)
                    {
                        await innerMainPage.DisplayAlert("Error", $"Error creating new file: {ex.Message}", "OK");
                    }
                }
            } else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid file extension.", "OK");
            }
        }

        private async void CreateNewFolder()
        {
            await _sessionRedirector.StayOrRedirectToLogin();
            if (SelectedFolder == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No folder selected.", "OK");
                return;
            }

            var mainPage = Application.Current?.MainPage;
            if (mainPage != null)
            {
                string newFolderName = await mainPage.DisplayPromptAsync(
                    "Create a new folder",
                    "What is the name of the new folder?",
                    "OK",
                    "Cancel",
                    "New Folder",
                    maxLength: 255
                );
                if (!string.IsNullOrEmpty(newFolderName))
                {
                    string newFolderPath = Path.Combine(SelectedFolder.Path, newFolderName);

                    // checking if the folder already exists
                    if (Directory.Exists(newFolderPath))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", $"Folder {newFolderName} already exists.", "OK");
                        return;
                    }

                    Directory.CreateDirectory(newFolderPath);

                    Folders.Add(new FolderItem
                    {
                        Name = newFolderName,
                        Path = newFolderPath,
                        Icon = GetIconForFile(""),
                        Items = new ObservableCollection<object>()
                    });

                    await Application.Current.MainPage.DisplayAlert("Success", $"{newFolderName} created successfully.", "OK");

                    // Refresh the view
                    LoadFolders();
                    LoadRootFiles();
                }
                else
                {
                    // cancellation handling
                    await mainPage.DisplayAlert("Cancelled", "Folder creation was cancelled.", "OK");
                }
            }else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Main page is null", "OK");
            }
        }

        private async Task Delete()
        {
            await _sessionRedirector.StayOrRedirectToLogin();

            var mainPage = Application.Current?.MainPage;

            try
            {
                if (SelectedFile == null && SelectedFolder == null)
                {
                    if (mainPage != null)
                    {
                        await mainPage.DisplayAlert("Error", "No file or folder selected.", "OK");
                    }
                    return;
                }

                bool isConfirmed = await mainPage.DisplayAlert("Confirm Delete", "Are you sure you want to delete the selected item(s)?", "Yes", "No");

                if (isConfirmed)
                {
                    string userBaseFilesPath = AppContext.BaseDirectory;
                    string userFilesPath = Path.GetFullPath(Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\UserFiles\")).TrimEnd(Path.DirectorySeparatorChar);

                    // Ensure paths are within the UserFiles directory and not the root directory
                    bool IsWithinUserFiles(string path)
                    {
                        string fullPath = Path.GetFullPath(path);
                        bool isWithin = fullPath.StartsWith(userFilesPath, StringComparison.OrdinalIgnoreCase);
                        bool isRoot = fullPath.Equals(userFilesPath, StringComparison.OrdinalIgnoreCase);
                        return isWithin && !isRoot;
                    }

                    // Handle file deletion
                    if (SelectedFile != null)
                    {
                        string filePath = Path.GetFullPath(SelectedFile.Path);

                        if (File.Exists(filePath) && IsWithinUserFiles(filePath))
                        {
                            File.Delete(filePath);
                            SelectedFolder?.Items.Remove(SelectedFile);
                            SelectedFiles.Remove(SelectedFile);
                            SelectedFile = null;
                        }
                        else
                        {
                            await mainPage?.DisplayAlert("Error", "The selected file does not exist or is outside the allowed directory.", "OK");
                        }
                    }

                    // Refresh the view
                    LoadFolders();
                    LoadRootFiles();
                }
            }
            catch (Exception ex)
            {
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert("Error", $"Error deleting item: {ex.Message}", "OK");
                }
            }
        }

        private bool IsWithinUserFiles(string path)
        {
            string userBaseFilesPath = AppContext.BaseDirectory;
            string userFilesPath = Path.GetFullPath(Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\UserFiles\"));

            return Path.GetFullPath(path).StartsWith(userFilesPath, StringComparison.OrdinalIgnoreCase);
        }

        private async void LoadFolders()
        {
            try
            {
                string userBaseFilesPath = AppContext.BaseDirectory;
                string folderPath = Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\UserFiles\");
                string userFilesPath = Path.GetFullPath(folderPath);

                // Set the name for the root folder and load its contents
                var rootFolderItem = new FolderItem
                {
                    // Set a name for the root folder
                    Name = "Main folder",
                    Path = userFilesPath,
                    Icon = GetIconForFile("")
                };

                // Load root and subfolders
                LoadFolderRecursively(userFilesPath, rootFolderItem);
                Folders.Clear();
                Folders.Add(rootFolderItem);
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Error", $"Error loading folders: {ex.Message}\n{ex.StackTrace}", "OK");
            }
        }

        private void LoadFolderRecursively(string path, FolderItem? parentFolder)
        {
            try
            {
                // Create folder item
                var folderItem = new FolderItem
                {
                    Name = Path.GetFileName(path),
                    Path = path,
                    Icon = GetIconForFile("")
                };

                // Add to parent or root
                if (parentFolder != null)
                {
                    parentFolder.Items.Add(folderItem);
                }
                else
                {
                    Folders.Add(folderItem);
                }

                // Load subdirectories
                string[] subdirectories = Directory.GetDirectories(path);
                foreach (var subdirectory in subdirectories)
                {
                    LoadFolderRecursively(subdirectory, folderItem);
                }

                // Load files in the current directory
                string[] files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    if (IsAllowedFileType(file))
                    {
                        folderItem.Items.Add(new FileItem
                        {
                            Name = Path.GetFileName(file),
                            Path = file,
                            Icon = GetIconForFile(Path.GetFileName(file))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current?.MainPage?.DisplayAlert("Error", $"Error loading folder {path}: {ex.Message}", "OK");
            }
        }

        private void LoadFolder(string path)
        {
            try
            {
                var folderItem = new FolderItem
                {
                    Name = Path.GetFileName(path),
                    Path = path,
                    Icon = GetIconForFile("")
                };

                // Load subdirectories
                string[] subdirectories = Directory.GetDirectories(path);
                foreach (var subdirectory in subdirectories)
                {
                    var subFolderItem = new FolderItem
                    {
                        Name = Path.GetFileName(subdirectory),
                        Path = subdirectory,
                        Icon = GetIconForFile("")
                    };
                    folderItem.Items.Add(subFolderItem);
                }

                // Load files in the current directory
                string[] files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    if (IsAllowedFileType(file))
                    {
                        folderItem.Items.Add(new FileItem
                        {
                            Name = Path.GetFileName(file),
                            Path = file,
                            Icon = GetIconForFile(Path.GetFileName(file))
                        });
                    }
                }

                // Add the loaded folder to the collection
                Folders.Add(folderItem);
            }
            catch (Exception ex)
            {
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    mainPage.DisplayAlert("Error", $"Error loading folder: {ex.Message}", "OK");
                }
            }
        }

        private async void LoadRootFiles()
        {
            try
            {
                // Attempt to get the project base directory
                string userBaseFilesPath = AppContext.BaseDirectory;
                string folderPath = Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\UserFiles\");
                string userFilesPath = Path.GetFullPath(folderPath);

                var mainPage = Application.Current?.MainPage;
                // Additional debugging output
                /*
                
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert("Debug Info", $"UserBaseFilesPath: {userBaseFilesPath}\nFolderPath: {folderPath}\nUserFilesPath: {userFilesPath}", "OK");
                }
                */

                // Check if the path exists
                if (!Directory.Exists(userFilesPath))
                {
                    // Output the path to debug
                    if (mainPage != null)
                    {
                        await mainPage.DisplayAlert("Error", $"Directory does not exist at path: {userFilesPath}", "OK");
                    }
                    return;
                }

                string[] directories = Directory.GetDirectories(userFilesPath);

                foreach (var directory in directories)
                {
                    var folderItem = new FolderItem
                    {
                        Name = Path.GetFileName(directory),
                        Path = directory,
                        Icon = GetIconForFile("")
                    };

                    // Load files in the folder
                    string[] files = Directory.GetFiles(directory);
                    foreach (var file in files)
                    {
                        if (IsAllowedFileType(file))
                        {
                            folderItem.Files.Add(new FileItem
                            {
                                Name = Path.GetFileName(file),
                                Path = file, // Ensure path is set correctly
                                Icon = GetIconForFile(Path.GetFileName(file))
                            });
                        }
                    }

                    Folders.Add(folderItem);
                    // Set the name for the root folder and load its contents
                    var rootFolderItem = new FolderItem
                    {
                        // Set a name for the root folder
                        Name = "Main folder",
                        Path = userFilesPath,
                        Icon = GetIconForFile("")
                    };

                    // Load root and subfolders
                    SelectedFolder = rootFolderItem;
                }
            }
            catch (Exception ex)
            {
                // Output the full exception to debug
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert("Error", $"Error loading folders and files: {ex.Message}\nStack Trace: {ex.StackTrace}", "OK");
                }
            }
        }

        private void LoadFilesForSelectedFolder()
        {
            if (SelectedFolder != null)
            {
                SelectedFolder.Items.Clear();
                string[] files = Directory.GetFiles(SelectedFolder.Path);
                foreach (var file in files)
                {
                    if (IsAllowedFileType(file))
                    {
                        SelectedFolder.Items.Add(new FileItem
                        {
                            Name = Path.GetFileName(file),
                            Path = file,
                            Icon = GetIconForFile(Path.GetFileName(file))
                        });
                    }
                }

                string[] subdirectories = Directory.GetDirectories(SelectedFolder.Path);
                foreach (var subdirectory in subdirectories)
                {
                    var subFolderItem = new FolderItem
                    {
                        Name = Path.GetFileName(subdirectory),
                        Path = subdirectory,
                        Icon = GetIconForFile("")
                    };

                    string[] subFiles = Directory.GetFiles(subdirectory);
                    foreach (var subFile in subFiles)
                    {
                        if (IsAllowedFileType(subFile))
                        {
                            subFolderItem.Items.Add(new FileItem
                            {
                                Name = Path.GetFileName(subFile),
                                Path = subFile,
                                Icon = GetIconForFile(Path.GetFileName(subFile))
                            });
                        }
                    }
                    SelectedFolder.Items.Add(subFolderItem);
                }
            }
        }

        private static bool IsAllowedFileType(string? fileName)
        {
            if (fileName == null) return false;

            string[] allowedExtensions = [".doc", ".docx", ".rtf", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".txt", ".encrypted", ".key", ".base64" , ".iv"];
            string extension = Path.GetExtension(fileName).ToLower();
            return allowedExtensions.Contains(extension);
        }

        private static string GetIconForFile(string? fileName)
        {
            if (fileName == null) return string.Empty;

            string userBaseFilesPath = AppDomain.CurrentDomain.BaseDirectory;

            string wordIcon = Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\Resources\Images\word_icon.png");
            string wordIconPath = Path.GetFullPath(Path.Combine(wordIcon));

            string excelIcon = Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\Resources\Images\excel_icon.png");
            string excelIconPath = Path.GetFullPath(Path.Combine(excelIcon));

            string pptIcon = Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\Resources\Images\powerpoint_icon.png");
            string pptIconPath = Path.GetFullPath(Path.Combine(pptIcon));

            string pdfIcon = Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\Resources\Images\pdf_icon.png");
            string pdfIconPath = Path.GetFullPath(Path.Combine(pdfIcon));

            string txtIcon = Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\Resources\Images\text_icon.png");
            string txtIconPath = Path.GetFullPath(Path.Combine(txtIcon));

            string folderIcon = Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\Resources\Images\folder_icon.png");
            string folderIconPath = Path.GetFullPath(folderIcon);

            string extension = Path.GetExtension(fileName).ToLower();
            if (extension == string.Empty) // Assuming no extension means it's a folder
            {
                return folderIconPath;
            }

            return extension switch
            {
                ".doc" or ".docx" or ".rtf" => wordIconPath,
                ".xls" or ".xlsx" => excelIconPath,
                ".ppt" or ".pptx" => pptIconPath,
                ".pdf" => pdfIconPath,
                ".txt" or ".encrypted" or ".key" or ".base64" => txtIconPath
            };
        }

        private async void OpenFile(FileItem file)
        {
            await _sessionRedirector.StayOrRedirectToLogin();
            try
            {
                string userBaseFilesPath = AppDomain.CurrentDomain.BaseDirectory;
                string folder = Path.Combine(userBaseFilesPath, @"..\..\..\..\..\..\UserFiles\");
                string userFilesPath = file?.Name != null ? Path.GetFullPath(Path.Combine(folder, file.Name)) : string.Empty;

                if (!string.IsNullOrEmpty(userFilesPath) && File.Exists(userFilesPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = userFilesPath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    mainPage.DisplayAlert("Error", $"Error opening file: {ex.Message}", "OK");
                }
            }
        }

        // private asynchronous Encrypt method is responsible for encrypting the selected file
        private async Task Encrypt()
        {
            await _sessionRedirector.StayOrRedirectToLogin();
            try
            {
                // Check if the selected file is null
                if (SelectedFile == null)
                {
                    // if mainPage is not null, display an alert with the error message
                    var mainPage = Application.Current?.MainPage;
                    if (mainPage != null)
                    {
                        // Display an alert with the error message
                        await mainPage.DisplayAlert("Error", "No file selected.", "OK");
                    }
                    return;
                }

                // Create a new instance of Aes to be used for encryption utilizing AesEncryptionUtility
                using (Aes aes = Aes.Create())
                {
                    // Ensure AES-256 key size
                    aes.KeySize = 256;
                    // Generate a new IV for the AES encryption
                    aes.GenerateIV();
                    // save the IV to a byte array
                    byte[] iv = aes.IV;

                    // Load the configuration settings from the appsettings.json file
                    var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                    // Get the KMS key ID from the configuration settings
                    var kmsKeyId = configuration["AWS:Kms-key-id"];

                    // Get the unencrypted file path
                    string unencryptedFilePath = SelectedFile.Path;
                    // Get the selected file name without the extension
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(unencryptedFilePath);

                    // Generate the data key corresponding to the file using AWS KMS (filename as encryption context)
                    var (plaintextKeyBase64, encryptedKeyBase64) = await AwsKmsUtility.GenerateDataKeyAsync(kmsKeyId, fileNameWithoutExtension);

                    // store AWS KMS generated plaintext data key in byte arrary
                    byte[] plaintextKeyBytes = plaintextKeyBase64;
                    // store AWS KMS generated encrypted data key in encryptedDataKey variable after converting it from base64 to byte array
                    var encryptedDataKey = Convert.FromBase64String(encryptedKeyBase64);

                    // Check if the plaintext key is null or the encrypted key is null or empty
                    if (plaintextKeyBase64 == null || string.IsNullOrEmpty(encryptedKeyBase64))
                    {
                        // if mainPage is not null, display an alert with the error message
                        var mainPage = Application.Current?.MainPage;
                        if (mainPage != null)
                        {
                            // Display an alert with the error message indicating that the data key generation failed
                            await mainPage.DisplayAlert("Error", "Failed to generate or store data key.", "OK");
                        }
                        // exit the method execution
                        return;
                    }

                    // save selected file path to a string variable inputFilePath
                    string inputFilePath = SelectedFile.Path;
                    // check if the inputFilePath is null or empty
                    if (string.IsNullOrEmpty(inputFilePath))
                    {
                        // if mainPage is not null, display an alert with the error message
                        var mainPage = Application.Current?.MainPage;
                        if (mainPage != null)
                        {
                            // Display an alert with the error message indicating that the file path is null or empty to the user
                            await mainPage.DisplayAlert("Error", $"File path is null or empty for the file {SelectedFile.Name}.", "OK");
                        }
                        // return, exit the method execution
                        return;
                    }
                    // save the encrypted file combining the input file path and .encrypted extension to a string variable outputFilePath
                    string outputFilePath = inputFilePath + ".encrypted";

                    try
                    {
                        // attempt to encrypt the file using AES-GCM with the plaintext data key using AesEncryptionUtility class
                        AesEncryptionUtility.EncryptFile(inputFilePath, outputFilePath, plaintextKeyBase64);

                        // check if the encrypted file does not exist at the outputFilePath
                        if (!File.Exists(outputFilePath))
                        {
                            // if mainPage is not null, display an alert with the error message
                            var mainPageInner = Application.Current?.MainPage;
                            if (mainPageInner != null)
                            {
                                // Display an alert with the error message indicating that the encrypted file was not created
                                await mainPageInner.DisplayAlert("Error", $"Encrypted file not created: {outputFilePath}", "OK");
                            }
                            // return, exit the method execution
                            return;
                        }

                        // same the encrypted data key reasonably
                        string encryptedDataKeyFilePath = unencryptedFilePath + "-key.base64";
                        // save the encrypted data key to a string variable encryptedKeyBase64
                        await File.WriteAllTextAsync(encryptedDataKeyFilePath, encryptedKeyBase64);

                        // Delete the unencrypted file
                        File.Delete(unencryptedFilePath);

                        // if mainPage is not null, display an alert with the success message
                        var mainPage = Application.Current?.MainPage;
                        if (mainPage != null)
                        {
                            // Display an alert with the success message indicating that the file was encrypted successfully
                            await mainPage.DisplayAlert("Success", "File encrypted successfully.", "OK");
                        }

                        // check if the selected folder is not null and the selected folder items are not null
                        if (SelectedFolder != null && SelectedFolder.Items != null)
                        {
                            // cast the items to fileItem and find the matching file by path
                            var fileItem = SelectedFolder.Items.OfType<FileItem>().FirstOrDefault(f => f.Path == unencryptedFilePath);
                            // if (the selected) fileItem is not null
                            if (fileItem != null)
                            {
                                // remove the unencrypted file from the selected folder items
                                SelectedFolder.Items.Remove(fileItem);

                                // refresh the items in the selected folder by setting the selected folder to null and then back to the selected folder
                                var previousFolder = SelectedFolder;
                                SelectedFolder = null;
                                SelectedFolder = previousFolder;
                            }
                        }
                    }
                    // catch possible exception during encryption process
                    catch (Exception ex)
                    {
                        // if mainPage is not null, display an alert with the error message
                        var mainPage = Application.Current?.MainPage;
                        if (mainPage != null)
                        {
                            // Display an alert with the error message indicating that an error occurred during encryption
                            await mainPage.DisplayAlert("Error", $"Error encrypting file {SelectedFile.Name}: {ex.Message}", "OK");
                        }
                    }
                }
            }
            // catch possible exception during the Encrypt() method execution
            catch (Exception ex)
            {
                // if mainPage is not null, display an alert with the error message
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null)
                {
                    // Display an alert with the error message indicating that an error occurred during encryption
                    await mainPage.DisplayAlert("Error", $"Error during encryption: {ex.Message}", "OK");
                }
            }
        }

        // private asynchronous Decrypt method is responsible for decrypting the selected file
        private async Task Decrypt()
        {
            await _sessionRedirector.StayOrRedirectToLogin();
            try
            {
                // Check if the selected file is null
                if (SelectedFile == null)
                {
                    // if mainPage is not null, display an alert with the error message
                    var mainPage = Application.Current?.MainPage;
                    if (mainPage != null)
                    {
                        // Display an alert with the error message indicating that no file was selected
                        await mainPage.DisplayAlert("Error", "No file selected.", "OK");
                    }
                    return;
                }

                // save the selected file path to a string variable encryptedFilePath
                string encryptedFilePath = SelectedFile.Path;
                // save the decrypted file path to a string variable decryptedFilePath
                string decryptedFilePath = encryptedFilePath.Replace(".encrypted", "");

                // check if the encryptedFilePath is null or empty
                if (string.IsNullOrEmpty(encryptedFilePath))
                {
                    // if mainPage is not null, display an alert with the error message
                    var mainPage = Application.Current?.MainPage;
                    if (mainPage != null)
                    {
                        // Display an alert with the error message indicating that the file path is null or empty
                        await mainPage.DisplayAlert("Error", $"File path is null or empty for the file {SelectedFile.Name}.", "OK");
                    }
                    return;
                }

                try
                {
                    // use Path.GetDirectoryName() method to get the directory of the encrypted AWS generated data kay file path
                    string directory = Path.GetDirectoryName(encryptedFilePath);
                    // save the encrypted key file path to a string variable fileNameWithoutExtension indicating that the .encryped was trimmed from the file name
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(encryptedFilePath);
                    // save the original key file name without extension to a string variable originalFileNameWithoutExtension indicating that now we have just the file name
                    // this will be used by AwsKmsUtility for encryption context
                    string originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameWithoutExtension);
                    // save the encrypted key file path to a string variable encryptedDataKeyFilePath
                    string encryptedDataKeyFilePath = Path.Combine(directory, fileNameWithoutExtension + "-key.base64");

                    // check if the encryptedDataKeyFilePath does not exist
                    if (!File.Exists(encryptedDataKeyFilePath))
                    {
                        // if mainPage is not null, display an alert with the error message
                        var mainTestingPage = Application.Current?.MainPage;
                        if (mainTestingPage != null)
                        {
                            // Display an alert with the error message indicating that the encrypted key file was not found
                            await mainTestingPage.DisplayAlert("Error", $"Encrypted key file not found: {encryptedDataKeyFilePath}", "OK");
                        }
                        return;
                    }

                    // save the original file to encryptedDataKeyBase64 variable
                    string encryptedDataKeyBase64 = await File.ReadAllTextAsync(encryptedDataKeyFilePath);

                    // AwsKmsUtility is called to decrypt the data key using the encrypted key and the original file name without extension as encryption context
                    byte[] decryptedKey = await AwsKmsUtility.DecryptDataKeyAsync(encryptedDataKeyBase64, originalFileNameWithoutExtension);

                    // check if the decryptedKey is null or the length of the decryptedKey is not equal to 32, corresponding to the AES key size
                    if (decryptedKey == null || decryptedKey.Length != 32)
                    {
                        // if mainPage is not null, display an alert with the error message
                        var mainTestPage = Application.Current?.MainPage;
                        if (mainTestPage != null)
                        {
                            // Display an alert with the error message indicating that the decrypted key is invalid
                            await mainTestPage.DisplayAlert("Error", "Invalid decrypted key.", "OK");
                        }
                        return;
                    }

                    // Decrypt the file using the decrypted AWS generated data key and the DecryptFile() method of AesEncryptionUtility class
                    AesEncryptionUtility.DecryptFile(encryptedFilePath, decryptedFilePath, decryptedKey);

                    // Check if the decrypted file was not created
                    if (!File.Exists(decryptedFilePath))
                    {
                        // if mainPage is not null, display an alert with the error message
                        var mainTesterPage = Application.Current?.MainPage;
                        if (mainTesterPage != null)
                        {
                            // Display an alert with the error message indicating that the decrypted file was not created
                            await mainTesterPage.DisplayAlert("Error", $"Decrypted file not created: {decryptedFilePath}", "OK");
                        }
                        return;
                    }

                    // Delete the encrypted file
                    File.Delete(encryptedFilePath);

                    // Delete the encrypted data key file
                    File.Delete(encryptedDataKeyFilePath);

                    // if mainPage is not null, display an alert with the success message
                    var successPage = Application.Current?.MainPage;
                    if (successPage != null)
                    {
                        // Display an alert with the success message indicating that the file was decrypted successfully
                        await successPage.DisplayAlert("Success", "File decrypted successfully.", "OK");
                    }

                    // check if the selected folder is not null and the selected folder items are not null
                    if (SelectedFolder != null && SelectedFolder.Items != null)
                    {
                        // Cast the items to FileItem and find the matching file by path
                        var fileItem = SelectedFolder.Items.OfType<FileItem>().FirstOrDefault(f => f.Path == encryptedFilePath);
                        if (fileItem != null)
                        {
                            SelectedFolder.Items.Remove(fileItem);

                            // refresh the items in the selected folder by setting the selected folder to null and then back to the selected folder
                            var previousFolder = SelectedFolder;
                            SelectedFolder = null;
                            SelectedFolder = previousFolder;
                        }
                    }
                }
                // catch possible exception during decryption process
                catch (Exception ex)
                {
                    // if mainPage is not null, display an alert with the error message
                    var mainPage = Application.Current?.MainPage;
                    if (mainPage != null)
                    {
                        // Display an alert with the error message indicating that an error occurred during decryption
                        await mainPage.DisplayAlert("Error", $"Error decrypting file {SelectedFile.Name}: {ex.Message}", "OK");
                    }
                }
            }
            // catch possible exception during the Decrypt() method execution
            catch (Exception ex)
            {
                // if mainPage is not null, display an alert with the error message
                var errorPage = Application.Current?.MainPage;
                if (errorPage != null)
                {
                    // Display an alert with the error message indicating that an error occurred during decryption
                    await errorPage.DisplayAlert("Error", $"Error during decryption: {ex.Message}", "OK");
                }
            }
        }

        // private asynchronous BackUp method is responsible for backing up the selected file to the predefined S3 bucket
        private async Task BackUp()
        {
            await _sessionRedirector.StayOrRedirectToLogin();

            // save the mainPage to a variable mainPage
            var mainPage = Application.Current?.MainPage;

            try
            {
                // Check if the selected file is null and display error through display alert if so
                if (SelectedFile == null)
                {
                    if (mainPage != null)
                    {
                        // Display an alert with the error message indicating that no file was selected
                        await mainPage.DisplayAlert("Error", "No file selected.", "OK");
                    }
                    return;
                }

                // Load the configuration settings from the appsettings.json file
                var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

                // Get the S3 bucket name from the configuration settings
                var S3bucketName = configuration["AWS:Bucket-name"];

                // Check if the S3 bucket name is null or empty
                if (string.IsNullOrEmpty(S3bucketName))
                {
                    if (mainPage != null)
                    {
                        // Display an alert with the error message indicating that the S3 bucket name is not configured properly
                        await mainPage.DisplayAlert("Error", "S3 bucket name is not configured properly.", "OK");
                    }
                    return;
                }
                // Create a new instance of AwsS3Utility
                var awsS3Utility = new AwsS3Utility();

                // initialize the variables filePath and fileName to empty strings
                string? filePath = string.Empty;
                string? fileName = string.Empty;

                try
                {
                    // save the selected file path to the filePath variable
                    filePath = SelectedFile.Path;
                    // save the selected file name to the fileName variable
                    fileName = SelectedFile.Name;

                    // attempt to upload the file to the S3 bucket using the UploadFileAsync() method of AwsS3Utility class
                    await awsS3Utility.UploadFileAsync(S3bucketName, filePath, fileName);

                    // if mainPage is not null, display an alert with the success message
                    if (mainPage != null)
                    {
                        // Display an alert with the success message indicating that the file was backed up successfully
                        await mainPage.DisplayAlert("Success", "File backed up successfully.", "OK");
                    }
                }
                // catch possible exception during the backup upload process
                catch (Exception ex)
                {
                    if (mainPage != null)
                    {
                        // Display an alert with the error message indicating that an error occurred during the backup upload process
                        await mainPage.DisplayAlert("Error", $"Error uploading file {fileName}: {ex.Message}", "OK");
                    }
                }
            }
            // catch possible exception during the BackUp() method execution
            catch (Exception ex)
            {
                if (mainPage != null)
                {
                    // Display an alert with the error message indicating that an error occurred during the backup method execution
                    await mainPage.DisplayAlert("Error", $"Error during backup: {ex.Message}", "OK");
                }
            }
        }
    }
}
