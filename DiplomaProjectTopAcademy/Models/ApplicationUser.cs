using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        //[Required]
        public int? UsernameChangeLimit { get; set; } = 10; // set number of username changes 
        //[Required]
        public byte[]? ProfilePicture { get; set; }
    }
}
