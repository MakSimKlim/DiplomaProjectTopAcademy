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

        // Новые поля для подписки
        public bool IsActive { get; set; } = true; // главный флаг доступа
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public string? SubscriptionType { get; set; } // Trial, Monthly, Yearly, Test
        public bool TrialUsed { get; set; } = false; // чтобы не дать второй раз
    }
}
