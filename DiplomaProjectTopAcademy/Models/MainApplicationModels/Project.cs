using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class Project
    {
        [Key]
        public int IDProject { get; set; }
        [Required]
        public required string Designation { get; set; }
        [Required]
        public required string ProjectName { get; set; }
        [Required]
        public required decimal ChangeNumber { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public required DateTime issueDate { get; set; }

        //Navigation Properties
        public required ICollection<Slab> Slabs { get; set; } // Один Project → много Slabs

    }
}
