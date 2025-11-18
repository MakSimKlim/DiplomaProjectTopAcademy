using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using DiplomaProjectTopAcademy.Models;

namespace DiplomaProjectTopAcademy.Controllers
{
    public class BackupController : Controller
    {
        private readonly string backupDir;
        private readonly string _connectionString;

        public BackupController(IConfiguration config)
        {
            // Читаем строку подключения из appsettings.json или переменных окружения
            _connectionString = config.GetConnectionString("DefaultConnection");
            backupDir = config["BackupSettings:Directory"];
        }

        public IActionResult Index()
        {
            if (!Directory.Exists(backupDir))
                Directory.CreateDirectory(backupDir);

            var files = Directory.GetFiles(backupDir, "*.bak");

            var backups = files.Select(f =>
            {
                var info = new FileInfo(f);
                return new BackupViewModel
                {
                    FileName = info.Name,
                    CreatedAt = info.CreationTime,
                    SizeKb = info.Length / 1024,
                    Location = "Local"
                };
            }).OrderByDescending(b => b.CreatedAt).ToList();

            return View(backups);
        }

        [HttpPost]
        public IActionResult CreateBackup()
        {
            if (!Directory.Exists(backupDir))
                Directory.CreateDirectory(backupDir);

            // Получаем имя базы из connection string
            var builder = new SqlConnectionStringBuilder(_connectionString);
            var dbName = builder.InitialCatalog;

            // Формируем имя файла: <DbName>_YYYY_MM_DD_HH_mm_ss.bak
            var fileName = $"{dbName}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.bak";
            var filePath = Path.Combine(backupDir, fileName);

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = $"BACKUP DATABASE [{dbName}] TO DISK = '{filePath}'";
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            TempData["Message"] = $"Backup {fileName} created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Download(string fileName)
        {
            var path = Path.Combine(backupDir, fileName);
            if (!System.IO.File.Exists(path))
                return NotFound();

            var mimeType = "application/octet-stream";
            var fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, mimeType, fileName);
        }

        [HttpPost]
        public IActionResult CleanupOldBackups(int keepLast = 10)
        {
            var files = Directory.GetFiles(backupDir, "*.bak")
                                 .Select(f => new FileInfo(f))
                                 .OrderByDescending(f => f.CreationTime)
                                 .ToList();

            var oldFiles = files.Skip(keepLast);
            foreach (var file in oldFiles)
            {
                file.Delete();
            }

            TempData["Message"] = $"Cleanup done. Only last {keepLast} backups kept.";
            return RedirectToAction("Index");
        }
    }
}
