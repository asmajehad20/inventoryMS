
using Npgsql;
using System.Text.Json;


namespace inventoryMSLogic.src.DataAccessLayer
{
    /// <summary>
    /// Represents a product in the inventory.
    /// </summary>
    public class Product
    {
        public string Name { get; set; } = ""; 
        public int Price { get; set; }
        public string Barcode { get; set; } = "";
        public int Quantity { get; set; }
        public string Status { get; set; } = "";
        public string CategoryName { get; set; } = "";
    }

    /// <summary>
    /// Handles database operations related to inventory.
    /// </summary>
    class InventoryRepository
    {
        private readonly DatabaseConnection dbConnection;

        public InventoryRepository() 
        {
            dbConnection = new DatabaseConnection();
        }

        /// <summary>
        /// Retrieves information of all products from the database.
        /// </summary>
        /// <returns>A JSON string containing information of all products.</returns>
        public string GetAllProducts()
        {
            List<Product> ProductsList = [];

            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT products.product_name, products.bar_code, products.price, products.quantity, products.status, categories.name " +
                    "FROM products " +
                    "INNER JOIN categories ON products.category_id = categories.category_id;";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    Product product = new()
                    {
                        Name = reader.GetString(0),
                        Barcode = reader.GetString(1),
                        Price = reader.GetInt32(2),
                        Quantity = reader.GetInt32(3),
                        Status = reader.GetString(4),
                        CategoryName = reader.GetString(5)
                    };

                    ProductsList.Add(product);
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: retrieving products failed: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection(); // Ensure connection is closed
            }

            string jsonProducts = JsonSerializer.Serialize(ProductsList);
            return jsonProducts;
        }


        /// <summary>
        /// Retrieves the stock status and quantity of a product based on the provided keyword (barcode or product name).
        /// </summary>
        /// <param name="keyword">The barcode or product name to search for.</param>
        /// <returns>A string containing the status and quantity of the product.</returns>
        public string ProductStatus(string keyword)
        {
            string status = "";
            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT status, quantity FROM products " +
                    "WHERE bar_code = @keyword or product_name ILIKE @keyword;";

                using NpgsqlCommand cmd = new NpgsqlCommand(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@keyword", keyword);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string Status = reader.GetString(0);
                    string Quantity = reader.GetInt32(1).ToString();
                    status = Status + "," + Quantity;
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: failed to get the stock status for product: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection();
            }
            return status;
        }


        /// <summary>
        /// Retrieves products with the specified status from the database and returns them as JSON.
        /// </summary>
        /// <param name="status">The status of the products to retrieve.</param>
        /// <returns>A JSON string containing information about products with the specified status.</returns>
        public string GetProductStatus(string status)
        {
            List<Product> ProductsList = [];

            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT products.product_name, products.bar_code, products.price, products.quantity, products.status, categories.name " +
                    "FROM products " +
                    "INNER JOIN categories ON products.category_id = categories.category_id " +
                    "WHERE products.status = @status;";

                using NpgsqlCommand cmd = new NpgsqlCommand(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@status", status); 
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Product product = new()
                    {
                        Name = reader.GetString(0),
                        Barcode = reader.GetString(1),
                        Price = reader.GetInt32(2),
                        Quantity = reader.GetInt32(3),
                        Status = reader.GetString(4),
                        CategoryName = reader.GetString(5)
                    };
                    ProductsList.Add(product);
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: retrieving products failed: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection(); 
            }

            string jsonProducts = JsonSerializer.Serialize(ProductsList);
            
            return jsonProducts;
        }



        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">The product to be added.</param>
        public void AddProduct(Product product) 
        {

            try
            {
                dbConnection.OpenConnection();
                string query = "INSERT INTO products (product_id, product_name, bar_code, price, quantity, status, category_id) " +
                       "VALUES (uuid_generate_v4(), @Name, @barcode, @price, @quantity, @status, " +
                       "(SELECT categories.category_id FROM categories WHERE categories.name = @categoryName));";


                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@barcode", product.Barcode);
                cmd.Parameters.AddWithValue("@price", product.Price);    
                cmd.Parameters.AddWithValue("@quantity", product.Quantity);
                cmd.Parameters.AddWithValue("@status", product.Status);
                cmd.Parameters.AddWithValue("@categoryName", product.CategoryName);
                cmd.ExecuteNonQuery();

                Console.WriteLine($"product added");
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: adding product failed : {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }


        /// <summary>
        /// Deletes a product from the database based on the provided keyword (barcode or product name).
        /// </summary>
        /// <param name="keyword">The barcode or product name of the product to be deleted.</param>
        public void DeleteProduct(string keyword)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "DELETE FROM products WHERE product_name = @keyword or bar_code = @keyword";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@Name", keyword);

                cmd.ExecuteNonQuery();
                Console.WriteLine($"product deleted");
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: failed to delete product : {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Checks if a product exists in the database based on the provided keyword (barcode or product name).
        /// </summary>
        /// <param name="Keyword">The barcode or product name to search for.</param>
        /// <returns>True if the product exists, otherwise false.</returns>
        public bool ProductExists(string Keyword)
        {
            bool exists = false;
            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT COUNT(*) FROM products WHERE product_name ILIKE @Keyword OR bar_code = @Keyword";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@Keyword", Keyword);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                exists = count > 0;


            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: searching for product failed: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection(); 
            }

            return exists;
        }

        /// <summary>
        /// Retrieves information about a product based on the provided keyword (barcode or product name).
        /// </summary>
        /// <param name="Keyword">The barcode or product name to search for.</param>
        /// <returns>A Product object containing information about the product, or null if not found.</returns>
        public Product GetProduct(string Keyword)
        {
            Product? product = null;
            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT products.product_name, products.bar_code, products.price, products.quantity, products.status, categories.name " +
                    "FROM products " +
                    "INNER JOIN categories ON products.category_id = categories.category_id " +
                    "WHERE product_name ILIKE @Keyword OR bar_code = @Keyword";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@Keyword", Keyword);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    product = new Product
                    {
                        Name = reader.GetString(0),
                        Barcode = reader.GetString(1),
                        Price = reader.GetInt32(2),
                        Quantity = reader.GetInt32(3),
                        Status = reader.GetString(4),
                        CategoryName = reader.GetString(5)
                    };

                }
                else
                {
                    Console.WriteLine("Product not found.");
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: searching for product failed: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection();
            }

            return product;
        }


