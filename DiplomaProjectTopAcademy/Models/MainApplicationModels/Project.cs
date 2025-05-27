using System.ComponentModel.DataAnnotations;

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
    }
}
