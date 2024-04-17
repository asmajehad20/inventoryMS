

using inventoryMSLogic.src.BusinessLogicLayer;
using inventoryMSLogic.src.DataAccessLayer;
using System.Data;
using System.Text.Json;

namespace inventoryMSCli.CLI
{
    /// <summary>
    /// Represents the admin page of the Inventory Management System CLI.
    /// </summary>
    class AdminPage
    {
        public static string UserName { get; private set; } = "";

        private readonly InventoryManager _inventoryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminPage"/> class with the specified username.
        /// </summary>
        /// <param name="userName">The username of the admin.</param>
        public AdminPage(string userName)
        {
            UserName = userName;
            InventoryRepository _repository = new();
            _inventoryManager = new InventoryManager(_repository);
        }

        /// <summary>
        /// Displays the admin menu options.
        /// </summary>
        private static void DisplayAdminMenu()
        {
            Console.WriteLine();
            Console.WriteLine("\x1b[1m  Inventory Management System \x1b[0m");
            Console.WriteLine("-- Admin Main Menu --");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Options:");

            Console.WriteLine("> u or users       <show user options>");
            Console.WriteLine("> p or products    <show products options>");
            Console.WriteLine("> roles            <show roles options>");
            Console.WriteLine("> categories        <show category options>");
            Console.WriteLine("-------------");

            Console.WriteLine("> c      <Clear Screen>");
            Console.WriteLine("> exit   <Exit>");
            Console.WriteLine("--------------------------------");
        }



        /// <summary>
        /// Runs the admin menu of the Inventory Management System CLI.
        /// </summary>
        public void Run()
        {
            Console.Clear();
            bool programExit = false;

            while (!programExit)
            {
                DisplayAdminMenu();

                string UserCommand = Console.ReadLine() ?? "";
                switch (UserCommand.Trim())
                {

                    case "u" or "users":
                        UserMenuRun();
                        break;

                    case "p" or "products":
                        ProductMenuRun();
                        break;

                    case "roles":
                        RolesMenuRun();
                        break;

                    case "categories":
                        CategoryMenuRun();
                        break;

                    case "c":
                        Console.Clear();
                        break;

                    case "exit":
                        programExit = true;
                        break;

                    default:
                        Console.WriteLine("error :: invalid input see options");
                        Console.WriteLine();
                        break;
                }
            }
        }

        /// <summary>
        /// Displays the menu for user options.
        /// </summary>
        private static void UsersOptionsMenu()
        {
            Console.WriteLine();
            Console.WriteLine("-- user options Menu --");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Options:");

            Console.WriteLine("> all         <show all users>");
            Console.WriteLine("> info        <show user informations>");
            Console.WriteLine("> delete      <delete user account>");
            Console.WriteLine("-------------");

            Console.WriteLine("> c      <Clear Screen>");
            Console.WriteLine("> exit   <Exit>");
            Console.WriteLine("--------------------------------");
        }

        /// <summary>
        /// Runs the user menu.
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

                    case "all":
                        UserManager.GetAllUsers();
                        break;

                    case "info":
                        UserManager.GetUserInfo(UserName);

                        break;

                    case "delete":
                        string Password = MainPage.GetPassword();
                        if (Password == "-exit" || Password == "exit") return;
                        UserManager.DeleteUser(UserName, Password);
                        Console.Clear();
                        _ = new MainPage();
                        MainPage.Run();
                        break;


                    case "c": Console.Clear();
                        break;

                    case "exit":
                        programExit = true;
                        break;

