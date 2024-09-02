// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// using Microsoft.Extensions.Configuration;
// using System.IO;

// namespace HealthcareManagementSystem.Data
// {
//   public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<HealthcareContext>
//   {
//     public HealthcareContext CreateDbContext(string[] args)
//     {
//       IConfigurationRoot configuration = new ConfigurationBuilder()
//           .SetBasePath(Directory.GetCurrentDirectory())
//           .AddJsonFile("appsettings.json")
//           .Build();

//       var builder = new DbContextOptionsBuilder<HealthcareContext>();
//       var connectionString = configuration.GetConnectionString("HealthcareDatabase");

//       builder.UseSqlite(connectionString);

//       return new HealthcareContext(builder.Options);
//     }
//   }
// }
