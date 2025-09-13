using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        { 
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            //var configuration = new ConfigurationBuilder()
            //   .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..\\FitCoachPro.API"))
            //     .AddJsonFile("appsettings.json", optional: false)
            //     .AddJsonFile("appsettings.Development.json", optional: true)
            //     .Build();

            //var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FitCoachProDb;Integrated Security=True;Connect Timeout=30;"); // can't run wit connectionString

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
