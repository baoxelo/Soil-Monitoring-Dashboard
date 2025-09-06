using Microsoft.AspNetCore.Identity;
using Soil_Monitoring_Web_App.Models;
using Microsoft.EntityFrameworkCore;
using Soil_Monitoring_Web_App.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using Soil_Monitoring_Web_App.Services;
using Soil_Monitoring_Web_App.IExtensionServices;
using Soil_Monitoring_Web_App.ExtensionServices;
using Microsoft.AspNetCore.Authentication;

namespace Soil_Monitoring_Web_App
{
    public class Program
    {
        public static async Task Main(string[] args)
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
            builder.Services.AddTransient<IFirebaseStorage, FirebaseStorage>();
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
            builder.Services.AddAuthentication().AddGoogle(googleOptions => {
                // Đọc thông tin Authentication:Google từ appsettings.json
                IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

                // Thiết lập ClientID và ClientSecret để truy cập API google
                googleOptions.ClientId = googleAuthNSection["ClientId"];
                googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
                // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
                googleOptions.CallbackPath = "/Login-Google";
                googleOptions.ClaimActions.MapJsonKey("image", "picture");
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                var importDb = scope.ServiceProvider.GetRequiredService<ICsvToSqlService>();
                await importDb.ImportCsvToDatabase();
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
