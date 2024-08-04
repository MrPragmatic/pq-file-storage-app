using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Win32;
using System.Net;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DataAccessClassLibrary.Models
{
    [Table("profiles")]
    public class Profile : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("email")]
        [Required(ErrorMessage = "The email field is required.")]
        [EmailAddress(ErrorMessage = "The email you entered is not a valid e-mail address. Please write a valid email to register.")]
        public string? Email { get; set; }

        [Column("email_confirmed_at")]
        public DateTime? EmailConfirmedAt { get; set; }
    }
}