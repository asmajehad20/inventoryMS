using inventoryMSLogic.src.BusinessLogicLayer;
using inventoryMSLogic.src.DataAccessLayer;
using Moq;

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
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            Product product = new()
            {
                Name = "new product 3",
                Barcode = "121200998800",
                Price = 10,
                Quantity = 100,
                Status = "in stock",
                CategoryName = "Electronics"
            };
            
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
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            Product product = new()
            {
                Name = "existing product",
                Barcode = "121200998800",
                Price = 10,
                Quantity = 100,
                Status = "in stock",
                CategoryName = "Electronics"
            };
            
            mockRepository.Setup(repo => repo.ProductExists(product.Name)).Returns(true);
            //mockRepository.Setup(repo => repo.ProductExists(product.Barcode)).Returns(true);

            // Act & Assert
            Assert.Throws<Exception>(() => inventoryManager.AddProduct(product.Name, product.Barcode, product.Price, product.Quantity, product.Status, product.CategoryName));

        }

        [Fact]
        public void InventoryManager_AddInValidExistingProductBarcode_Fail()
        {
            //Arrange
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            Product product = new()
            {
                Name = "existing product",
                Barcode = "121200998800",
                Price = 10,
                Quantity = 100,
                Status = "in stock",
                CategoryName = "Electronics"
            };
            
            //mockRepository.Setup(repo => repo.ProductExists(product.Name)).Returns(true);
            mockRepository.Setup(repo => repo.ProductExists(product.Barcode)).Returns(true);

            // Act & Assert
            Assert.Throws<Exception>(() => inventoryManager.AddProduct(product.Name, product.Barcode, product.Price, product.Quantity, product.Status, product.CategoryName));

        }

        [Fact]
        public void InventoryManager_UpdateProduct_Success()
        {
            //Arrange
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

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

            mockRepository.Setup(repo => repo.ProductExists(keyword)).Returns(true);
            mockRepository.Setup(repo => repo.GetProduct(keyword)).Returns(product);
            mockRepository.Setup(repo => repo.GetProductID(keyword)).Returns(productID);
            mockRepository.Setup(repo => repo.UpdateProduct(productID, product.Name, product.Barcode, product.Price, product.Quantity, product.Status, product.CategoryName)).Returns(true);

            // Act
            bool result = inventoryManager.UpdateProduct(keyword, product.Name, product.Barcode, product.Price, product.Quantity, product.Status, product.CategoryName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void InventoryManager_UpdateNoneExsistingProduct_Fail()
        {
            //Arrange
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            string keyword = "None Exsisting Product";
            Product product = new()
            {
                Name = "new updated product name",
                Barcode = "",
                Price = 0,
                Quantity = 0,
                Status = "",
                CategoryName = ""
            };

            mockRepository.Setup(repo => repo.ProductExists(keyword)).Returns(false);

            // Act & Assert
            Assert.Throws<Exception>(() => inventoryManager.UpdateProduct(keyword, product.Name, product.Barcode, product.Price, product.Quantity, product.Status, product.CategoryName));

        }

        [Fact]
        public void InventoryManager_DeleteProduct_Success()
        {
            //Arrange
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            string keyword = "Product";
            

            mockRepository.Setup(repo => repo.ProductExists(keyword)).Returns(true);
            mockRepository.Setup(repo => repo.DeleteProduct(keyword)).Returns(true);

            // Act
            bool result = inventoryManager.DeleteProduct(keyword);

            // Assert
            Assert.True(result);
            
        }

    }
}
