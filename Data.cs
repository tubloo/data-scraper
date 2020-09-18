using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace iDeskDataScraper
{
    public class iDeskDbContext : DbContext
    {
        public DbSet<Incident> Incidents { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
                    => options.UseSqlite("Data Source=./data/iDeskData.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Blog>()
                .HasMany(b => b.Posts)
                .WithOne(p => p.Blog)
                .HasForeignKey(p => p.BlogId);*/

            modelBuilder.Entity<Incident>();
        }
    }

    public class Incident
    {
        public string IncidentID { get; set; }
        public string Summary { get; set; }

    }

}