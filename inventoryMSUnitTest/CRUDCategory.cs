
using inventoryMSLogic.src.BusinessLogicLayer;
using inventoryMSLogic.src.DataAccessLayer;
using Moq;
using System.Reflection;

namespace inventoryMSUnitTest
{
    public class CRUDCategory
    {
        [Fact]
        public void GetCategories_ReturnsCategories()
        {
            //Arrange 
            InventoryRepository repository = new();
            InventoryManager inventoryManager = new(repository);

            // Act
            string[] result = inventoryManager.GetCategories();

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("New category5", true)] // existing category
        [InlineData("Electronics", false)] // existing category
        [InlineData(null, false)] // Null category
        [InlineData("", false)] // Empty category
        public void AddCategory_Scenarios(string category, bool expectedResult)
        {
            // Arrange
            InventoryRepository repository = new();
            InventoryManager inventoryManager = new(repository);
            bool actualResult;

            // Act
            try
            {
                inventoryManager.AddCategory(category);
                actualResult = true; 
            }
            catch
            {
                actualResult = false; 
            }

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void DeleteCategory_ExistingCategory_DeletesCategory()
        {
            // Arrange
            InventoryRepository repository = new();
            InventoryManager inventoryManager = new(repository);
            string existingCategory = "New category5";

            // Act
            bool result = inventoryManager.DeleteCategory(existingCategory);

            // Assert
            Assert.True(result);
        }


    }

}
