using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace iDeskDataScraper
{
    public class iDeskDbContext : DbContext
    {
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<ControlParam> ControlParams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
                    => options.UseSqlite("Data Source=./files/data/iDeskData.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Blog>()
                .HasMany(b => b.Posts)
                .WithOne(p => p.Blog)
                .HasForeignKey(p => p.BlogId);*/

            modelBuilder.Entity<Incident>().HasKey(lc => new { lc.IncidentID });;
            modelBuilder.Entity<ControlParam>().HasKey(lc => new { lc.Key });;
        }
    }

    public class Incident
    {
       public string IncidentID { get; set; }
        public string Summary { get; set; }
    }

    public class ControlParam
    {
       public string Key { get; set; }
       public string Value { get; set; }

    }
}