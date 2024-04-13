

using System.Text.Json;
using inventoryMSLogic.src.BusinessLogicLayer;
using inventoryMSLogic.src.DataAccessLayer;

namespace inventoryMSCli.CLI
{
    /// <summary>
    /// Contains methods related to user options in the Inventory Management System.
    /// </summary>
    class UserPage
    {
        public static string UserName { get; private set; } = "";

        public UserPage(string userName)
        {
            UserName = userName;
        }

        /// <summary>
        /// Displays the user menu on the console.
        /// </summary>
        private static void DisplayUserMenu()
        {
            Console.WriteLine();
            Console.WriteLine("\x1b[1m  Inventory Management System \x1b[0m");
            Console.WriteLine("-- User Main Menu --");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Options:");

            Console.WriteLine("> u or users       <show user options>");
            Console.WriteLine("> p or products    <show products options>");
            Console.WriteLine("-------------");

            Console.WriteLine("> c      <Clear Screen>");
            Console.WriteLine("> exit   <Exit>");
            Console.WriteLine("--------------------------------");
        }

        /// <summary>
        /// Runs the user page logic.
        /// </summary>
        public static void Run()
        {
            
            Console.Clear();
            bool programExit = false;

            while (!programExit)
            {
                DisplayUserMenu();

                string UserCommand = Console.ReadLine() ?? "";
                switch (UserCommand.Trim())
                {

                    case "u" or "users":
                        UserMenuRun();
                        break;

                    case "p" or "products":
                        ProductMenuRun();
                        break;

                    case "c":
                        Console.Clear();
                        break;

                    case "exit":
                        programExit = true;
                        Console.Clear();
                        break;

                    default:
                        Console.WriteLine("error :: invalid input see options");
                        Console.WriteLine();
                        break;
                }
            }

        }

        /// <summary>
        /// Displays the options menu for user-related actions.
        /// </summary>
        private static void UsersOptionsMenu()
        {
            Console.WriteLine();
            Console.WriteLine("-- user options Menu --");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Options:");

            Console.WriteLine("> info        <show user informations>");
            Console.WriteLine("> delete      <delete user account>");
            Console.WriteLine("-------------");

            Console.WriteLine("> c      <Clear Screen>");
            Console.WriteLine("> exit   <Exit>");
            Console.WriteLine("--------------------------------");
           
        }

        /// <summary>
        /// Runs the user menu for user-related actions.
        /// </summary>
        private static void UserMenuRun()
        {
            Console.Clear();
            bool programExit = false;

            while (!programExit)
            {
                UsersOptionsMenu();

                string UserCommand = Console.ReadLine() ?? "";
                switch (UserCommand.Trim())
                {

                    case "info":
                        UserManager.GetUserInfo(UserName);
                        
                        break;

                    case "delete":
                        string Password = MainPage.GetPassword();
                        if (Password == "-exit" || Password == "exit") return;
                        UserManager.DeleteUser(UserName, Password);
                        _ = new MainPage();
                        MainPage.Run();
                        break;


                    case "c":
                        Console.Clear();
                        break;

                    case "exit":
                        programExit = true;
                        Console.Clear();
                        break;

                    default:
                        Console.WriteLine("error :: invalid input see options");
                        Console.WriteLine();
                        break;
                }
            }
        }


        /// <summary>
        /// Displays the options menu for product-related actions.
        /// </summary>
        private static void ProductsOptionsMenu()
        {
            Console.WriteLine();
            Console.WriteLine("-- products options Menu --");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Options:");

            Console.WriteLine("> all               <show all products>");
            Console.WriteLine("> add               <add a new product>");
            Console.WriteLine("> status            <show all products in status>");

            Console.WriteLine("> delete            <delete product>");
            Console.WriteLine("> update            <update product>");

            Console.WriteLine("> p_status    <show the stock status for a product>");
            Console.WriteLine("> search            <search for products by keyword>");
            Console.WriteLine("-------------");

            Console.WriteLine("> c      <Clear Screen>");
            Console.WriteLine("> exit   <Exit>");
            Console.WriteLine("--------------------------------");
            
        }

