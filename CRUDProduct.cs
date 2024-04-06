
using inventoryMSLogic.src.BusinessLogicLayer;
using inventoryMSLogic.src.DataAccessLayer;

namespace inventoryMSUnitTest
{
    public class CRUDProduct
    {
        [Fact]
        public void GetAllProducts_Returns_Products()
        {

            // Act
            var result = InventoryManager.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Product[]>(result);
        }



        [Fact]
        public void AddProduct_ValidInput_ReturnsProduct()
        {
            // Arrange
            string name = "Testttt";
            string barcode = "113456789089";
            int price = 10;
            int quantity = 5;
            string status = "in stock";
            string category = "Electronics";

            // Act
            Product addedProduct = InventoryManager.AddProduct(name, barcode, price, quantity, status, category);

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
            // Act & Assert
            Assert.Throws<ArgumentException>(() => InventoryManager.AddProduct(name, barcode, price, quantity, status, category));
        }

        [Theory]
        [InlineData("Existing Product", "123456789087", 10, 5, "in stock", "Electronics")]
        [InlineData("Test Product", "Existing Barcode", 10, 5, "in stock", "Electronics")]
        public void AddProduct_ProductAlreadyExists_ThrowsException(string name, string barcode, int price, int quantity, string status, string category)
        {
            // Act & Assert
            Assert.Throws<Exception>(() => InventoryManager.AddProduct(name, barcode, price, quantity, status, category));
        }

        [Fact]
        public void UpdateProduct_ProductExists_SuccessfulUpdate()
        {
            // Arrange
            string keyword = "tv";
            string name = "tv";
            string barcode = "123456789777";
            int price = 20;
            int quantity = 10;
            string status = "out of stock";
            string category = "Electronics";

            // Act & Assert
            Assert.True(InventoryManager.UpdateProduct(keyword, name, barcode, price, quantity, status, category));
        }



        [Fact]
        public void UpdateProduct_ProductDoesNotExist_ExceptionThrown()
        {
            // Arrange
            string keyword = "non_existing_product_keyword";
            string name = "Name";
            string barcode = "Barcode";
            int price = 20;
            int quantity = 10;
            string status = "Status";
            string category = "Category";

            // Act & Assert
            Assert.Throws<Exception>(() => InventoryManager.UpdateProduct(keyword, name, barcode, price, quantity, status, category));
        }

        [Fact]
        public void UpdateProduct_NullParameters_SuccessfulUpdateWithStoredValues()
        {
            // Arrange
            string keyword = "Jeans";

            // Act & Assert
            Assert.True(InventoryManager.UpdateProduct(keyword, null, "909090909091", 0, 0, null, null));
        }

        [Fact]
        public void DeleteProduct_ProductExists_SuccessfulDeletion()
        {
            // Arrange
            string keyword = "Testttt"; 

            // Act & Assert
            Assert.True(InventoryManager.DeleteProduct(keyword));
        }

        [Fact]
        public void DeleteProduct_ProductDoesNotExist_ExceptionThrown()
        {
            // Arrange
            string keyword = "non_existing_product_keyword"; 

            // Act & Assert
            Assert.Throws<Exception>(() => InventoryManager.DeleteProduct(keyword));
        }

    }

}
