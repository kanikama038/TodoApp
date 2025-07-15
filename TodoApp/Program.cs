using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RazorLight;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Services;

using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Requests;

namespace TodoApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<TodoAppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("TodoAppDbContext") ?? throw new InvalidOperationException("Connection string 'TodoAppDbContext' not found.")));

            // Configure the application to use appsettings.json and user secrets.
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<Program>();

            // EmailSettings binding.
            builder.Services.Configure<_EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<_GmailApiSettings>(builder.Configuration.GetSection("GmailApiSettings"));

            // Register the email service.
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddSingleton<GmailAuthService>();
            builder.Services.AddScoped<GmailEmailService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

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

            app.UseAuthorization();
            //app.UseAuthentication();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
