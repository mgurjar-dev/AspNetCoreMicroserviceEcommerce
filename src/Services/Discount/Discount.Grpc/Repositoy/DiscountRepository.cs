using Dapper;
using Discount.Grpc.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositoy
{
    public class DiscountRepository: IDiscountRepository
    {
        private readonly IConfiguration config;

        public DiscountRepository(IConfiguration config)
        {
            this.config = config;
        }

        
        public async Task<Coupon> GetDiscount(string productName)
        {
            using var conn = new NpgsqlConnection(config.GetValue<string>("DatabaseSettings:ConnectionString"));
            var coupon = await conn.QueryFirstOrDefaultAsync<Coupon>("select * from coupon where ProductName=@ProductName", new { ProductName = productName});
            if (coupon == null) return new Coupon { ProductName="No Discount" , Amount=0, Description="No Discount"};
            return coupon;
        }

         public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection
                (config.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected =
                await connection.ExecuteAsync
                    ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(config.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected = await connection.ExecuteAsync
                    ("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(config.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
                new { ProductName = productName });

            if (affected == 0)
                return false;

            return true;
        }
       
    }
}
