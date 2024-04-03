
using inventoryMSLogic.src.DataAccessLayer;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace inventoryMSLogic.src.BusinessLogicLayer
{
    /// <summary>
    /// Provides methods for managing inventory-related operations.
    /// </summary>
    public class InventoryManager
    {
        static readonly InventoryRepository ProductData = new();

        /// <summary>
        /// Retrieves information of all products from the inventory.
        /// </summary>
        /// <returns>An array of Product objects representing all products.</returns>
        public static Product[] GetAllProducts()
        {
            string JSONproducts = ProductData.GetAllProducts();
            Product[] products = JsonSerializer.Deserialize<Product[]>(JSONproducts) ?? [];

            return products;
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
        public static Product AddProduct(string name, string barcode, int price, int quantity, string status, string category)
        {
            Product product = null;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(barcode) || price == 0 || quantity == 0 || string.IsNullOrEmpty(status) || string.IsNullOrEmpty(category))
            {
                Console.WriteLine("All fields must have valid values");
                return product;
            }
            product = new()
            {
                Name = name,
                Barcode = barcode,
                Price = price,
                Quantity = quantity,
                Status = status,
                CategoryName = category
            };
            ProductData.AddProduct(product);
            return product;    
        }

        /// <summary>
        /// Updates product information based on the provided parameters.
        /// If a parameter is null or empty, it retains the existing value.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product to be updated.</param>
        /// <param name="ProductName">The new name of the product.</param>
        /// <param name="Barcode">The new barcode of the product.</param>
        /// <param name="Price">The new price of the product.</param>
        /// <param name="Quantity">The new quantity of the product.</param>
        /// <param name="Status">The new status of the product.</param>
        /// <param name="Category">The new category of the product.</param>
        public static void UpdateProduct(string keyword, string ProductName, string Barcode, int Price, int Quantity, string Status, string Category)
        {
            string name = ProductName;
            string barcode = Barcode;
            int price = Price;
            int quantity = Quantity;
            string status = Status;
            string category = Category;

            var StoredProduct = ProductData.GetProduct(keyword);
            if (StoredProduct != null)
            {
                if (string.IsNullOrEmpty(ProductName))
                {
                    name = StoredProduct.Name;
                }
                if(string.IsNullOrEmpty(Barcode))
                {
                    barcode = StoredProduct.Barcode;
                }
                if(Price == 0)
                {
                    price = StoredProduct.Price;
                }
                if (Quantity == 0)
                {
                    quantity = StoredProduct.Quantity;
                }
                if (string.IsNullOrEmpty(Status))
                {
                    status = StoredProduct.Status;
                }
                if (string.IsNullOrEmpty(Category))
                {
                    category = StoredProduct.CategoryName;
                }
            }
            AddProduct(name, barcode, price, quantity, status, category);
        }

        /// <summary>
        /// Checks if a product exists in the inventory.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product.</param>
        /// <returns>True if the product exists, otherwise false.</returns>
        public static bool CheckIfProductExists(string keyword)
        {
            return ProductData.ProductExists(keyword);
        }

        /// <summary>
        /// Retrieves all categories from the inventory.
        /// </summary>
        /// <returns>An array of strings representing all categories.</returns>
        public static string[] GetCategories()
        {
            string JsonCategories = ProductData.GetAllCategories();
            string[] categories = JsonSerializer.Deserialize<string[]>(JsonCategories) ?? [];
            return categories;
        }

        /// <summary>
        /// Retrieves all categories from the inventory along with their IDs.
        /// </summary>
        /// <returns>A dictionary containing category IDs and their corresponding names.</returns>
        public static Dictionary<int, string> GetAllCategories()
        {
            string jsonCategories = ProductData.GetAllCategories();
            List<string> categoriesList = JsonSerializer.Deserialize<List<string>>(jsonCategories);

            Dictionary<int, string> categories = new Dictionary<int, string>();

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
        /// Retrieves status information of a product from the inventory.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product.</param>
        public static void StatusTracking(string keyword)
        {
            string JSONproducts = ProductData.GetProductStatus(keyword);
            Product[] products = JsonSerializer.Deserialize<Product[]>(JSONproducts) ?? [];

            Console.WriteLine("Products:");
            Console.WriteLine(JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true }));
        }

        /// <summary>
        /// Deletes a product from the inventory.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product to be deleted.</param>
        public static void DeleteProduct(string keyword)
        {
            ProductData.DeleteProduct(keyword);
        }

        /// <summary>
        /// Searches for products in the inventory based on a keyword.
        /// </summary>
        /// <param name="keyword">The keyword to search for.</param>
        /// <returns>An array of Product objects matching the search criteria.</returns>
        public static Product[] Search(string keyword)
        {
            string JSONproducts = ProductData.Search(keyword);
            Product[] products = JsonSerializer.Deserialize<Product[]>(JSONproducts) ?? [];

            return products;
           

        }

        /// <summary>
        /// Retrieves status information of a specific product from the inventory.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product.</param>
        public static void GetProductStatus(string keyword)
        {
            string productStatus = ProductData.ProductStatus(keyword);
            string status = productStatus.Split(',')[0];
            string quantity = productStatus.Split(',')[1];

            if (productStatus != "")
            Console.WriteLine($"product {keyword} is {status}, quantity: {quantity}");

        }

        /// <summary>
        /// Retrieves information of a specific product from the inventory.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product.</param>
        /// <returns>The product matching the provided keyword.</returns>
        public static Product GetProduct(string keyword)
        {
            Product product = null;
            if (CheckIfProductExists(keyword))
            {
                return ProductData.GetProduct(keyword);
            } 
            return product;
        }

        /// <summary>
        /// Adds a new category to the inventory.
        /// </summary>
        /// <param name="roleName">The name of the category to be added.</param>
        public static void AddCategory(string roleName)
        {
            if (string.IsNullOrEmpty(roleName) && !ProductData.CategoryExists(roleName))
            {
                ProductData.AddCategory(roleName);
            }
        }

        /// <summary>
        /// Deletes a category from the inventory.
        /// </summary>
        /// <param name="roleName">The name of the category to be deleted.</param>
        public static void DeleteCategory(string roleName)
        {
            if (string.IsNullOrEmpty(roleName) && !ProductData.CategoryExists(roleName))
            {
                ProductData.DeleteCategory(roleName);
            }
        }
    }


}