        /// <summary>
        /// Runs the product menu for product-related actions.
        /// </summary>
        private static void ProductMenuRun()
        {
            Console.Clear();
            bool programExit = false;

            while (!programExit)
            {
                ProductsOptionsMenu();

                string UserCommand = Console.ReadLine() ?? "";
                switch (UserCommand.Trim())
                {

                    case "all":
                        Product[] products = InventoryManager.GetAllProducts();
                        Console.WriteLine("Products:");
                        Console.WriteLine("----------------------------------------------------------------------------------------------------------");
                        Console.WriteLine("|Product Name          | Barcode       | Price   | Quantity  | Status       | Category                   |");
                        Console.WriteLine("----------------------------------------------------------------------------------------------------------");

                        foreach (var p in products)
                        {
                            Console.WriteLine($"| {p.Name, -20} | {p.Barcode,-13} | {p.Price,-7} | {p.Quantity,-9} | {p.Status, -12} | {p.CategoryName, -25} |");
                        }

                        Console.WriteLine("----------------------------------------------------------------------------------------------------------");


                        break;

                    case "add":
                        AddNewProduct();
                        break;

                    case "status":
                        InventoryManager.StatusTracking(SetStatus().ToLower());
                        break;

                    case "delete":
                        Console.Write("Product Name or Barcode:");
                        string keyword = Console.ReadLine() ?? "";
                        InventoryManager.DeleteProduct(keyword);
                        break;

                    case "update":
                        Console.Write("Product Name or Barcode:");
                        string productkeywork = Console.ReadLine() ?? "";
                        UpdateProduct(productkeywork);
                        break;

                    case "p_status":
                        Console.Write("Product Name or Barcode:");
                        string ProductStatus = Console.ReadLine() ?? "";
                        InventoryManager.GetProductStatus(ProductStatus);
                        break;

                    case "search":
                        Console.Write("search word:");
                        string searchkeyword = Console.ReadLine() ?? "";

                        Product[] product = InventoryManager.Search(searchkeyword);
                        Console.WriteLine("Products:");
                        Console.WriteLine(JsonSerializer.Serialize(product, new JsonSerializerOptions { WriteIndented = true }));
                        
                        break;


                    case "c":
                        Console.Clear();
                        break;

                    case "exit":
                        programExit = true;
                        Console.Clear();
                        break;

                    default:
                        Console.WriteLine("error :: invalid input see options");
                        Console.WriteLine();
                        break;
                }
            }
        }

        /// <summary>
        /// Updates the details of an existing product.
        /// </summary>
        /// <param name="keyword">The name or barcode of the product to update.</param>
        public static void UpdateProduct(string keywword)
        {
            if (!InventoryManager.CheckIfProductExists(keywword))
            {
                Console.WriteLine($"cant find Product {keywword}");
                return;
            }

            //get product name
            Console.Write("Product Name: ");
            string ProductName = Console.ReadLine() ?? "";
            //get product barcode
            Console.Write("Product Barcode: ");
            string Barcode = Console.ReadLine() ?? "";

            //get price
            Console.Write("Price: ");
            string pricestring = Console.ReadLine() ?? "";
            int price = 0;
            if (pricestring != "")
            {
                _ = int.TryParse(pricestring, out price);
            }

            //get quantity
            Console.Write("Quantity: ");
            string quantitystring = Console.ReadLine() ?? "";
            int quantity = 0;
            if (quantitystring != "")
            {
                _ = int.TryParse(quantitystring, out quantity);
            }
            
            //get status
            string status = SetStatus();
            //get category display categories 
            string category = SetCategory();

            InventoryManager.UpdateProduct(keywword, ProductName, Barcode, price, quantity, status, category);
        }

        /// <summary>
        /// Adds a new product to the inventory.
        /// </summary>
        public static void AddNewProduct()
        {
            //get product name
            string ProductName = GetProductName();
            if (InventoryManager.CheckIfProductExists(ProductName))
            {
                Console.WriteLine($"Product with Name {ProductName} already in inventory");
                return;
            }

            //get product barcode
            string Barcode =  GetBarcode();
            if (InventoryManager.CheckIfProductExists(Barcode))
            {
                Console.WriteLine($"Product with Barcode {Barcode} already in inventory");
                return;
            }

            //get price 
            int price = GetPrice();

            //get quantity
            int quantity = GetQuantity();

            //get status
            string status = SetStatus();

            //get category display categories 
            string category = SetCategory();

            InventoryManager.AddProduct(ProductName, Barcode, price, quantity, status, category);
        }

