using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class Armature
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDArmature { get; set; }
        [Required]
        public required string Diameter { get; set; }        
        [Required]
        public required string CrossArea { get; set; }        
        [Required]
        public required string WeightOf1Meter { get; set; }

        //Navigation properties
        public ICollection<ArmatureRod>? ArmatureRods { get; set; } // связь один ко многим
    }
}
