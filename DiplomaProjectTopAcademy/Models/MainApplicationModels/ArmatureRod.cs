using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class ArmatureRod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public required Armature Armature { get; set; } // связь один ко многим
        public required ArmatureClass ArmatureClass { get; set; } // связь один ко многим
        public ICollection<SlabsArmatureRodsRelation>? SlabsArmatureRodsRelations { get; set; } // Связь многие ко многим через таблицу
    }
}
