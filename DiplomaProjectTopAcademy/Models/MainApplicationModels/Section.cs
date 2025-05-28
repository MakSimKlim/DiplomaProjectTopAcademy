using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class Section
    {
        [Key]
        public int IDSection { get; set; }
        [Required]
        public required string SectionName { get; set; }        
        [Required]
        public required string Axes { get; set; }        
        [Required]
        public required decimal RelativeZeroMark { get; set; }        
        [Required]
        public required decimal AbsoluteZeroMark { get; set; }

        //Navigation properties
        public required ICollection<Floor> Floors { get; set; } // Один Section → много Floors

    }
}
