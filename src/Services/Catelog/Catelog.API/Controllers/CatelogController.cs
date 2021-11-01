using Catelog.API.Entities;
using Catelog.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Serilog;
using System.Diagnostics;

namespace Catelog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatelogController : ControllerBase
    {

        private readonly IProductRepository repo;
        private readonly ILogger logger;

        public CatelogController(IProductRepository repo, ILogger logger)
        {
            this.repo = repo;
            this.logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            //Activity act = new Activity("GetProducts");
            //act.Start();
            var products = await repo.GetProducts();
            //act.Stop();
            return Ok(products);
        }

        [HttpGet("id:length(24)")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetByProductId(string id)
        {
            logger.Information("GetByProductId call");
            var prod = await repo.GetProduct(id);
            if (prod==null)
            {
                logger.Information("Not Found");
                return NotFound();
            }
            return Ok(prod);
        }
        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            logger.Information("GetByCategory call");
            var products = await repo.GetProductByCategory(category);
            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            logger.Information("Create product call");
            await repo.CreateProduct(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            logger.Information("Update product call");
            return Ok(await repo.UpdateProduct(product));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            logger.Information("Delete product call");
            return Ok(await repo.DeleteProduct(id));
        }
    }
}
