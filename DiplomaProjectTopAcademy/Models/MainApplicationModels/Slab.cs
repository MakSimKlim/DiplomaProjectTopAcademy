using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class Slab
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDSlab { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required decimal Scope { get; set; }
        [Required]
        public required decimal Thickness { get; set; }
        [Required]
        public required string ProjectSheet { get; set; }
        [Required]
        [ForeignKey("Project")]
        public required int IDProject { get; set; }
        public byte[]? Image { get; set; }

        //Navigation Properties
        public required Project Project { get; set; } // Один Slab → один Project
        public ICollection<SlabsFloorsRelation>? Floors { get; set; } // Связь многие ко многим через таблицу
        public ICollection<SlabsConcretesRelation>? Concretes { get; set; } // Связь многие ко многим через таблицу
        public ICollection<SlabsArmatureRodsRelation>? ArmatureRods { get; set; } // Связь многие ко многим через таблицу
    }
}

