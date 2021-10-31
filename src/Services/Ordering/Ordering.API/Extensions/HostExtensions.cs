using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDB<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder) where TContext:DbContext
        {
           

            var scope = host.Services.CreateScope();
            
                var serviceP = scope.ServiceProvider;
                var logger = serviceP.GetRequiredService<ILogger<TContext>>();
                var context = serviceP.GetService<TContext>();
                try
                {
                var retryP = Policy.Handle<SqlException>()
                     .WaitAndRetry(
                            retryCount: 5,
                            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            onRetry: (exception, retryCount, context) =>
                            {
                                logger.LogError($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                            });

                retryP.Execute(() => InvokeSeeder<TContext>(seeder, context, serviceP));
                
                }
                catch (SqlException ex)
                {
                logger.LogError("Error in MigrationDB");
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
