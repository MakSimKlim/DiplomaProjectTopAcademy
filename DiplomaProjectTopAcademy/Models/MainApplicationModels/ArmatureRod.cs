using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class ArmatureRod
    {
        [Key]
        public int IDArmatureRod { get; set; }
        [Required]
        public required int LengthInMillimeter { get; set; }
        [Required]
        [ForeignKey("Armature")]
        public required int IDArmature { get; set; }
        [Required]
        [ForeignKey("ArmatureClass")]
        public required int IDArmatureClass { get; set; }

        //Navigation Properties
        [Required]
        public required Armature Armature { get; set; } // Один ArmatureRod → один Armature
        [Required]
        public required ArmatureClass ArmatureClass { get; set; } // Один ArmatureRod → один ArmatureClass
        [Required]
        public ICollection<SlabsArmatureRodsRelation> SlabsArmatureRodsRelations { get; set; }
    }
}
