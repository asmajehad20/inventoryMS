
using System.Security.Cryptography;
using System.Text;
using inventoryMSLogic.src.DataAccessLayer;

namespace inventoryMSLogic.src.BusinessLogicLayer
{
    public class AuthenticationManager
    {

        // <summary>
        /// Checks whether the provided username and password match the stored credentials in the user repository.
        /// </summary>
        /// <param name="UserName">The username of the user attempting to authenticate.</param>
        /// <param name="Password">The password provided by the user attempting to authenticate.</param>
        /// <returns>
        /// True if the provided username and password match the stored credentials; otherwise, false.
        /// </returns>
        public static bool CheckUserCredentials(string UserName, string Password)
        {
            UserRepository UserData = new();
            string PasswordHash = GetPasswordHash(Password);
            string StoredPasswordHash = UserData.GetStoredPasswordHash(UserName);

            if (!string.IsNullOrEmpty(StoredPasswordHash.Trim()))
                if (StoredPasswordHash == PasswordHash)
                    return true;
            

            return false;
        }


        /// <summary>
        /// Registers a new user with the provided username, password, and role.
        /// </summary>
        /// <param name="UserName">The username of the user to register.</param>
        /// <param name="Password">The password of the user to register.</param>
        /// <param name="Role">The role of the user to register. Defaults to "user" if not specified.</param>
        public static void RegisterUser(string UserName, string Password, string Role="user")
        {

            UserRepository UserData = new();
            string PasswordHash = GetPasswordHash(Password);
            UserData.CreateUser(UserName, PasswordHash, Role);
           
        }

        /// <summary>
        /// Retrieves the role of the user with the provided username.
        /// </summary>
        /// <param name="UserName">The username of the user whose role to retrieve.</param>
        /// <returns>The role of the user with the provided username.</returns>
        public static string GetUserRole(string UserName )
        {

            string Role;

            UserRepository UserRole = new();
            Role = UserRole.GetStoredUserRole(UserName);

            return Role;
        }

        /// <summary>
        /// Generates a hash from the provided password using SHA256 algorithm.
        /// </summary>
        /// <param name="Password">The password to hash.</param>
        /// <returns>The SHA256 hash of the provided password.</returns>
        private static string GetPasswordHash(string Password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);

            byte[] hashBytes = SHA256.HashData(passwordBytes);

            StringBuilder stringBuilder = new();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                stringBuilder.Append(hashBytes[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}
