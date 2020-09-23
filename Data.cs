using System;
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
            modelBuilder.Entity<Incident>().HasKey(lc => new { lc.IncidentID }); ;
            modelBuilder.Entity<ControlParam>().HasKey(lc => new { lc.Key }); ;
        }
    }

    public class Incident
    {
        [ExcelColPos(1)]
        public string IncidentID { get; set; }
        [ExcelColPos(2)]
        public string Summary { get; set; }
        [ExcelColPos(3)]
        public string ReportedDate { get; set; }
    }


    public class ControlParam
    {
        public string Key { get; set; }
        public string Value { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property)]
    class ExcelColPos : Attribute
    {
        private int colPos;

        // Constructor 
        public ExcelColPos(int colPos)
        {
            this.colPos = colPos;
        }

        public int Colpos
        {
            get { return colPos; }
        }


    }

}