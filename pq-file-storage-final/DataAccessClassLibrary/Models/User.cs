﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DataAccessClassLibrary.Models
{
    [Table("users")]
    public class User : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }
        
        [Column("email")]
        [Required(ErrorMessage = "The email field is required.")]
        [EmailAddress(ErrorMessage = "The email you entered is not a valid e-mail address. Please write a valid email to register.")]
        public string? Email { get; set; }

        [Column("password")]
        [Required(ErrorMessage = "The password field is required.")]
        [PasswordPropertyText]
        public string? Password { get; set; }
    }
}