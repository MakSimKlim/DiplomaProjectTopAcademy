using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DiplomaProjectTopAcademy.Models.MainApplicationModels;

namespace DiplomaProjectTopAcademy.Data
{
    public class DiplomaProjectTopAcademyContext : DbContext
    {
        public DiplomaProjectTopAcademyContext (DbContextOptions<DiplomaProjectTopAcademyContext> options)
            : base(options)
        {
        }

        public DbSet<DiplomaProjectTopAcademy.Models.MainApplicationModels.Slab> Slabs { get; set; } = default!;
        public DbSet<DiplomaProjectTopAcademy.Models.MainApplicationModels.Project> Projects { get; set; } = default!;
    }
}
