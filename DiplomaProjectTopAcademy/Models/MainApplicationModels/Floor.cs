using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class Floor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDFloor { get; set; }
        [Required]
        public required int FloorNumber { get; set; }
        [Required]
        public required int TierNumber { get; set; }
        [Required]
        public required decimal FloorHeight { get; set; }
        [Required]
        public required decimal WallHeight { get; set; }
        [Required]
        [ForeignKey("Section")]
        public required int IDSection { get; set; }

        //Navigation Properties
        public required Section Section { get; set; } // Один Floor → один Section
        public ICollection<SlabsFloorsRelation>? Slabs { get; set; } // Связь многие ко многим через таблицу
    }
}
