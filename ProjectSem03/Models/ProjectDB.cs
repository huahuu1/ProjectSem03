using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjectSem03.Models
{
    public class ProjectDB : DbContext
    {
        public ProjectDB(DbContextOptions options) : base(options) { }

        public DbSet<Competition> Competition { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Posting> Posting { get; set; }
        public DbSet<Design> Design { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<Award> Award { get; set; }
        public DbSet<Exhibition> Exhibition { get; set; }
    }
}
