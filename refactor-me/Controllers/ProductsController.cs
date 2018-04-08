using System;
using System.Net;
using System.Web.Http;
using System.Linq;
using refactor_me.Models;
using System.Web.Http.Description;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace refactor_me.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        private DatabaseEntities db;

        public ProductsController()
        {
            this.db = new DatabaseEntities();
        }

        public ProductsController(DatabaseEntities db)
        {
            this.db = db;
        }

        //GET Products: /products
        [Route]
        [HttpGet]
        public IQueryable<Product> GetAll()
        {
            return db.Products;
        }

        //GET a single product by name: /products?name={name}
        [Route]
        [HttpGet]
        [ResponseType(typeof(Product))]
        public IHttpActionResult SearchByName(string name)
        {
            Product product = db.Products.Single(x => x.Name == name);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        //GET a single product by ID: /products/{id}
        [Route("{id}", Name = "GetProductById")]
        [HttpGet]
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(Guid id)
        {
            Product product = db.Products.Single(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        //POST: /products
        [Route]
        [HttpPost]
        public IHttpActionResult CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (product.Id == Guid.Empty)
            {
                product.Id = Guid.NewGuid();
            }
            db.Products.Add(product);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("GetProductById", new { id = product.Id }, product);
        }

        //PUT: /products/{id}
        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult UpdateProduct(Guid id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Product existing_product = db.Products.Single(x => x.Id == id);
            if (existing_product == null)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // DELETE /products/{id}
        [Route("{id}")]
        [HttpDelete]
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(Guid id)
        {
            Product product = db.Products.Single(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            var productOptions = db.ProductOptions.Where(x => x.ProductId == product.Id);
            if (productOptions.Count() > 0)
            {
                db.ProductOptions.RemoveRange(productOptions);
            }

            db.Products.Remove(product);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return StatusCode(HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(Guid id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }
    }
}
