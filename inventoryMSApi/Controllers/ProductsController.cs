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
    public class ProductsController : ControllerBase
    {
        /// <summary>
        /// Retrieves products optionally filtered by a keyword.
        /// user can search for a product or list of products uising a keyword in Header
        /// </summary>
        /// <param name="keyword">Keyword to filter products (optional).</param>
        /// <returns>List of products.</returns>
        [HttpGet]
        public IActionResult GetProducts([FromHeader] string? keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                // If model is null or keyword is empty, return all products
                Product[] allProducts = InventoryManager.GetAllProducts();
                return Ok(allProducts);
            }
            else
            {
                // Retrieve products based on the provided keyword
                Product[] filteredProducts = InventoryManager.Search(keyword);
                if (filteredProducts.Length == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(filteredProducts);
                }
               
            }
        }

        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="product">The product to be added.</param>
        /// <returns>The added product.</returns>
        [HttpPost]
        public IActionResult AddProduct([FromBody] Product product)
        {
            if (product == null) return BadRequest("Product data is missing.");

            var existingProduct = InventoryManager.GetProduct(product.Name ?? product.Barcode);
            if (existingProduct != null)
                return BadRequest("Product already in inventory.");

            Product newProduct = InventoryManager.AddProduct(product.Name??"", product.Barcode ?? "", product.Price, product.Quantity, product.Status??"", product.CategoryName??"");

            if (newProduct != null) 
            {
                return Ok(newProduct);
            }
            else return BadRequest("Product data is missing.");

        }

        /// <summary>
        /// Updates an existing product in the inventory.
        /// </summary>
        /// <param name="product">The updated product data.</param>
        /// <param name="keyword">The keyword of the product to update.</param>
        /// <returns>
        /// 200 OK with the updated product if successful,
        /// 400 Bad Request if the product data is missing,
        /// 404 Not Found if the product does not exist in the inventory.
        /// </returns>
        [HttpPut("{keyword}")]
        public IActionResult UpdateProduct([FromBody] Product product, string keyword)
        {

            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Keyword is missing.");
            }

            if (InventoryManager.CheckIfProductExists(keyword))
            {
                Product existingProduct = InventoryManager.GetProduct(keyword);

                string key = product.Name ?? product.Barcode;

                string name = !string.IsNullOrEmpty(product.Name) ? product.Name : existingProduct.Name;
                string barcode = !string.IsNullOrEmpty(product.Barcode) ? product.Barcode : existingProduct.Barcode;
                
                int price = (product.Price ==0) ? existingProduct.Price : product.Price;
                int quantity = (product.Quantity == 0) ? existingProduct.Quantity : product.Quantity;

                string status = !string.IsNullOrEmpty(product.Status) ? product.Status : existingProduct.Status;
                string category = !string.IsNullOrEmpty(product.CategoryName) ? product.CategoryName : existingProduct.CategoryName;

                InventoryManager.UpdateProduct(key, name, barcode, price, quantity, status, category);
                Product updatedProduct = InventoryManager.GetProduct(product.Name ?? product.Barcode ?? "");
                return Ok(updatedProduct);
            }

            return NotFound("PRODUCT NOT FOUND.");

        }

        /// <summary>
        /// Deletes a product by its keyword.
        /// </summary>
        /// <param name="keyword">The keyword of the product to delete.</param>
        /// <returns>Confirmation message.</returns>
        [HttpDelete("{keyword}")]
        public IActionResult DeleteProduct(string keyword)
        {

            Product existingProduct = InventoryManager.GetProduct(keyword);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            InventoryManager.DeleteProduct(keyword);

            return Ok("Product deleted successfully.");
        }

        /// <summary>
        /// Retrieves a product by its keyword.
        /// </summary>
        /// <param name="keyword">The keyword of the product to retrieve.</param>
        /// <returns>The retrieved product.</returns>
        [HttpGet("{keyword}")]
        public IActionResult GetProduct(string keyword)
        {

            Product product = InventoryManager.GetProduct(keyword);
            if (string.IsNullOrEmpty(keyword))
            {
                return NotFound();
            }
            if (product == null) { return NotFound(); }
            return Ok(product);
        }

    }
}
