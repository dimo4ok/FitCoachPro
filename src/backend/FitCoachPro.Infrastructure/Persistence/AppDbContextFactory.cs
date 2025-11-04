using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FitCoachPro.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Try to find the API project directory that contains appsettings.json.
        // Walk up from the current directory to a reasonable limit.
        string start = Directory.GetCurrentDirectory();
        string projectPath = null;
        var current = new DirectoryInfo(start);
        for (int i = 0; i < 10 && current != null; i++)
        {
            // Prefer explicit FitCoachPro.API folder when present
            var apiFolder = Path.Combine(current.FullName, "FitCoachPro.API");
            if (Directory.Exists(apiFolder))
            {
                projectPath = apiFolder;
                break;
            }

            // Or use the directory itself if it contains appsettings.json
            if (File.Exists(Path.Combine(current.FullName, "appsettings.json")))
            {
                projectPath = current.FullName;
                break;
            }

            current = current.Parent;
        }

        // Fallback to current directory if nothing found
        projectPath ??= Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
           .SetBasePath(projectPath)
           // Make files optional for design-time to avoid hard failures when running from different working directories.
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
           .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
           .AddEnvironmentVariables()
           .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Could not find a connection string named 'DefaultConnection'. " +
                "Ensure appsettings.json (or environment variables) provide it, or run 'dotnet ef' with --startup-project pointing to the API project."
            );
        }

        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
