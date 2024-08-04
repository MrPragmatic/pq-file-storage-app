using Supabase;
using pq_file_storage_project.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pq_file_storage_project.Services;
using pq_file_storage_project.Pages;
using static Supabase.Gotrue.Constants;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel.Communication;
using Supabase.Interfaces;
using pq_file_storage_project.Features.Userspace;

namespace pq_file_storage_project.Features.Login
{
    public class UserSpaceCommand(UserSpaceViewModel viewModel, SupabaseService supabaseService) : AsyncCommandBase
    {
        private readonly UserSpaceViewModel viewModel = viewModel;
        private readonly SupabaseService _supabaseService = supabaseService;

        protected override async Task ExecuteAsync(object? parameter)
        {
            // initialize Supabase client
            await _supabaseService.InitializeAsync();
        }
    }
}