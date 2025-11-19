using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using DiplomaProjectTopAcademy.Models;

namespace DiplomaProjectTopAcademy.Controllers
{
    public class BackupController : Controller
    {
        private readonly string backupDir;
        private readonly string _connectionString;
        private static string _frequency = "EveryMinute"; // хранится в памяти
        private static DateTime? _lastBackupUtc = null;


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

            // передаём текущий тариф во ViewBag
            ViewBag.BackupFrequency = _frequency;

            // вычисляем время следующего запуска
            var delay = GetDelay();
            if (delay != Timeout.InfiniteTimeSpan && _lastBackupUtc != null)
            {
                ViewBag.NextBackupUtc = _lastBackupUtc.Value.Add(delay);
            }
            else
            {
                ViewBag.NextBackupUtc = null;
            }


            return View(backups);
        }

        [HttpPost]
        public IActionResult SetBackupFrequency(string frequency)
        {
            var allowed = new[] { "EveryMinute", "Hourly", "Daily", "Weekly", "Disabled" };
            if (!allowed.Contains(frequency))
            {
                TempData["Message"] = "Invalid frequency.";
                return RedirectToAction("Index");
            }

            _frequency = frequency; // сохраняем глобально
            TempData["Message"] = $"Frequency set to {frequency}";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateBackup()
        {
            try
            {
                if (!Directory.Exists(backupDir))
                    Directory.CreateDirectory(backupDir);

                var builder = new SqlConnectionStringBuilder(_connectionString);
                var dbName = builder.InitialCatalog;

                var fileName = $"{dbName}_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}.bak";
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

                _lastBackupUtc = DateTime.UtcNow;

                TempData["Message"] = $"Backup {fileName} created successfully!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Backup failed: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        public static TimeSpan GetDelay() => _frequency switch
        {
            "EveryMinute" => TimeSpan.FromMinutes(1),
            "Hourly" => TimeSpan.FromHours(1),
            "Daily" => TimeSpan.FromDays(1),
            "Weekly" => TimeSpan.FromDays(7),
            _ => Timeout.InfiniteTimeSpan // Disabled
        };

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
                try
                {
                    file.Delete();
                }
                catch { /* игнорируем ошибки удаления */ }
            }

            TempData["Message"] = $"Cleanup done. Only last {keepLast} backups kept.";
            return RedirectToAction("Index");
        }
    }
}
