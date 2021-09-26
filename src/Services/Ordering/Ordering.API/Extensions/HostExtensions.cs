using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDB<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder, int? retry = 0) where TContext:DbContext
        {
            int retries = retry.Value;

            var scope = host.Services.CreateScope();
            
                var serviceP = scope.ServiceProvider;
                var logger = serviceP.GetRequiredService<ILogger<TContext>>();
                var context = serviceP.GetService<TContext>();
                try
                {
                    InvokeSeeder<TContext>(seeder, context, serviceP);
                }
                catch (SqlException ex)
                {
                    retries++;
                    if (retries < 50)
                    {
                        System.Threading.Thread.Sleep(2000);
                        MigrateDB<TContext>(host, seeder, retries);
                    }
                }
            return host;
        }

        public static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider service ) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, service);
        }
    }
}
