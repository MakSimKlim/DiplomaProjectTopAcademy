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
        public string? Name { get; set; }
        public decimal? Scope { get; set; }
        public decimal? Thickness { get; set; }
        public string? ProjectSheet { get; set; }
        [ForeignKey("Project")]
        public int? IDProject { get; set; }
        public byte[]? Image { get; set; }

        //Navigation Properties
        public Project? Project { get; set; } // Один Slab → один Project
        public ICollection<SlabsFloorsRelation>? Floors { get; set; } // Связь многие ко многим через таблицу
        public ICollection<SlabsConcretesRelation>? Concretes { get; set; } // Связь многие ко многим через таблицу
        public ICollection<SlabsArmatureRodsRelation>? ArmatureRods { get; set; } // Связь многие ко многим через таблицу
    }
}

// Brench for Scaffolding