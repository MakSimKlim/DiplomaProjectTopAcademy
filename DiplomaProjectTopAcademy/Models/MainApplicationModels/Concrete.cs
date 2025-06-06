using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class Concrete
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDConcrete { get; set; }
        [Required]
        public required string ConcreteClass { get; set; }
        [Required]
        public required string ConcreteGrade { get; set; }
        [Required]
        public required decimal StrengthMPa { get; set; }

        //Navigation Properties
        public ICollection<Slab>? Slabs { get; set; } // Связь один ко многим

    }
}
