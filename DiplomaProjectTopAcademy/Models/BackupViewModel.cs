namespace DiplomaProjectTopAcademy.Models
{
    public class BackupViewModel
    {
        public string FileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public long SizeKb { get; set; }
        public string Location { get; set; } // Local / Cloud
    }

}
