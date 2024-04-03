using inventoryMSLogic.src.BusinessLogicLayer;

namespace inventoryMSCli.CLI
{
    /// <summary>
    /// Represents the main page of the Inventory Management System CLI.
    /// </summary>
    class MainPage
    {

        /// <summary>
        /// Displays the main menu options.
        /// </summary>
        private static void DisplayMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("\x1b[1m  Inventory Management System \x1b[0m");
            Console.WriteLine("-- Main Menu --");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Options:");
            Console.WriteLine("-l      <Login>");
            Console.WriteLine("-r      <Register>");
            Console.WriteLine("-c      <Clear Screen>");
            Console.WriteLine("-exit   <Exit>");
            Console.WriteLine("--------------------------------");
        }

        /// <summary>
        /// Runs the main menu of the Inventory Management System CLI.
        /// </summary>
        public static void Run()
        {
            bool programExit = false;

            while (!programExit)
            {
                DisplayMainMenu();

                var UserCommand = Console.ReadLine();
                switch (UserCommand)
                {
                    case "-l":
                        Login();
                        break;

                    case "-r":
                        Register();
                        break;

                    case "-c":
                        Console.Clear();
                        break;

                    case "-exit" or "exit":
                        programExit = true;
                        break;

                    default:
                        Console.WriteLine("error :: invalid input see options");
                        break;
                }
            }

        }

        //-------- functions -----------
        /// <summary>
        /// Prompts the user for login credentials and attempts to log in.
        /// </summary>
        private static void Login()
        {
            Console.WriteLine("\u001b[1mlog in:  \u001b[0m");
            Console.WriteLine("--------------------------------");

            //get username
            string UserName = GetUserName();
            if (UserName == "-exit" || UserName == "exit") return;

            //get password
            string Password = GetPassword();
            while (Password.Trim() == "")
            {
                Console.WriteLine();
                Console.WriteLine("error :: Password cant be empty");
                Password = GetPassword();
            }
            if (Password == "-exit" || Password == "exit") return;

            bool IsValidUser = AuthenticationManager.CheckUserCredentials(UserName, Password);
            if (IsValidUser)
            {
                string UserRole = AuthenticationManager.GetUserRole(UserName);
                if (UserRole == "user")
                {
                    _ = new UserPage(UserName);
                    UserPage.Run();
                }
                else
                {
                    _ = new AdminPage(UserName);
                    AdminPage.Run();
                }
            }
            else
            {
                Console.WriteLine("error :: UserName or Password is wrong");
                Console.ReadKey();
                return;
            }

        }

        /// <summary>
        /// Registers a new user by collecting required information.
        /// </summary>
        private static void Register()
        {
            Console.WriteLine("\u001b[1minformation needed for registration:  \u001b[0m");
            Console.WriteLine("--------------------------------");

            //get username
            string UserName = GetUserName();
            if (UserName == "-exit" || UserName == "exit") return;

            // get password
            string Password = GetPassword();
            while (Password.Trim() == "")
            {
                Console.WriteLine();
                Console.WriteLine("error :: Password cant be empty");
                Password = GetPassword();
            }
            if (Password == "-exit" || Password == "exit") return;

            //get role 
            string Role = "user";
            Console.WriteLine();

            AuthenticationManager.RegisterUser(UserName, Password, Role);
        }

        /// <summary>
        /// Prompts the user to enter their username.
        /// </summary>
        /// <returns>The username entered by the user.</returns>
        private static string GetUserName()
        {
            string UserName;
            bool IvalidInput;

            do
            {
                Console.Write("UserName: ");
                UserName = Console.ReadLine() ?? "";
                IvalidInput = string.IsNullOrEmpty(UserName.Trim());

                if (IvalidInput)
                {
                    Console.WriteLine("error :: UserName can't be empty");
                }

            } while (IvalidInput);

            return UserName;
        }

        /// <summary>
        /// Prompts the user to enter their password without showing it on the console.
        /// </summary>
        /// <returns>The password entered by the user.</returns>
        public static string GetPassword()
        {
            string password = "";
            ConsoleKeyInfo key;
            Console.Write("Password: ");

            do
            {
                key = Console.ReadKey(intercept: true);
                if (key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                }
            } while (key.Key != ConsoleKey.Enter);

            return password;
        }


    }
}
