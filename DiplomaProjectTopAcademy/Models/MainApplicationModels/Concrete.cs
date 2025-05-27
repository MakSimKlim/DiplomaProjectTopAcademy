using System.ComponentModel.DataAnnotations;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class Concrete
    {
        [Key]
        public int IDConcrete { get; set; }
        [Required]
        public required string ConcreteClass { get; set; }
        [Required]
        public required string ConcreteGrade { get; set; }
        [Required]
        public required decimal StrengthMPa { get; set; }

    }
}
