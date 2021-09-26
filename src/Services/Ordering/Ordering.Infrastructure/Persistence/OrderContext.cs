using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContext: DbContext
    {
        public OrderContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries<EntityBase>())
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        item.Entity.CreatedDate = DateTime.Now;
                        item.Entity.CreatedBy = "test";
                        break;
                    case EntityState.Modified:
                        item.Entity.LastModifiedDate = DateTime.Now;
                        item.Entity.LastModifiedBy = "test";
                        break;
                }
               
            }
            int res = 0;
            try
            {
                res = await base.SaveChangesAsync(cancellationToken);

            }
            catch (Exception ex)
            { 
            
            }
            return res;
        }
    }
}
