using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using refactor_me.Models;

namespace refactor_me.Controllers
{
    [RoutePrefix("products")]
    public class ProductOptionsController : ApiController
    {
        private DatabaseEntities db;

        public ProductOptionsController()
        {
            this.db = new DatabaseEntities();
        }

        public ProductOptionsController(DatabaseEntities db)
        {
            this.db = db;
        }

        // GET Options: /products/{id}/options
        [Route("{productId}/options")]
        [HttpGet]
        [ResponseType(typeof(ProductOption))]
        public IQueryable<ProductOption> GetProductOption(Guid productId)
        {
            var productOptions = db.ProductOptions.Where(x => x.ProductId == productId);
            return productOptions;
        }

        //GET a single option: /products/{id}/options/{optionId}
        [Route("{productId}/options/{id}", Name = "GetProductOptionById")]
        [HttpGet]
        [ResponseType(typeof(ProductOption))]
        public IHttpActionResult GetOption(Guid productId, Guid id)
        {
            var productOption = db.ProductOptions.Single(x => x.ProductId == productId && x.Id == id);
            if (productOption == null)
            {
                return NotFound();
            }

            return Ok(productOption);
        }

        // POST: /products/{id}/options
        [Route("{productId}/options")]
        [HttpPost]
        [ResponseType(typeof(ProductOption))]
        public IHttpActionResult CreateOption(Guid productId, ProductOption productOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Product product = db.Products.Single(x => x.Id == productId);
            if (product == null)
            {
                return BadRequest();
            }

            if (productOption.Id == Guid.Empty)
            {
                productOption.Id = Guid.NewGuid();
            }
            productOption.ProductId = productId;
            db.ProductOptions.Add(productOption);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ProductOptionExists(productOption.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetProductOptionById", new { productId = productOption.ProductId, id = productOption.Id }, productOption);
        }

        // PUT: /products/{id}/options/{optionId}
        [Route("{productId}/options/{id}")]
        [HttpPut]
        public IHttpActionResult UpdateProductOption(Guid id, ProductOption productOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProductOption _productOption = db.ProductOptions.Single(x => x.Id == id);
            if (_productOption == null)
            {
                return BadRequest();
            }

            db.Entry(productOption).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductOptionExists(id))
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

        // DELETE: /products/{id}/options/{optionId}
        [Route("/options/{id}")]
        [HttpDelete]
        [ResponseType(typeof(ProductOption))]
        public IHttpActionResult DeleteProductOption(Guid id)
        {
            ProductOption productOption = db.ProductOptions.Single(x => x.Id == id);
            if (productOption == null)
            {
                return NotFound();
            }

            db.ProductOptions.Remove(productOption);
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

        private bool ProductOptionExists(Guid id)
        {
            return db.ProductOptions.Count(e => e.Id == id) > 0;
        }
    }
}