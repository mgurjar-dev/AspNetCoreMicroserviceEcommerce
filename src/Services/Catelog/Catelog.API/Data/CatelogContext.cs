using Catelog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catelog.API.Data
{
    public class CatelogContext : ICatelogContext
    {
        public CatelogContext(IConfiguration config)
        {
            var client = new MongoClient(config.GetValue<string>("DatabaseSettings:ConnectionString"));
            var db = client.GetDatabase(config.GetValue<string>("DatabaseSettings:DatabaseName"));
            Products = db.GetCollection<Product>(config.GetValue<string>("DatabaseSettings:CollectionName"));
            CatelogContextSeed.SeedData(Products);

        }
        public IMongoCollection<Product> Products { get; }
    }
}