        /// <summary>
        /// Searches for products based on the provided search word and returns matching products as JSON.
        /// </summary>
        /// <param name="SearchWord">The search word to look for in product names, barcodes, status, and category names.</param>
        /// <returns>A JSON string containing information about matching products.</returns>
        public string Search(string SearchWord)
        {
            List<Product> ProductsFound = [];

            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT products.product_name, products.bar_code, products.price, products.quantity, products.status, categories.name " +
                       "FROM products " +
                       "INNER JOIN categories ON products.category_id = categories.category_id " +
                       "WHERE products.product_name ILIKE @SearchWord OR " +
                       "products.bar_code = @SearchWord OR " +
                       "products.status ILIKE @SearchWord OR " +
                       "categories.name ILIKE @SearchWord"; 

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@SearchWord", "%" + SearchWord + "%");// wildcard search
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Product product = new()
                    {
                        Name = reader.GetString(0),
                        Barcode = reader.GetString(1),
                        Price = reader.GetInt32(2),
                        Quantity = reader.GetInt32(3),
                        Status = reader.GetString(4),
                        CategoryName = reader.GetString(5)
                    };

                    ProductsFound.Add(product);
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: searching for products failed: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection(); // Ensure connection is closed
            }

            string jsonProducts = JsonSerializer.Serialize(ProductsFound);
            return jsonProducts;
        }

        /// <summary>
        /// Retrieves all categories from the database and returns them as JSON.
        /// </summary>
        /// <returns>A JSON string containing information about all categories.</returns>
        public string GetAllCategories()
        {
            List<string> categories = [];

            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT name FROM categories; ";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    categories.Add(reader.GetString(0));
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: retrieving categories failed: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection(); 
            }

            string jsonProducts = JsonSerializer.Serialize(categories);
            return jsonProducts;
        }

        /// <summary>
        /// Adds a new category to the database.
        /// </summary>
        /// <param name="name">The name of the category to be added.</param>
        public void AddCategory(string name)
        {

            try
            {
                dbConnection.OpenConnection();
                string query = "INSERT category_id, name INTO categories VALUES uuid_generate_v4(), @name ; ";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();


            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: adding category failed: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection();
            }

            return;
        }

        /// <summary>
        /// Checks if a category exists in the database.
        /// </summary>
        /// <param name="name">The name of the category to check.</param>
        /// <returns>True if the category exists, otherwise false.</returns>
        public bool CategoryExists(string name)
        {
            bool roleExists = false;

            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT COUNT(*) FROM categories WHERE name ILIKE @name";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, dbConnection.Connection))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    roleExists = count > 0;
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error retrieving category: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection();
            }

            return roleExists;
        }

        /// <summary>
        /// Deletes a category from the database.
        /// </summary>
        /// <param name="name">The name of the category to be deleted.</param>
        public void DeleteCategory(string name)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "DELETE FROM categories WHERE name ILIKE @name";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, dbConnection.Connection))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }





    }
}
