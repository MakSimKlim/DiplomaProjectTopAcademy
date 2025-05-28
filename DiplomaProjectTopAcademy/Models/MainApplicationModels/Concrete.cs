using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        //Navigation Properties
        [Required]
        public required ICollection<SlabsConcretesRelation> SlabsConcretesRelations { get; set; } // Связь многие ко многим через таблицу

    }
}
