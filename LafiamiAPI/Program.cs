using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace LafiamiAPI
{
#pragma warning disable CS1591
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Error()
                    .WriteTo.RollingFile("Log\\Lafiami-log-{Date}.txt")
                    .CreateLogger();

                CreateWebHostBuilder(args).Build().ExecuteSeedData().Run();

                Log.CloseAndFlush();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                //.UseKestrel()
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration()
                .UseStartup<Startup>();
        }
    }


    public static class WebHostExtensions
    {
        public static IWebHost ExecuteSeedData(this IWebHost host)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                LafiamiContext context = services.GetService<LafiamiContext>();

                // now we have the DbContext. Run migrations
                context.Database.Migrate();

                // now that the database is up to date. Let's seed
                UserManager<UserViewModel> userManager = services.GetService<UserManager<UserViewModel>>();
                RoleManager<ApplicationRoleModel> roleManager = services.GetService<RoleManager<ApplicationRoleModel>>();
                SeedData.SetupDatabase(context, userManager, roleManager).GetAwaiter().GetResult();
            }

            return host;
        }
    }
#pragma warning restore CS1591
}
