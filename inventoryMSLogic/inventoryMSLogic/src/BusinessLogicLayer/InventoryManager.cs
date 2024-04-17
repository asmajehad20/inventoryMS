
using inventoryMSLogic.src.DataAccessLayer;
using System.Text.Json;

namespace inventoryMSLogic.src.BusinessLogicLayer
{
    /// <summary>
    /// Provides methods for managing inventory-related operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="InventoryManager"/> class with the specified inventory repository.
    /// </remarks>
    /// <param name="productData">The inventory repository to be used by the inventory manager.</param>
    public class InventoryManager(InventoryRepository productData)
    {
        
        private readonly InventoryRepository ProductData = productData;

        /// <summary>
        /// Checks if a product exists in the inventory.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product.</param>
        /// <returns>True if the product exists, otherwise false.</returns>
        public bool CheckIfProductExists(string keyword)
        {
            return ProductData.ProductExists(keyword);
        }

        /// <summary>
        /// Retrieves product details from the database based on the provided keyword, which can be either the product name or barcode.
        /// </summary>
        /// <param name="keyword">The keyword to search for, which can be either the product name or barcode.</param>
        /// <returns>A <see cref="Product"/> object containing details of the found product, or null if not found.</returns>
        /// <exception cref="Exception">Thrown when the product is not found.</exception>
        public Product? GetProduct(string keyword)
        {
            if (!CheckIfProductExists(keyword))
            {
                throw new Exception("product not found");
            }

            return ProductData.GetProduct(keyword);
        }

        /// <summary>
        /// Retrieves information of all products from the inventory.
        /// </summary>
        /// <returns>An array of Product objects representing all products.</returns>
        public Product[] GetAllProducts()
        {
            return ProductData.GetAllProducts();
        }

        /// <summary>
        /// Adds a new product to the inventory.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="barcode">The barcode of the product.</param>
        /// <param name="price">The price of the product.</param>
        /// <param name="quantity">The quantity of the product.</param>
        /// <param name="status">The status of the product.</param>
        /// <param name="category">The category of the product.</param>
        /// <returns>The added product.</returns>
        public Product AddProduct(string name, string barcode, int price, int quantity, string status, string category)
        {
            // Input validation
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(barcode) ||
                price <= 0 ||
                quantity <= 0 ||
                string.IsNullOrEmpty(status) ||
                string.IsNullOrEmpty(category))
            {
                throw new ArgumentException("All fields must have valid values.");
            }

            if (CheckIfProductExists(name))
            {
                throw new Exception("Product already exists with the same name.");
            }

            if (CheckIfProductExists(barcode))
            {
                throw new Exception("Product already exists with the same barcode.");
            }

            Product product = new()
            {
                Name = name,
                Barcode = barcode,
                Price = price,
                Quantity = quantity,
                Status = status,
                CategoryName = category
            };

            if (ProductData.AddProduct(product))
            {
                return product;
            }
            else
            {
                throw new Exception("Failed to add product.");
            }

        }

        /// <summary>
        /// Updates product information based on the provided parameters.
        /// If a parameter is null or empty, it retains the existing value.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product to be updated.</param>
        /// <param name="name">The new name of the product.</param>
        /// <param name="barcode">The new barcode of the product.</param>
        /// <param name="price">The new price of the product.</param>
        /// <param name="quantity">The new quantity of the product.</param>
        /// <param name="status">The new status of the product.</param>
        /// <param name="category">The new category of the product.</param>
        public bool UpdateProduct(string keyword, string? name, string? barcode, int price, int quantity, string? status, string? category)
        {

            if (!CheckIfProductExists(keyword))
            {
                throw new Exception($"Product not found.");
            }

            Product? storedProduct = ProductData.GetProduct(keyword) ?? throw new Exception("coudnt get product");

            string ProductId = ProductData.GetProductID(keyword);

            // Replace null and empty values with stored values
            name = string.IsNullOrEmpty(name) ? storedProduct.Name : name;
            barcode = string.IsNullOrEmpty(barcode) ? storedProduct.Barcode : barcode;
            price = price == 0 ? storedProduct.Price : price;
            quantity = quantity == 0 ? storedProduct.Quantity : quantity;
            status =  string.IsNullOrEmpty(status) ? storedProduct.Status : status;
            category = string.IsNullOrEmpty(category) ? storedProduct.CategoryName : category;

            if (ProductData.UpdateProduct(ProductId, name, barcode, price, quantity, status, category))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a product from the inventory.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product to be deleted.</param>
        public bool DeleteProduct(string keyword)
        {
            if (ProductData.ProductExists(keyword) && ProductData.DeleteProduct(keyword))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Retrieves status information of a product from the inventory.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product.</param>
        public void StatusTracking(string keyword)
        {
            Product[] products = ProductData.GetProductsByStatus(keyword);

            Console.WriteLine("Products:");
            Console.WriteLine(JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true }));
        }

