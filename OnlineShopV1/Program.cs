using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineShopV1.DAL;

namespace OnlineShopV1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var commandLineApplication = new CommandLineApplication(false);
            var doMigrate = commandLineApplication.Option(
                "--ef-migrate",
                "Apply entity framework migrations and exit",
                CommandOptionType.NoValue);
            commandLineApplication.HelpOption("-? | -h | --help");
            commandLineApplication.OnExecute(() =>
            {
                ExecuteApp(args, doMigrate);
                return 0;
            });
            commandLineApplication.Execute(args);
        }
        
        private static void ExecuteApp(string[] args, CommandOption doMigrate)
        {
            Console.WriteLine("Loading web host");
            var webHost = CreateWebHostBuilder(args).Build();

            if (doMigrate.HasValue())
            {
                Console.WriteLine("Applying Entity Framework migrations");
                using (var scope = webHost.Services.CreateScope()) {
                    using (var context = scope.ServiceProvider.GetRequiredService<OnlineShopDbContext>())
                    {
                        context.Database.Migrate();
                        Console.WriteLine("All done, closing app");
                        Environment.Exit(0);
                    }
                }
            }

            // no flags provided, so just run
            webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}