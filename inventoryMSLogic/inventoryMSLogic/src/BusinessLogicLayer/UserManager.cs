using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using inventoryMSLogic.src.DataAccessLayer;

namespace inventoryMSLogic.src.BusinessLogicLayer
{
    /// <summary>
    /// Provides methods for managing user-related operations.
    /// </summary>
    public class UserManager
    {
        static readonly UserRepository UserData = new();

        /// <summary>
        /// Retrieves user information based on the provided username.
        /// </summary>
        /// <param name="UserName">The username of the user to retrieve information for.</param>
        
        public static void GetUserInfo(string UserName)
        {
            string Role = AuthenticationManager.GetUserRole(UserName);
            Console.WriteLine();
            Console.WriteLine("User info:");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("| Username       | Role           |");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"| {UserName,-14} | {Role,-14} |");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine();
            
        }


        /// <summary>
        /// Deletes a user account if the provided username and password match.
        /// </summary>
        /// <param name="UserName">The username of the user account to be deleted.</param>
        /// <param name="Password">The password of the user account.</param>
        public static void DeleteUser(string UserName, string Password)
        {
            if(AuthenticationManager.CheckUserCredentials(UserName, Password))
            {
                //delete user
                UserData.DeleteUser(UserName);
                Console.WriteLine($"{UserName} account is deleted");
                Console.WriteLine($"press any key to continue ...");
                Console.ReadKey();
                return;
            }
            else
            {
                Console.WriteLine($"Password is wrong");
                return;
            }

        }

        /// <summary>
        /// Retrieves information about all users and prints it to the console.
        /// </summary>
        /// <returns>An array of User objects representing all users.</returns>
        public static User[] GetAllUsers()
        {
            
            string usersString = UserData.GetAllUsers();
            User[] users = JsonSerializer.Deserialize<User[]>(usersString) ?? [];

            Console.WriteLine("Users:");
            Console.WriteLine("-----------------------------------------------------------------------------------------------");
            Console.WriteLine("| Username       | Password                                                         | Role    |");
            Console.WriteLine("-----------------------------------------------------------------------------------------------");

            foreach (User user in users)
            {
                Console.WriteLine($"| {user.Username,-14} | {user.Password,-42} | {user.Role,-7} |");
            }

            Console.WriteLine("-----------------------------------------------------------------------------------------------");

            return users;
        }

        /// <summary>
        /// Retrieves all roles from the database.
        /// </summary>
        /// <returns>An array of strings representing all roles.</returns>
        public static string[] GetAllRoles()
        {

            string RolesString = UserData.GetAllRoles();
            string[] roles = JsonSerializer.Deserialize<string[]>(RolesString) ?? [];

            return roles;

        }

        /// /// <summary>
        /// Adds a new role to the database if it doesn't already exist.
        /// </summary>
        /// <param name="roleName">The name of the role to be added.</param>
        public static void AddRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName) && !UserData.RoleExists(roleName))
            {
                UserData.AddRoles(roleName);
            }
        }

        /// <summary>
        /// Deletes a role from the database if it exists.
        /// </summary>
        /// <param name="roleName">The name of the role to be deleted.</param>
        public static void DeleteRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName) && !UserData.RoleExists(roleName))
            {
                UserData.DeleteRole(roleName);
            }
        }


    }
}
