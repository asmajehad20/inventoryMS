
using Npgsql;
using System.Security.Cryptography;
using System.Text.Json;

namespace inventoryMSLogic.src.DataAccessLayer
{
    /// <summary>
    /// Represents a product in the inventory.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// the name of the product in the inventory.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// the price of the product in the inventory.
        /// </summary>
        public int Price { get; set; } = 0;

        /// <summary>
        /// the barcode of the product in the inventory.
        /// </summary>
        public string Barcode { get; set; } = "";

        /// <summary>
        /// the quantity of the product in the inventory.
        /// </summary>
        public int Quantity { get; set; } = 0;

        /// <summary>
        /// the status of the product in the inventory.
        /// </summary>
        public string Status { get; set; } = "";

        /// <summary>
        /// the category of the product in the inventory.
        /// </summary>
        public string CategoryName { get; set; } = "";
    }

    /// <summary>
    /// Handles database operations related to inventory.
    /// </summary>
    public class InventoryRepository
    {
        private readonly DatabaseConnection dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryRepository"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor creates a new <see cref="DatabaseConnection"/> object and assigns it to the database connection field.
        /// </remarks>
        public InventoryRepository(){ dbConnection = new DatabaseConnection(); }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryRepository"/> class with the specified database connection.
        /// </summary>
        /// <param name="connection">The database connection to be used by the inventory repository.</param>
        public InventoryRepository(DatabaseConnection connection){ dbConnection = connection; }

        //////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Checks whether a product exists in the database based on the provided keyword, which can be either the product name or barcode.
        /// </summary>
        /// <param name="Keyword">The keyword to search for, which can be either the product name or barcode.</param>
        /// <returns>True if the product exists, otherwise false.</returns>
        /// <exception cref="NpgsqlException">Thrown when an error occurs during database interaction.</exception>
        public virtual bool ProductExists(string Keyword)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT EXISTS (SELECT 1 FROM products WHERE product_name ILIKE @Keyword OR bar_code = @Keyword)";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@Keyword", Keyword);

                return (bool)(cmd.ExecuteScalar() ?? false);
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: checking if product exsist failed: {ex.Message}");
                return false;
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves the product ID from the database based on the provided keyword, which can be either the product name or barcode.
        /// </summary>
        /// <param name="Keyword">The keyword to search for, which can be either the product name or barcode.</param>
        /// <returns>The product ID as a string if found.</returns>
        /// <exception cref="NpgsqlException">Thrown when an error occurs during database interaction.</exception>
        public string GetProductID(string Keyword)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT products.product_id FROM products WHERE product_name ILIKE @Keyword OR bar_code = @Keyword";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@Keyword", Keyword);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return reader.GetGuid(0).ToString();
                }
                else
                {
                    Console.WriteLine("Product not found.");
                    return "";
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: searching for product by ID failed: {ex.Message}");
                return "";
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves product details from the database based on the provided keyword, which can be either the product name or barcode.
        /// </summary>
        /// <param name="Keyword">The keyword to search for, which can be either the product name or barcode.</param>
        /// <returns>A <see cref="Product"/> object containing details of the found product, or null if not found.</returns>
        /// <exception cref="NpgsqlException">Thrown when an error occurs with Npgsql, the PostgreSQL .NET data provider.</exception>
        public virtual Product? GetProduct(string Keyword)
        {
            Product? product = null;
            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT products.product_name, products.bar_code, products.price, products.quantity, products.status, categories.name " +
                    "FROM products " +
                    "LEFT JOIN categories ON products.category_id = categories.category_id " +
                    "WHERE product_name ILIKE @Keyword OR bar_code = @Keyword;";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@Keyword", Keyword);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    product = new Product
                    {
                        Name = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        Barcode = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Price = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                        Quantity = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                        Status = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        CategoryName = reader.IsDBNull(5) ? "" : reader.GetString(5)
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
        /// Retrieves details of all products from the database.
        /// </summary>
        /// <returns>An array of <see cref="Product"/> objects containing details of all products.</returns>
        /// <exception cref="NpgsqlException">Thrown when an error occurs with Npgsql, the PostgreSQL .NET data provider.</exception>
        public virtual Product[] GetAllProducts()
        {
            List<Product> ProductsList = [];

            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT products.product_name, products.bar_code, products.price, products.quantity, products.status, categories.name " +
                    "FROM products " +
                    "LEFT JOIN categories ON products.category_id = categories.category_id;";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    Product product = new()
                    {
                        Name = reader.IsDBNull(0) ? "" : reader.GetString(0),
                        Barcode = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Price = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                        Quantity = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                        Status = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        CategoryName = reader.IsDBNull(5) ? "" : reader.GetString(5)
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

            return ProductsList.ToArray();
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">The <see cref="Product"/> object containing details of the product to be added.</param>
        /// <returns>True if the product is successfully added, otherwise false.</returns>
        /// <exception cref="NpgsqlException">Thrown when an error occurs with Npgsql, the PostgreSQL .NET data provider.</exception>
        public virtual bool AddProduct(Product product)
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
                return true;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: adding product failed : {ex.Message}");
                return false;
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Updates the details of a product in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the product to be updated.</param>
        /// <param name="name">The new name of the product.</param>
        /// <param name="barcode">The new barcode of the product.</param>
        /// <param name="price">The new price of the product.</param>
        /// <param name="quantity">The new quantity of the product.</param>
        /// <param name="status">The new status of the product.</param>
        /// <param name="category">The new category name of the product.</param>
        /// <returns>True if the product is successfully updated, otherwise false.</returns>
        /// <exception cref="NpgsqlException">Thrown when an error occurs during database interaction.</exception>
        public virtual bool UpdateProduct(string id, string name, string barcode, int price, int quantity, string status, string category)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "UPDATE products SET product_name = @Name, " +
                       "bar_code = @barcode, " +
                       "price = @price, " +
                       "quantity = @quantity, " +
                       "status = @status, " +
                       "category_id = (SELECT category_id FROM categories WHERE name = @categoryName) " +
                       "WHERE product_id = @id;";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, Guid.Parse(id));
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@barcode", barcode);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@categoryName", category);
                cmd.ExecuteNonQuery();

                Console.WriteLine($"product updated");
                return true;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: updating product failed : {ex.Message}");
                return false;
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Deletes a product from the database based on the provided keyword, which can be either the product name or barcode.
        /// </summary>
        /// <param name="keyword">The keyword to identify the product to be deleted, which can be either the product name or barcode.</param>
        /// <returns>True if the product is successfully deleted, otherwise false.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during database interaction.</exception>
        /// <exception cref="NpgsqlException">Thrown when an error occurs with Npgsql, the PostgreSQL .NET data provider.</exception>
        public virtual bool DeleteProduct(string keyword)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "DELETE FROM products WHERE product_name ilike @keyword or bar_code = @keyword";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@keyword", keyword);

                cmd.ExecuteNonQuery();
                Console.WriteLine($"product deleted");
                return true;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: failed to delete product : {ex.Message}");
                return false;
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves the stock status and quantity of a product based on the provided keyword (barcode or product name).
        /// </summary>
        /// <param name="keyword">The barcode or product name to search for.</param>
        /// <returns>A string containing the status and quantity of the product.</returns>
        public virtual string ProductStatus(string keyword)
        {
            string status = "";
            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT status, quantity FROM products " +
                    "WHERE bar_code = @keyword or product_name ILIKE @keyword;";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
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
        public virtual Product[] GetProductsByStatus(string status)
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
            
            return ProductsList.ToArray();
        }

        /// <summary>
        /// Searches for products based on the provided search word and returns matching products as JSON.
        /// </summary>
        /// <param name="SearchWord">The search word to look for in product names, barcodes, status, and category names.</param>
        /// <returns>A JSON string containing information about matching products.</returns>
        public virtual Product[] Search(string SearchWord)
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
                       "categories.name ILIKE @SearchWord;"; 

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
                dbConnection.CloseConnection();
            }

            return ProductsFound.ToArray();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //Categories// 

        /// <summary>
        /// Checks if a category exists in the database.
        /// </summary>
        /// <param name="name">The name of the category to check.</param>
        /// <returns>True if the category exists, otherwise false.</returns>
        public virtual bool CategoryExists(string name)
        {
            bool roleExists = false;

            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT COUNT(*) FROM categories WHERE name ILIKE @name;";

                using (NpgsqlCommand cmd = new(query, dbConnection.Connection))
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
        /// Retrieves all categories from the database and returns them as JSON.
        /// </summary>
        /// <returns>A JSON string containing information about all categories.</returns>
        public virtual string[] GetAllCategories()
        {
            List<string> categories = []; // Correct list initialization

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
                // Handle the exception as per your application's requirements
                // You might want to throw the exception here to propagate it
            }
            finally
            {
                dbConnection.CloseConnection(); // Ensure connection is closed
            }

            return categories.ToArray();
        }


        /// <summary>
        /// Adds a new category to the database.
        /// </summary>
        /// <param name="name">The name of the category to be added.</param>
        public virtual bool AddCategory(string name)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "INSERT INTO categories (category_id, name) VALUES (uuid_generate_v4(), @Name);";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.ExecuteNonQuery();
                Console.WriteLine("category added");
                return true;

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: adding category failed: {ex.Message}");
                return false;
            }
            finally
            {
                dbConnection.CloseConnection();
            }      
        }

        /// <summary>
        /// Deletes a category from the database.
        /// </summary>
        /// <param name="name">The name of the category to be deleted.</param>
        public virtual bool DeleteCategory(string name)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "DELETE FROM categories WHERE name ILIKE @name;";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
                Console.WriteLine("category deleted");
                return true;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: deleting category failed: {ex.Message}");
                return false;
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves the category ID from the database based on the provided category name.
        /// </summary>
        /// <param name="Keyword">The category name to search for.</param>
        /// <returns>The category ID as a string if found.</returns>
        /// <exception cref="Exception">Thrown when the category ID is not found in the database.</exception>
        /// <exception cref="NpgsqlException">Thrown when an error occurs during database interaction.</exception>

        public virtual string GetCategoryID(string Keyword)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT categories.category_id FROM categories WHERE name ILIKE @Keyword; ";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@Keyword", Keyword);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return reader.GetGuid(0).ToString();
                }
                else
                {
                    Console.WriteLine("category not found.");
                    throw new Exception("category id not found.");
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: searching for category failed: {ex.Message}");
                throw;
            }
            catch(Exception e)
            {
                Console.WriteLine($"category not found : {e.Message}");
                throw;
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Updates the name of a category in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the category to be updated.</param>
        /// <param name="name">The new name of the category.</param>
        /// <returns>True if the category is successfully updated, otherwise false.</returns>
        /// <exception cref="NpgsqlException">Thrown when an error occurs during database interaction.</exception>
        public virtual bool UpdateCategory(string id, string name)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "UPDATE categories SET name = @Name WHERE category_id = @id;";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, Guid.Parse(id));
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.ExecuteNonQuery();

                Console.WriteLine($"product updated");
               // cmd.Dispose();
                return true;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: updating category failed : {ex.Message}");
                return false;
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }

    }
}
