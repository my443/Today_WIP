using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Today2.Models;
using Microsoft.EntityFrameworkCore;

namespace Today2.Data
{
    public class AppDbContext : DbContext
    {
        public string databasePath = Properties.Settings.Default.LastDatabasePath;

        public AppDbContext() // Parameterless for design-time tools
            : this(Properties.Settings.Default.LastDatabasePath ?? "actions.db")
        {
        }
        public AppDbContext(string dbPath)
        {
            databasePath = dbPath;
        }
        public DbSet<TodayAction> Actions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // If a custom dbPath was provided, use it. Otherwise, use a default.
                string path = databasePath ?? "actions.db";
                optionsBuilder.UseSqlite($"Data Source={path}");
            }
        }
    }
}
