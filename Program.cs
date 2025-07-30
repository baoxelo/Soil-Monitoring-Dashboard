using Microsoft.AspNetCore.Identity;
using Soil_Monitoring_Web_App.Models;
using Microsoft.EntityFrameworkCore;
using ShoesShop.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using Soil_Monitoring_Web_App.Services;
using Soil_Monitoring_Web_App.IExtensionServices;
using Soil_Monitoring_Web_App.ExtensionServices;

namespace Soil_Monitoring_Web_App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<DatabaseContext>();
            builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
            builder.Services.ConfigureIdentity();
            builder.Services.AddAuthorization();
            builder.Services.AddTransient<IEmailSender, SendMailService>();
            builder.Services.AddScoped<ICsvToSqlService, CsvToSqlService>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });
            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromSeconds(30);
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();
            app.Run();
        }
    }
}
