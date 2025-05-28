using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class Slab
    {
        [Key]
        public required int IDSlab { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required decimal Scope { get; set; }
        [Required]
        public required decimal Thickness { get; set; }
        [Required]
        public required string ProjectSheet { get; set; }
        [ForeignKey("Project")]
        public required string IDProject { get; set; }
        public byte[]? Image { get; set; }

        //Navigation Properties
        public required Project Project { get; set; } // Один Slab → один Project
        public required ICollection<SlabsFloorsRelation> SlabsFloorsRelations { get; set; } // Связь многие ко многим через таблицу
        public required ICollection<SlabsConcretesRelation> SlabsConcretesRelations { get; set; } // Связь многие ко многим через таблицу
        public required ICollection<SlabsArmatureRodsRelation> SlabsArmatureRodsRelations { get; set; } // Связь многие ко многим через таблицу
    }
}
