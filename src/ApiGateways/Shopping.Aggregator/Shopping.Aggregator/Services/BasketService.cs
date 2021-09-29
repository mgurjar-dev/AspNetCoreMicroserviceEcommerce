using Shopping.Aggregator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Shopping.Aggregator.Extensions;
using System.Text.Json;
using Shopping.Aggregator.Services;

namespace Shopping.Aggregator.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient httpClient;

        public BasketService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<BasketModel> GetBasket(string userName)
        {
            var response = await httpClient.GetAsync($"/api/basket/{userName}");
            return await response.ReadContentAs<BasketModel>();
        }
    }
}
