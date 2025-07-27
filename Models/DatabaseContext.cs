using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Soil_Monitoring_Web_App.Configuration.Entities;
using System.Drawing;

namespace Soil_Monitoring_Web_App.Models
{
    public class DatabaseContext : IdentityDbContext<AppUser>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=LOCALHOST;Database=SoilMonitoring;User=root;Password=baobao2001";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName != null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            };

            builder.ApplyConfiguration(new RoleConfiguration());


        }
        public DatabaseContext(DbContextOptions<DatabaseContext> options, IWebHostEnvironment webHostEnvironment) : base(options)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<MeasurementDate> Measurements { get; set; }
        public DbSet<SoilData> SoilDatas { get; set; }
    }
}
