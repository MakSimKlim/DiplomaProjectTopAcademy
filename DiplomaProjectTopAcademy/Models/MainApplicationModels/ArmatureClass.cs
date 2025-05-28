using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class ArmatureClass
    {
        [Key]
        public int IDArmatureClass { get; set; }        
        [Required]
        public required string NameClass { get; set; }        
        [Required]
        public required decimal FluidityYield { get; set; }

        //Navigation Properties
        [Required]
        public required ICollection<ArmatureRod> ArmatureRods { get; set; } // Один ArmatureClass → много ArmatureRods
    }
}