        /// <summary>
        /// Retrieves status information of a specific product from the inventory.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product.</param>
        public void GetProductStatus(string keyword)
        {
            string productStatus = ProductData.ProductStatus(keyword);
            string status = productStatus.Split(',')[0];
            string quantity = productStatus.Split(',')[1];

            if (productStatus != "")
            Console.WriteLine($"product {keyword} is {status}, quantity: {quantity}");

        }

        /// <summary>
        /// Searches for products in the inventory based on a keyword.
        /// </summary>
        /// <param name="keyword">The keyword to search for.</param>
        /// <returns>An array of Product objects matching the search criteria.</returns>
        public Product[] Search(string keyword)
        {
            return ProductData.Search(keyword);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////
        //categories//

        /// <summary>
        /// Retrieves all categories from the inventory.
        /// </summary>
        /// <returns>An array of strings representing all categories.</returns>
        public string[] GetCategories()
        {
            return ProductData.GetAllCategories();
        }

        /// <summary>
        /// Retrieves all categories from the inventory.
        /// </summary>
        /// <returns>A dictionary containing category number and their corresponding names.</returns>
        public Dictionary<int, string> GetAllCategories()
        {
            List<string> categoriesList = ProductData.GetAllCategories().ToList();

            Dictionary<int, string> categories = [];

            for (int i = 0; i < categoriesList.Count; i++)
            {
                categories.Add(i + 1, categoriesList[i]);
            }

            foreach (var cat in categories)
            {
                Console.WriteLine($"> {cat.Key}  {cat.Value}");
            }

            return categories;
        }

        /// <summary>
        /// Adds a new category to the inventory.
        /// </summary>
        /// <param name="category">The name of the category to be added.</param>
        public void AddCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                throw new Exception("cant add an empty category");
            }

            if (ProductData.CategoryExists(category))
            {
                throw new Exception("category exsist");
            }

            bool categoryAdded = ProductData.AddCategory(category);

            if (!categoryAdded)
            {
                throw new Exception("adding a category failed");
            }

        }

        /// <summary>
        /// Deletes a category from the inventory.
        /// </summary>
        /// <param name="category">The name of the category to be deleted.</param>
        public bool DeleteCategory(string category)
        {

            if (!string.IsNullOrEmpty(category) && ProductData.CategoryExists(category))
            {
                if (!ProductData.DeleteCategory(category))
                {
                    throw new Exception("Failed to delete category.");
                }
            }
            return true;
        }

        /// <summary>
        /// updates a category from the inventory.
        /// </summary>
        /// <param name="category">The name of the category to be updated.</param>
        /// <param name="NewNmae">The new updated name of the category.</param>
        public bool UpdateCategory(string category, string NewNmae)
        {
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(NewNmae))
            {
                throw new Exception("category cant be null");
            }

            if (!ProductData.CategoryExists(category))
            {
                throw new Exception("Failed to find category.");
            }
            
            string categoryID = ProductData.GetCategoryID(category);
                
            if(!ProductData.UpdateCategory(categoryID, NewNmae))
            {
                throw new Exception("Failed to update category.");
            }
            
            return true;
        }

    }


}