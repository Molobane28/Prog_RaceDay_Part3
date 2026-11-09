using Microsoft.AspNetCore.Authentication.Cookies;

namespace RaceDay.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Add services to the container.
            builder.Services.AddControllersWithViews();

            // 2. Register the API Client and Session
            builder.Services.AddHttpClient<RaceDay.MVC.Services.ApiService>();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor(); // Helps us access sessions easily

            // 3. Register Cookie Authentication for the MVC App
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // 4. IMPORTANT: Middleware Order Matters!
            app.UseSession();
            app.UseAuthentication(); // Must be before Authorization
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}