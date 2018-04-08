using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Http.Results;
using refactor_me.Controllers;
using refactor_me.Models;
using refactor_me.Tests.Controllers;
using System.Linq;

namespace refactor_me.Tests
{
    [TestClass]
    public class TestProductsController
    {
        private Mock<DatabaseEntities> mockContext;
        private Mock<DbSet<Product>> productMockSet;
        private Mock<DbSet<ProductOption>> productOptionMockSet;
        private readonly static List<Product> product = new List<Product>
        {
            new Product { Id = Guid.NewGuid(), Name = "Demo1", Description = "Description1", Price = 10.00M, DeliveryPrice = 10.00M },
            new Product { Id = Guid.NewGuid(), Name = "Demo2", Description = "Description1", Price = 3.75M, DeliveryPrice = 3.75M },
            new Product { Id = Guid.NewGuid(), Name = "Demo3", Description = "Description1", Price = 16.99M, DeliveryPrice = 16.99M },
            new Product { Id = Guid.NewGuid(), Name = "Demo4", Description = "Description1", Price = 11.00M, DeliveryPrice = 11.00M }
        };

        private readonly static List<ProductOption> productOptions = new List<ProductOption>
        {
            new ProductOption { Id = Guid.NewGuid(), ProductId = product.First().Id, Name = "Demo1", Description = "Description1" },
            new ProductOption { Id = Guid.NewGuid(), ProductId = product.First().Id, Name = "Demo2", Description = "Description1" },
            new ProductOption { Id = Guid.NewGuid(), ProductId = product.First().Id, Name = "Demo3", Description = "Description1" },
            new ProductOption { Id = Guid.NewGuid(), ProductId = product.First().Id, Name = "Demo4", Description = "Description1" }
        };

        [TestInitialize()]
        public void Initialize()
        {
            this.mockContext = new Mock<DatabaseEntities>();
            this.productMockSet = new Mock<DbSet<Product>>();
            this.productOptionMockSet = new Mock<DbSet<ProductOption>>();
            this.mockContext.Setup(m => m.Products).Returns(TestHelpers.SetupDbSet(this.productMockSet, product));
            this.mockContext.Setup(m => m.ProductOptions).Returns(TestHelpers.SetupDbSet(this.productOptionMockSet, productOptions));
        }

        [TestMethod]
        public void GetAllProducts_ShouldReturnAllProducts()
        {
            ProductsController controller = new ProductsController(this.mockContext.Object);

            IQueryable<Product> result = controller.GetAll();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Count());
        }

        [TestMethod]
        public void GetProductByName_ShouldReturnCorrectProduct()
        {
            ProductsController controller = new ProductsController(this.mockContext.Object);
            IHttpActionResult response = controller.SearchByName(product.First().Name);

            var result = response as OkNegotiatedContentResult<Product>;
            var productResult = result.Content;

            Assert.IsNotNull(response);
            Assert.IsNotNull(productResult);
            Assert.AreEqual(product.First().Id, productResult.Id);
        }

        [TestMethod]
        public void GetProductById_ShouldReturnCorrectProduct()
        {
            ProductsController controller = new ProductsController(this.mockContext.Object);
            IHttpActionResult response = controller.GetProduct(product.First().Id);

            var result = response as OkNegotiatedContentResult<Product>;
            var productResult = result.Content;

            Assert.IsNotNull(response);
            Assert.IsNotNull(productResult);
            Assert.AreEqual(product.First().Id, productResult.Id);
        }

        [TestMethod]
        public void CreateProduct_ShouldReturnCorrectProduct()
        {
            ProductsController controller = new ProductsController(this.mockContext.Object);
            Product _product = new Product { Id = Guid.NewGuid(), Name = "NewProduct", Description = "New_Product_Description", Price = 15.00M, DeliveryPrice = 20.00M };
            IHttpActionResult response = controller.CreateProduct(_product);

            var result = response as CreatedAtRouteNegotiatedContentResult<Product>;
            var productResult = result.Content;

            Assert.IsNotNull(response);
            Assert.IsNotNull(productResult);
            Assert.AreEqual(_product.Id, productResult.Id);
        }

        [TestMethod]
        public void UpdateProduct_ShouldReturnCorrectStatusCodet()
        {
            ProductsController controller = new ProductsController(this.mockContext.Object);
            var new_product = new Product
            {
                Name = "Demo1Updated",
                Description = "Description1Updated",
                Price = 12.00M,
                DeliveryPrice = 12.00M
            };
            IHttpActionResult response = controller.UpdateProduct(product.First().Id, new_product);
            var result = (StatusCodeResult) response;

            Assert.IsNotNull(response);
            Assert.AreEqual(200, (int)result.StatusCode);
        }

        [TestMethod]
        public void DeleteProduct_ShouldReturnCorrectStatusCodet()
        {
            ProductsController controller = new ProductsController(this.mockContext.Object);

            IHttpActionResult response = controller.DeleteProduct(product.First().Id);
            var result = (StatusCodeResult)response;

            Assert.IsNotNull(response);
            Assert.AreEqual(200, (int)result.StatusCode);
        }
    }
}
