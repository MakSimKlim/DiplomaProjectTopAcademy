using DiplomaProjectTopAcademy.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaProjectTopAcademy.Controllers
{
    public class BackupController : Controller
    {
        public IActionResult Index()
        {
            // Здесь можно получить список файлов из папки резервных копий
            var backups = new List<BackupViewModel>
        {
            new BackupViewModel { FileName = "backup_2025_11_18.bak", CreatedAt = DateTime.Now.AddDays(-1), SizeKb = 20480, Location = "Local" },
            new BackupViewModel { FileName = "backup_2025_11_17.bak", CreatedAt = DateTime.Now.AddDays(-2), SizeKb = 19800, Location = "Cloud" }
        };

            return View(backups);
        }

        [HttpPost]
        public IActionResult CreateBackup()
        {
            // Здесь вызывается сервис, который делает BACKUP DATABASE
            // Например, через SqlCommand: BACKUP DATABASE [YourDb] TO DISK = '...'

            TempData["Message"] = "Backup created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Download(string fileName)
        {
            // Здесь читаем файл из папки резервных копий
            var path = Path.Combine("C:\\Backups", fileName);
            var mimeType = "application/octet-stream";
            var fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, mimeType, fileName);
        }
    }
}
