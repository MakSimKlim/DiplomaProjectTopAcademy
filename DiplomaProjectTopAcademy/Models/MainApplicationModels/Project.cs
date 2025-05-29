using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDProject { get; set; }
        public string? Designation { get; set; }
        public string? ProjectName { get; set; }
        public decimal? ChangeNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime? issueDate { get; set; }

        //Navigation Properties
        public ICollection<Slab>? Slabs { get; set; } // Один Project → много Slabs

    }
}
