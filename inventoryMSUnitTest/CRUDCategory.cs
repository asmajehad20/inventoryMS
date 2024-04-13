
using inventoryMSLogic.src.BusinessLogicLayer;
using inventoryMSLogic.src.DataAccessLayer;
using Moq;

namespace inventoryMSUnitTest
{
    public class CRUDCategory
    {
        [Fact]
        public void GetCategories_ReturnsNonNullArray()
        {
            // Act
            string[] categories = InventoryManager.GetCategories();

            // Assert
            Assert.NotNull(categories);
        }

        [Theory]
        [InlineData("New Category", true)] // Valid unique category
        [InlineData("Electronics", false)] // Valid existing category
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

    }

}
