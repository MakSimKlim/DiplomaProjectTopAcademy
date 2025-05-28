using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class Armature
    {
        [Key]
        public int IDArmature { get; set; }
        [Required]
        public required string Diameter { get; set; }        
        [Required]
        public required string CrossArea { get; set; }        
        [Required]
        public required string WeightOf1Meter { get; set; }

        //Navigation properties
        [Required]
        public required ICollection<ArmatureRod> ArmatureRods { get; set; } // Один Armature → много ArmatureRods
    }
}
