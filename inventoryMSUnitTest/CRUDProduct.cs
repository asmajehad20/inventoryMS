
using inventoryMSLogic.src.BusinessLogicLayer;
using inventoryMSLogic.src.DataAccessLayer;

namespace inventoryMSUnitTest
{
    public class CRUDProduct
    {
        [Fact]
        public void GetAllProducts_Returns_Products()
        {
            //Arrange 
            InventoryRepository repository = new();
            InventoryManager inventoryManager = new(repository);

            // Act
            var result = inventoryManager.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Product[]>(result);
        }



        [Fact]
        public void AddProduct_ValidInput_ReturnsProduct()
        {
            // Arrange
            InventoryRepository repository = new();
            InventoryManager inventoryManager = new(repository);

            string name = "TestD";
            string barcode = "113400780089";
            int price = 10;
            int quantity = 5;
            string status = "in stock";
            string category = "Electronics";

            // Act
            Product addedProduct = inventoryManager.AddProduct(name, barcode, price, quantity, status, category);

            // Assert
            Assert.NotNull(addedProduct);
            Assert.Equal(name, addedProduct.Name);
            Assert.Equal(barcode, addedProduct.Barcode);

        }

        [Theory]
        [InlineData("", "123456789087", 10, 5, "in stock", "Electronics")] // Invalid name
        [InlineData("Test Product", "", 10, 5, "in stock", "Electronics")] // Invalid barcode
        [InlineData("Test Product", "123456789087", 0, 5, "in stock", "Electronics")] // Invalid price
        [InlineData("Test Product", "123456789087", 10, 0, "in stock", "Electronics")] // Invalid quantity
        [InlineData("Test Product", "123456789087", 10, 5, "", "Electronics")] // Invalid status
        [InlineData("Test Product", "123456789087", 10, 5, "in stock", "")] // Invalid category
        public void AddProduct_InvalidInput_ThrowsArgumentException(string name, string barcode, int price, int quantity, string status, string category)
        {
            //Arrange 
            InventoryRepository repository = new();
            InventoryManager inventoryManager = new(repository);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => inventoryManager.AddProduct(name, barcode, price, quantity, status, category));
        }

        [Theory]
        [InlineData("Existing Product", "123456789087", 10, 5, "in stock", "Electronics")]
        [InlineData("Test Product", "Existing Barcode", 10, 5, "in stock", "Electronics")]
        public void AddProduct_ProductAlreadyExists_ThrowsException(string name, string barcode, int price, int quantity, string status, string category)
        {
            //Arrange
            InventoryRepository repository = new();
            InventoryManager inventoryManager = new(repository);

            // Act & Assert
            Assert.Throws<Exception>(() => inventoryManager.AddProduct(name, barcode, price, quantity, status, category));
        }

        [Fact]
        public void UpdateProduct_ProductExists_SuccessfulUpdate()
        {
            // Arrange
            InventoryRepository repository = new();
            InventoryManager inventoryManager = new(repository);

            string keyword = "tv";
            string name = "tv";
            string barcode = "123456789777";
            int price = 20;
            int quantity = 10;
            string status = "out of stock";
            string category = "Electronics";

            // Act & Assert
            Assert.True(inventoryManager.UpdateProduct(keyword, name, barcode, price, quantity, status, category));
        }



        [Fact]
        public void UpdateProduct_ProductDoesNotExist_ExceptionThrown()
        {
            // Arrange
            InventoryRepository repository = new();
            InventoryManager inventoryManager = new(repository);

            string keyword = "non_existing_product_keyword";
            string name = "Name";
            string barcode = "Barcode";
            int price = 20;
            int quantity = 10;
            string status = "Status";
            string category = "Category";

            // Act & Assert
            Assert.Throws<Exception>(() => inventoryManager.UpdateProduct(keyword, name, barcode, price, quantity, status, category));
        }

        [Fact]
        public void UpdateProduct_NullParameters_SuccessfulUpdateWithStoredValues()
        {
            // Arrange
            InventoryRepository repository = new();
            InventoryManager inventoryManager = new(repository);

            string keyword = "Jeans";

            // Act & Assert
            Assert.True(inventoryManager.UpdateProduct(keyword, null, "909090909091", 0, 0, null, null));
        }

        [Fact]
        public void DeleteProduct_ProductExists_SuccessfulDeletion()
        {
            // Arrange
            InventoryRepository repository = new();
            InventoryManager inventoryManager = new(repository);

            string keyword = "TestD"; 

            // Act & Assert
            Assert.True(inventoryManager.DeleteProduct(keyword));
        }

    }

}
