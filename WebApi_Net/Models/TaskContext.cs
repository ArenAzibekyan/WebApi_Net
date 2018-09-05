using System;
using Microsoft.EntityFrameworkCore;

namespace WebApi_Net.Models
{
    public class TaskContext : DbContext
    {
        public DbSet<TaskItem> TaskItems { get; set; }

        // https://www.benday.com/2017/02/17/ef-core-migrations-without-hard-coding-a-connection-string-using-idbcontextfactory/
        // not hardcoded connection string
        // or os environment variables
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var user = Environment.GetEnvironmentVariable("POSTGRESQL_USER");
            var pwd = Environment.GetEnvironmentVariable("POSTGRESQL_PWD");
            var host = Environment.GetEnvironmentVariable("POSTGRESQL_HOST");
            var port = Environment.GetEnvironmentVariable("POSTGRESQL_PORT");
            var database = Environment.GetEnvironmentVariable("POSTGRESQL_DATABASE");

            optionsBuilder.UseNpgsql(
                $"User ID={user}; Password={pwd}; Host={host}; Port={port}; Database={database}; Pooling=true; Use SSL Stream=True; SSL Mode=Require; TrustServerCertificate=True;");
        }
    }
}