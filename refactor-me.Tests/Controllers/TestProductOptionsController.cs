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

namespace refactor_me.Tests.Controllers
{
    [TestClass]
    public class TestProductOptionsController
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
        public void GetAllProductOptions_ShouldReturnAllOptionsOfProduct()
        {
            ProductOptionsController controller = new ProductOptionsController(this.mockContext.Object);

            IQueryable<ProductOption> result = controller.GetProductOption(product.First().Id);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Count());
        }

        [TestMethod]
        public void GetProductOptionById_ShouldReturnCorrectProductOption()
        {
            ProductOptionsController controller = new ProductOptionsController(this.mockContext.Object);
            IHttpActionResult response = controller.GetOption(product.First().Id, productOptions.First().Id);

            var result = response as OkNegotiatedContentResult<ProductOption>;
            var productResult = result.Content;

            Assert.IsNotNull(response);
            Assert.IsNotNull(productResult);
            Assert.AreEqual(productOptions.First().Id, productResult.Id);
        }

        [TestMethod]
        public void CreateProductOption_ShouldReturnCorrectProductOption()
        {
            ProductOptionsController controller = new ProductOptionsController(this.mockContext.Object);

            ProductOption _productOption = new ProductOption { Id = Guid.NewGuid(), ProductId = product.First().Id, Name = "New_Product_Option", Description = "New_Product_Option_Description" };
            IHttpActionResult response = controller.CreateOption(product.First().Id, _productOption);

            var result = response as CreatedAtRouteNegotiatedContentResult<ProductOption>;
            var productResult = result.Content;

            Assert.IsNotNull(response);
            Assert.IsNotNull(productResult);
            Assert.AreEqual(_productOption.Id, productResult.Id);
        }

        [TestMethod]
        public void UpdateProductOption_CorrectStatusCode()
        {
            ProductOptionsController controller = new ProductOptionsController(this.mockContext.Object);
            var new_productOption = new ProductOption
            {
                Name = "Demo1Updated",
                Description = "Description1Updated",
            };
            IHttpActionResult response = controller.UpdateProductOption(productOptions.First().Id, new_productOption);
            var result = (StatusCodeResult)response;

            Assert.IsNotNull(response);
            Assert.AreEqual(200, (int)result.StatusCode);
        }

        [TestMethod]
        public void DeleteProductOption_ShouldReturnCorrectStatusCode()
        {
            ProductOptionsController controller = new ProductOptionsController(this.mockContext.Object);

            IHttpActionResult response = controller.DeleteProductOption(productOptions.First().Id);
            var result = (StatusCodeResult)response;

            Assert.IsNotNull(response);
            Assert.AreEqual(200, (int)result.StatusCode);
        }
    }
}
