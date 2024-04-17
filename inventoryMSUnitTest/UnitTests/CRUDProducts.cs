using inventoryMSLogic.src.BusinessLogicLayer;
using inventoryMSLogic.src.DataAccessLayer;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace inventoryMS.Tests.UnitTests
{
    public class CRUDProducts
    {
        [Fact]
        public void InventoryManager_GetProducts_ReturnsArrayOfProducts()
        {
            //Arrange 
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            mockRepository.Setup(repo => repo.GetAllProducts())
              .Returns(new Product[] {
                  new() { Name = "Product1", Price = 10, Barcode = "12345", Quantity = 5, Status = "in stock", CategoryName = "Category1" },
                  new() { Name = "Product2", Price = 15, Barcode = "67890", Quantity = 10, Status = "low on stock", CategoryName = "Category2" }
              });

            // Act
            Product[] result = inventoryManager.GetAllProducts();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void InventoryManager_AddValidNewProduct_Success()
        {
            //Arrange
            Product product = new()
            {
                Name = "new product 3",
                Barcode = "121200998800",
                Price = 10,
                Quantity = 100,
                Status = "in stock",
                CategoryName = "Electronics"
            };
            
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            mockRepository.Setup(repo => repo.ProductExists(product.Name)).Returns(false);
            mockRepository.Setup(repo => repo.ProductExists(product.Barcode)).Returns(false);
            mockRepository.Setup(repo => repo.AddProduct(It.IsAny<Product>())).Returns(true);

            // Act
            Product result = inventoryManager.AddProduct(product.Name, product.Barcode, product.Price, product.Quantity, product.Status, product.CategoryName);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Product>(result);
           
        }

        [Theory]
        [InlineData("", "123456789087", 10, 5, "in stock", "Electronics")] // Invalid name
        [InlineData("Test Product", "", 10, 5, "in stock", "Electronics")] // Invalid barcode
        [InlineData("Test Product", "123456789087", 0, 5, "in stock", "Electronics")] // Invalid price
        [InlineData("Test Product", "123456789087", 10, 0, "in stock", "Electronics")] // Invalid quantity
        [InlineData("Test Product", "123456789087", 10, 5, "", "Electronics")] // Invalid status
        [InlineData("Test Product", "123456789087", 10, 5, "in stock", "")] //invalid category
        public void InventoryManager_AddInValidNewProductWithEmptyValues_Fail(string name, string barcode, int price, int quantity, string status, string category)
        {
            //Arrange 
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);


            // Act & Assert
            Assert.Throws<ArgumentException>(() => inventoryManager.AddProduct(name, barcode, price, quantity, status, category));
        }

        [Fact]
        public void InventoryManager_AddInValidExistingProductName_Fail()
        {
            //Arrange
            Product product = new()
            {
                Name = "existing product",
                Barcode = "121200998800",
                Price = 10,
                Quantity = 100,
                Status = "in stock",
                CategoryName = "Electronics"
            };
            
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            mockRepository.Setup(repo => repo.ProductExists(product.Name)).Returns(true);
            //mockRepository.Setup(repo => repo.ProductExists(product.Barcode)).Returns(true);

            // Act & Assert
            Assert.Throws<Exception>(() => inventoryManager.AddProduct(product.Name, product.Barcode, product.Price, product.Quantity, product.Status, product.CategoryName));

        }

        [Fact]
        public void InventoryManager_AddInValidExistingProductBarcode_Fail()
        {
            //Arrange
            Product product = new()
            {
                Name = "existing product",
                Barcode = "121200998800",
                Price = 10,
                Quantity = 100,
                Status = "in stock",
                CategoryName = "Electronics"
            };
            
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            //mockRepository.Setup(repo => repo.ProductExists(product.Name)).Returns(true);
            mockRepository.Setup(repo => repo.ProductExists(product.Barcode)).Returns(true);

            // Act & Assert
            Assert.Throws<Exception>(() => inventoryManager.AddProduct(product.Name, product.Barcode, product.Price, product.Quantity, product.Status, product.CategoryName));

        }

        [Fact]
        public void InventoryManager_UpdateProduct_Success()
        {
            //Arrange
            string keyword = "name of product || barcode of product";
            Product product = new()
            {
                Name = "new updated product name",
                Barcode = "",
                Price = 0,
                Quantity = 0,
                Status = "",
                CategoryName = ""
            };

            string productID = "uuid_string";

            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            mockRepository.Setup(repo => repo.ProductExists(keyword)).Returns(true);
            mockRepository.Setup(repo => repo.GetProduct(keyword)).Returns(product);
            mockRepository.Setup(repo => repo.GetProductID(keyword)).Returns(productID);
            mockRepository.Setup(repo => repo.UpdateProduct(productID, product.Name, product.Barcode, product.Price, product.Quantity, product.Status, product.CategoryName)).Returns(true);

            // Act
            bool result = inventoryManager.UpdateProduct(keyword, product.Name, product.Barcode, product.Price, product.Quantity, product.Status, product.CategoryName);

            // Assert
            Assert.True(result);
        }



/*
 *      
 *      
        

        

        [Fact]
        public void InventoryManager_UpdateCategory_Success()
        {
            // Arrange
            string Category = "category";//Existing Category
            string UpdatedCateoryName = "updated category";
            string CategoryID = "uuid_strring";

            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            mockRepository.Setup(repo => repo.CategoryExists(Category)).Returns(true);
            mockRepository.Setup(repo => repo.GetCategoryID(Category)).Returns(CategoryID);
            mockRepository.Setup(repo => repo.UpdateCategory(CategoryID, UpdatedCateoryName)).Returns(true);

            // Act
            bool result = inventoryManager.UpdateCategory(Category, UpdatedCateoryName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void InventoryManager_UpdateNoneExistingCategory_Fail()
        {
            // Arrange
            string Category = "non existing category";//Existing Category
            string UpdatedCateoryName = "updated category";
           
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            mockRepository.Setup(repo => repo.CategoryExists(Category)).Returns(false);

            // Act & Assert
            Assert.Throws<Exception>(() => inventoryManager.UpdateCategory(Category, UpdatedCateoryName));


        }

        [Theory]
        [InlineData(null, "new name")] // Null category
        [InlineData("", "new name")] // Empty category
        [InlineData("category", null)] // new name null 
        [InlineData("category", "")] // new name null 
        public void InventoryManager_UpdateCategory_Fail(string Category, string UpdatedCateoryName)
        {

            // Arrange
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => inventoryManager.UpdateCategory(Category, UpdatedCateoryName));
        }



        [Fact]
        public void InventoryManager_DeleteCategory_Success()
        {
            string Category = "category";//Existing Category

            // Arrange
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            mockRepository.Setup(repo => repo.CategoryExists(Category)).Returns(true);
            mockRepository.Setup(repo => repo.DeleteCategory(Category)).Returns(true);

            // Act
            bool result = inventoryManager.DeleteCategory(Category);

            // Assert
            Assert.True(result);
        }*/
    }
}
