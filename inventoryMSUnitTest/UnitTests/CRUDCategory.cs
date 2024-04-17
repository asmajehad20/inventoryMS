using inventoryMSLogic.src.BusinessLogicLayer;
using inventoryMSLogic.src.DataAccessLayer;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inventoryMS.Tests.UnitTests
{
    public class CRUDCategory
    {
        [Fact]
        public void InventoryManager_GetCategories_ReturnsCategories()
        {
            //Arrange 
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetAllCategories())
                  .Returns(new string[] { "Category1", "Category2" });

            // Act
            string[] result = inventoryManager.GetCategories();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void InventoryManager_AddValidUniqueCategory_Success()
        {
            //Arrange 
            string Category = "New category unit test";
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            mockRepository.Setup(repo => repo.CategoryExists(Category)).Returns(false);
            mockRepository.Setup(repo => repo.AddCategory(Category)).Returns(true);

            bool actualResult;
            // Act
            
            try
            {
                inventoryManager.AddCategory(Category);
                actualResult = true;
            }
            catch
            {
                actualResult = false;
            }

            // Assert
            Assert.True(actualResult);
           
        }

        [Theory]
        [InlineData(null)] // Null category
        [InlineData("")] // Empty category
        public void InventoryManager_AddNullOrEmptyCategory_FailScenarios(string Category)
        {
            //Arrange 
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            //mockRepository.Setup(repo => repo.CategoryExists(Category)).Returns(false);
            //mockRepository.Setup(repo => repo.AddCategory(Category)).Returns(false);

            // Act & Assert
            Assert.Throws<Exception>(() => inventoryManager.AddCategory(Category));
        }

        [Fact]
        public void InventoryManager_AddExistingCategory_Fail()
        {
            //Arrange 
            string Category = "Electronics";
            var mockRepository = new Mock<InventoryRepository>();
            InventoryManager inventoryManager = new(mockRepository.Object);

            mockRepository.Setup(repo => repo.CategoryExists(Category)).Returns(true);
            //mockRepository.Setup(repo => repo.AddCategory(Category)).Returns(false);

            // Act & Assert
            Assert.Throws<Exception>(() => inventoryManager.AddCategory(Category));
        }

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
        }
    }
}