        /// <summary>
        /// Sets the category based on user input.
        /// </summary>
        /// <returns>The selected category.</returns>
        public static string SetCategory()
        {
            string category = "";

            while (true)
            {
                Console.WriteLine("Category Options:");

                Dictionary<int, string> categories = InventoryManager.GetAllCategories();

                Console.Write("Enter category text or numeric code: ");
                string input = Console.ReadLine()?.ToLower() ?? "";
                if (input == "") return category;

                if (int.TryParse(input, out int code))
                {
                    if (categories.ContainsKey(code))
                    {
                        category = categories[code];
                    }
                    else
                    {
                        Console.WriteLine("Error: Invalid input");
                    }
                }
                else
                {
                    var categoryEntry = categories.FirstOrDefault(c => c.Value.Equals(input, StringComparison.CurrentCultureIgnoreCase));
                    if (!categories.Equals(default(KeyValuePair<int, string>)))
                    {
                        category = categoryEntry.Value;
                    }
                    else
                    {
                        Console.WriteLine("Error: Invalid value for category.");
                        continue; 
                    }
                }

                if (!string.IsNullOrEmpty(category))
                    break;
            }

            return category;
        }

        /// <summary>
        /// Sets the status based on user input.
        /// </summary>
        /// <returns>The selected status.</returns>
        public static string SetStatus()
        {
            string status = "";

            while (true)
            {
                Console.WriteLine("Status Options:");
                Console.Write("Enter status text or numeric code: ");
                Console.WriteLine("1. In Stock");
                Console.WriteLine("2. Out of Stock");
                Console.WriteLine("3. Low on Stock");

                Console.Write("Status: ");
                string input = Console.ReadLine() ?? "";
                input = input.ToLower();
                if (input == "") return status;

                if (int.TryParse(input, out int code) && code >= 1 && code <= 3)
                {
                    switch (code)
                    {
                        case 1:
                            status = "In Stock";
                            break;
                        case 2:
                            status = "Out of Stock";
                            break;
                        case 3:
                            status = "Low on Stock";
                            break;
                        default:
                            Console.WriteLine("error :: invalid input");
                            break;
                    }
                }
                else if (input == "in stock" || input == "out of stock" || input == "low on stock" || input == "")
                {
                    status = input;
                }
                else
                {
                    Console.WriteLine("error :: invalid input");
                    continue; 
                }

                if (!string.IsNullOrEmpty(status))
                    break;
            }
            return status;

        }

        /// <summary>
        /// Gets the price from user input.
        /// </summary>
        /// <returns>The price.</returns>
        public static int GetPrice()
        {
            int Price;

            while (true)
            {
                
                Console.Write("Price: ");
                string priceString = Console.ReadLine() ?? "";

                if (!int.TryParse(priceString, out Price))
                {
                    Console.WriteLine("error :: Invalid value for price.");
                }
                else
                {
                    break;
                }
            }

            return Price;
        }

        /// <summary>
        /// Gets the quantity from user input.
        /// </summary>
        /// <returns>The quantity.</returns>
        public static int GetQuantity()
        {
            int Quantity;

            while (true)
            {
                
                Console.Write("Quantity: ");
                string QuantityString = Console.ReadLine() ?? "";

                if (!int.TryParse(QuantityString, out Quantity))
                {
                    Console.WriteLine("error :: Invalid value for Quantity.");
                }
                else
                {
                    break;
                }
            }

            return Quantity;
        }

        /// <summary>
        /// Gets the barcode from user input.
        /// </summary>
        /// <returns>The barcode.</returns>
        public static string GetBarcode()
        {
            string ProductBarcode;

            while (true)
            {
                Console.Write("Barcode: ");
                ProductBarcode = Console.ReadLine() ?? "";

                if (ProductBarcode.Length != 12)
                {
                    Console.WriteLine("error :: Barcode must be 12 characters long.");
                }
                else if (!ProductBarcode.All(char.IsDigit))
                    {
                    Console.WriteLine("error :: Barcode must contain only numeric characters.");
                }
                else
                {
                    break; 
                }
            }
            return ProductBarcode;
        }

        /// <summary>
        /// Gets the product name from user input.
        /// </summary>
        /// <returns>The product name.</returns>
        public static string GetProductName()
        {
            string ProductName;

            while (true)
            {
                Console.Write("Product Name: ");
                ProductName = Console.ReadLine() ??"";

                if (string.IsNullOrWhiteSpace(ProductName))
                {
                    Console.WriteLine("error :: Product Name cannot be empty");
                }
                else
                {
                    break;
                }
            }

            return ProductName;
        }

    }
}
