using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductMicroservice.Model;
using static ProductsEF.Controllers.ProductsController;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductsEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        ProductDbContext dbContext;

       public ProductsController(ProductDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return dbContext.Products;
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var product = await dbContext.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody] Product product)
        {
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = product.id }, product);
        }
        bool productExists(long id)
        {
            return dbContext.Products.Any(e => e.id == id);
        }
        public class ProductEditInput {
            public long id { get; set; }
            public Product product { get; set; }
        }


        // PUT api/<ProductsController>
        [HttpPut()]
        public async Task<IActionResult> Put([FromBody]  Product product)
        {
            dbContext.Entry(product).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            
            catch (DbUpdateConcurrencyException)
            {
                if (!productExists(product.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var product = dbContext.Products.Where(c => c.id == id).First();
            if (product == null)
            {
                return NotFound();
            }

            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
