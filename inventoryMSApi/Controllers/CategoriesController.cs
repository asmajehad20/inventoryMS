using inventoryMSLogic.src.DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using inventoryMSLogic.src.BusinessLogicLayer;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace inventoryMSApi.Controllers
{
    /// <summary>
    /// Controller for managing products.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly InventoryManager inventoryManager;
        public CategoriesController()
        {
            InventoryRepository repository = new();
            inventoryManager = new(repository);
        }
        /// <summary>
        /// Retrieves all categories from inventory
        /// </summary>
        /// <returns>List of categories.</returns>
        [HttpGet]
        public IActionResult GetCategories()
        {
             string[] allcategories = inventoryManager.GetCategories();
             
             if (allcategories.Length == 0)
            {
                return NotFound("NO Categories Found");
            }
            return Ok(allcategories);
           
        }

        public class CategoryModel
        {
            public string? Name { get; set; }
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="category">The category to be added.</param>
        [HttpPost]
        public IActionResult AddCategory([FromBody] CategoryModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name))
            {
                return BadRequest("Category data is missing or invalid.");
            }

            try
            {
                inventoryManager.AddCategory(model.Name);
                return Ok("category added");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }

        /// <summary>
        /// Updates an existing category in the inventory.
        /// </summary>
        /// <param name="keyword">The keyword of the product to update.</param>
        /// <param name="newName">The updated name of the category .</param>
        /// <returns>
        /// 200 OK with the updated product if successful,
        /// 400 Bad Request if the product data is missing,
        /// 404 Not Found if the product does not exist in the inventory.
        /// </returns>
        [HttpPut]
        public IActionResult UpdateCategory([FromHeader] string keyword, [FromBody] CategoryModel model)
        {

            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Keyword is missing.");
            }
            if(string.IsNullOrEmpty(model.Name))
            {
                return BadRequest("new name for the category is missing.");
            }

            try
            {
                if(inventoryManager.UpdateCategory(keyword, model.Name))
                {
                    return Ok("category updated");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
     
            return BadRequest("category failed to update");
        }

        /// <summary>
        /// Deletes a product by its keyword.
        /// </summary>
        /// <param name="keyword">The keyword of the product to delete.</param>
        /// <returns>Confirmation message.</returns>
        [HttpDelete]
        public IActionResult Deletecatrgory([FromHeader]string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Keyword is missing.");
            }

            try
            {
                if (inventoryManager.DeleteCategory(keyword))
                {
                    return Ok("category deleted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return BadRequest("failed to delete category");
        }

       

    }
}
