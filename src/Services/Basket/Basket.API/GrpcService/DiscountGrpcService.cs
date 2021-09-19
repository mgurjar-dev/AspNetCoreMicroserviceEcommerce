using Discount.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.GrpcService
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient client;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient client)
        {
            this.client = client;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var request = new GetDiscountRequest { ProductName = productName };
            var coupon = await client.GetDiscountAsync(request);
            return coupon;
        }
    }
}
