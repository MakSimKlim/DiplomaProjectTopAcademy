using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class SlabsFloorsRelation
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Slab")]
        public int IDSlab { get; set; }
        [ForeignKey("Floor")]
        public int IDFloor { get; set; }

        //Navigation properties
        [Required]
        public required Slab Slab { get; set; } // Один SlabsFloorsRelation → один Slab
        [Required]
        public required Floor Floor { get; set; } // Один SlabsFloorsRelation → один Floor
    }
}
