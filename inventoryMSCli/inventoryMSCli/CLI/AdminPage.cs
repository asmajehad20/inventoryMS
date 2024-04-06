

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

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminPage"/> class with the specified username.
        /// </summary>
        /// <param name="userName">The username of the admin.</param>
        public AdminPage(string userName)
        {
            UserName = userName;
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
            Console.WriteLine("> category         <show category options>");
            Console.WriteLine("-------------");

            Console.WriteLine("> c      <Clear Screen>");
            Console.WriteLine("> exit   <Exit>");
            Console.WriteLine("--------------------------------");
        }



        /// <summary>
        /// Runs the admin menu of the Inventory Management System CLI.
        /// </summary>
        public static void Run()
        {
            Console.Clear();
            bool programExit = false;

            while (!programExit)
            {
                DisplayAdminMenu();

                var UserCommand = Console.ReadLine();
                switch (UserCommand)
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

                    case "category":
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

                var UserCommand = Console.ReadLine();
                switch (UserCommand)
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
                        return;  


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
        private static void ProductMenuRun()
        {
            Console.Clear();
            bool programExit = false;

            while (!programExit)
            {
                ProductsOptionsMenu();

                var UserCommand = Console.ReadLine();
                switch (UserCommand)
                {

                    case "all":
                        Product[] products = InventoryManager.GetAllProducts();
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
                        UserPage.AddNewProduct();
                        break;

                    case "status":
                        InventoryManager.StatusTracking(UserPage.SetStatus().ToLower());
                        break;

                    case "delete":
                        Console.Write("Product Name or Barcode:");
                        string keyword = Console.ReadLine() ?? "";
                        InventoryManager.DeleteProduct(keyword);
                        break;

                    case "update":
                        Console.Write("Product Name or Barcode:");
                        string productkeywork = Console.ReadLine() ?? "";
                        UserPage.UpdateProduct(productkeywork);
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
        private static void RolesMenuRun()
        {
            Console.Clear();
            bool programExit = false;

            while (!programExit)
            {
                RolesOptionsMenu();

                var UserCommand = Console.ReadLine();
                switch (UserCommand)
                {

                    case "all":
                        string[] roles = UserManager.GetAllRoles();
                        Console.WriteLine("System Roles:");
                        Console.WriteLine(JsonSerializer.Serialize(roles, new JsonSerializerOptions { WriteIndented = true }));

                        
                        break;

                    case "add ":
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

            Console.WriteLine("> all        <show all roles>");
            Console.WriteLine("> add         <add a new role>");
            Console.WriteLine("> delete      <delete role>");

            Console.WriteLine("-------------");

            Console.WriteLine("> c      <Clear Screen>");
            Console.WriteLine("> exit   <Exit>");
            Console.WriteLine("--------------------------------");
        }

        /// <summary>
        /// Runs the categories menu.
        /// </summary>
        private static void CategoryMenuRun()
        {
            Console.Clear();
            bool programExit = false;

            while (!programExit)
            {
                CategoryOptionsMenu();

                var UserCommand = Console.ReadLine();
                switch (UserCommand)
                {

                    case "all":
                        string[] categories = InventoryManager.GetCategories();
                        Console.WriteLine("System categories:");
                        Console.WriteLine(JsonSerializer.Serialize(categories, new JsonSerializerOptions { WriteIndented = true }));

                        break;

                    case "add ":
                        Console.Write("Category Name:");
                        InventoryManager.AddCategory(Console.ReadLine() ?? "");
                        
                        break;

                    case "delete":
                        Console.Write("Category Name:");
                        InventoryManager.DeleteCategory(Console.ReadLine() ?? "");
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
