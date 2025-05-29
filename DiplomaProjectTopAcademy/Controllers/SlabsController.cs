using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiplomaProjectTopAcademy.Data;
using DiplomaProjectTopAcademy.Models.MainApplicationModels;

namespace DiplomaProjectTopAcademy.Controllers
{
    public class SlabsController : Controller
    {
        private readonly DiplomaProjectTopAcademyContext _context;

        public SlabsController(DiplomaProjectTopAcademyContext context)
        {
            _context = context;
        }

        // GET: Slabs
        public async Task<IActionResult> Index()
        {
            var diplomaProjectTopAcademyContext = _context.Slabs.Include(s => s.Project);
            return View(await diplomaProjectTopAcademyContext.ToListAsync());
        }

        // GET: Slabs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slab = await _context.Slabs
                .Include(s => s.Project)
                .FirstOrDefaultAsync(m => m.IDSlab == id);
            if (slab == null)
            {
                return NotFound();
            }

            return View(slab);
        }

        // GET: Slabs/Create
        public IActionResult Create()
        {
            ViewData["IDProject"] = new SelectList(_context.Set<Project>(), "IDProject", "Designation");
            return View();
        }

        // POST: Slabs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IDSlab,Name,Scope,Thickness,ProjectSheet,IDProject,Image")] Slab slab)
        {
            if (ModelState.IsValid)
            {
                _context.Add(slab);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IDProject"] = new SelectList(_context.Set<Project>(), "IDProject", "Designation", slab.IDProject);
            return View(slab);
        }

        // GET: Slabs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slab = await _context.Slabs.FindAsync(id);
            if (slab == null)
            {
                return NotFound();
            }
            ViewData["IDProject"] = new SelectList(_context.Set<Project>(), "IDProject", "Designation", slab.IDProject);
            return View(slab);
        }

        // POST: Slabs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IDSlab,Name,Scope,Thickness,ProjectSheet,IDProject")] Slab slab, IFormFile imageFile)
        {
            if (id != slab.IDSlab)
            {
                return NotFound();
            }

            var existingSlab = await _context.Slabs.AsNoTracking().FirstOrDefaultAsync(s => s.IDSlab == id);
            if (existingSlab == null)
            {
                return NotFound();
            }

            // 🔹 **Добавленная проверка: передается ли файл?**
            if (imageFile != null && imageFile.Length > 0)
            {
                Console.WriteLine($"Файл загружен: {imageFile.FileName}, размер: {imageFile.Length} байт");

                using (var ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    slab.Image = ms.ToArray();
                }
            }
            else
            {
                // Если файл не загружен, оставляем старое изображение
                slab.Image = existingSlab.Image;
                Console.WriteLine("Изображение не загружено, оставляем старое.");
            }

            try
            {
                _context.Update(slab);
                _context.Entry(slab).Property(s => s.Image).IsModified = true; // ✅ Обновляем `Image`
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SlabExists(slab.IDSlab))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }




        // GET: Slabs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slab = await _context.Slabs
                .Include(s => s.Project)
                .FirstOrDefaultAsync(m => m.IDSlab == id);
            if (slab == null)
            {
                return NotFound();
            }

            return View(slab);
        }

        // POST: Slabs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slab = await _context.Slabs.FindAsync(id);
            if (slab != null)
            {
                _context.Slabs.Remove(slab);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SlabExists(int id)
        {
            return _context.Slabs.Any(e => e.IDSlab == id);
        }
    }
}
