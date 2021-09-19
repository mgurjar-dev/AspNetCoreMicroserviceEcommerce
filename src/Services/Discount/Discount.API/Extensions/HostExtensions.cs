using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDB<TContext>(this IHost host, int? retry)
        {
            var retryCnt = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var serviceP = scope.ServiceProvider;
                var config = serviceP.GetRequiredService<IConfiguration>();
                var log = serviceP.GetRequiredService<ILogger<TContext>>();

                try
                {
                    using var conn = new NpgsqlConnection(config.GetValue<string>("DatabaseSettings:ConnectionString"));
                    conn.Open();
                    using var command = new NpgsqlCommand { Connection = conn };

                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
                    command.ExecuteNonQuery();

                    log.LogInformation("Migrated postresql database.");
                }
                catch (NpgsqlException ex)
                {
                    if (retryCnt < 50)
                    {
                        retryCnt++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDB<TContext>(host, retryCnt);
                    }
                
                }

            }

                return host;
        }
    }
}
