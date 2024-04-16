
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
            // Act
            string[] result = InventoryManager.GetCategories();

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
            bool actualResult;

            // Act
            try
            {
                InventoryManager.AddCategory(category);
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
            string existingCategory = "New category5";

            // Act
            bool result = InventoryManager.DeleteCategory(existingCategory);

            // Assert
            Assert.True(result);
        }


    }

}