                    default:
                        Console.WriteLine("error :: invalid input see options");
                        Console.WriteLine();
                        break;
                }
            }
        }


        /// <summary>
        /// Displays the products options menu.
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
        /// Runs the product menu.
        /// </summary>
        private void ProductMenuRun()
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
                        Product[] products = _inventoryManager.GetAllProducts();
                        Console.WriteLine("Products:");
                        Console.WriteLine("----------------------------------------------------------------------------------------------------------");
                        Console.WriteLine("|Product Name          | Barcode       | Price   | Quantity  | Status       | Category                   |");
                        Console.WriteLine("----------------------------------------------------------------------------------------------------------");

                        foreach (var p in products)
                        {
                            Console.WriteLine($"| {p.Name,-20} | {p.Barcode,-13} | {p.Price,-7} | {p.Quantity,-9} | {p.Status,-12} | {p.CategoryName,-25} |");
                        }

                        Console.WriteLine("----------------------------------------------------------------------------------------------------------");

                        break;

                    case "add":
                        try
                        {
                            UserPage _userPage = new(UserName);
                            _userPage.AddNewProduct();
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case "status":
                        _inventoryManager.StatusTracking(UserPage.SetStatus().ToLower());
                        break;

                    case "delete":
                        Console.Write("Product Name or Barcode:");
                        string keyword = Console.ReadLine() ?? "";
                        _inventoryManager.DeleteProduct(keyword);
                        break;

                    case "update":
                        Console.Write("Product Name or Barcode:");
                        string productkeywork = Console.ReadLine() ?? "";
                        try
                        {
                            UserPage _userPage = new(UserName);
                            _userPage.UpdateProduct(productkeywork);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error : {ex.Message}");
                        }
                        break;

                    case "p_status":
                        Console.Write("Product Name or Barcode:");
                        string ProductStatus = Console.ReadLine() ?? "";
                        _inventoryManager.GetProductStatus(ProductStatus);
                        break;

                    case "search":
                        Console.Write("search word:");
                        string searchkeyword = Console.ReadLine() ?? "";

                        Product[] product = _inventoryManager.Search(searchkeyword);
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
        /// Displays the roles options menu.
        /// </summary>
        private static void RolesOptionsMenu()
        {

            Console.WriteLine("-- roles options Menu --");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Options:");

            Console.WriteLine("> all        <show all roles>");
            Console.WriteLine("> add         <add a new role>");
            Console.WriteLine("> delete      <delete role>");

            Console.WriteLine("-------------");

            Console.WriteLine("> c      <Clear Screen>");
            Console.WriteLine("> exit   <Exit>");
            Console.WriteLine("--------------------------------");
        }

        /// <summary>
        /// Runs the roles menu.
        /// </summary>
        private void RolesMenuRun()
        {
            Console.Clear();
            bool programExit = false;

            while (!programExit)
            {
                RolesOptionsMenu();

                string UserCommand = Console.ReadLine() ?? "";
                switch (UserCommand.Trim())
                {

                    case "all":
                        string[] roles = UserManager.GetAllRoles();
                        Console.WriteLine("System Roles:");
                        Console.WriteLine(JsonSerializer.Serialize(roles, new JsonSerializerOptions { WriteIndented = true }));

                        
                        break;

                    case "add":
                        Console.Write("Role Name:");
                        UserManager.AddRole(Console.ReadLine()??"");
                        break;

                    case "delete":
                        Console.Write("Role Name:");
                        UserManager.DeleteRole(Console.ReadLine() ?? "");
                        break;

                    case "c":
                        Console.Clear();
                        break;

                    case "exit":
                        programExit = true;
                        break;

                    default:
                        Console.WriteLine("error :: invalid input see options");
                        Console.WriteLine();
                        break;
                }
            }
        }

        /// <summary>
        /// Displays the category options menu.
        /// </summary>
        private static void CategoryOptionsMenu()
        {

            Console.WriteLine("-- category options Menu --");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Options:");

            Console.WriteLine("> all         <show all categories>");
            Console.WriteLine("> add         <add a new category>");
            Console.WriteLine("> update      <update a category>");
            Console.WriteLine("> delete      <delete category>");

            Console.WriteLine("-------------");

            Console.WriteLine("> c      <Clear Screen>");
            Console.WriteLine("> exit   <Exit>");
            Console.WriteLine("--------------------------------");
        }

        /// <summary>
        /// Runs the categories menu.
        /// </summary>
        private void CategoryMenuRun()
        {
            Console.Clear();
            bool programExit = false;

            while (!programExit)
            {
                CategoryOptionsMenu();

                string UserCommand = Console.ReadLine() ?? "";
                switch (UserCommand.Trim())
                {

                    case "all":
                        _inventoryManager.GetAllCategories();
                        
                        break;

                    case "add":
                        try
                        {
                            Console.Write("Category Name:");
                            string NewCategoryName = Console.ReadLine() ?? "";
                            _inventoryManager.AddCategory(NewCategoryName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error : {ex.Message}");
                        }
                        
                        break;

                    case "update":
                        try
                        {
                            Console.Write("Choose the category you want to update:");
                            UserPage _userPage = new(UserName);
                            string category = _userPage.SetCategory();
                            Console.Write("category new name:");
                            string newName = Console.ReadLine() ?? "";
                            _inventoryManager.UpdateCategory(category, newName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error : {ex.Message}");
                        }
                        break;

                    case "delete":
                        try
                        {
                            Console.Write("Category Name:");
                            _inventoryManager.DeleteCategory(Console.ReadLine() ?? "");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error : {ex.Message}");
                        }
                        break;

                    case "c":
                        Console.Clear();
                        break;

                    case "exit":
                        programExit = true;
                        break;

                    default:
                        Console.WriteLine("error :: invalid input see options");
                        Console.WriteLine();
                        break;
                }
            }
        }


    }
}
