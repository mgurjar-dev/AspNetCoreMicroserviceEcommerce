using Catelog.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catelog.API.Data
{
    public interface ICatelogContext
    {
        IMongoCollection<Product> Products { get; }
    }
}
