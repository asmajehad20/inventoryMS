
using Npgsql;
using System.Text.Json;

namespace inventoryMSLogic.src.DataAccessLayer
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";
        
    }

    /// <summary>
    /// Handles database operations related to users.
    /// </summary>
    class UserRepository
    {
        private readonly DatabaseConnection dbConnection;

        public UserRepository()
        {
            dbConnection = new DatabaseConnection(); 
        }

        /// <summary>
        /// Retrieves the stored role of the user from the database.
        /// </summary>
        /// <param name="UserName">The username of the user.</param>
        /// <returns>The role of the user.</returns>
        public string GetStoredUserRole(string UserName) 
        {
            string Role = "user";
            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT roles.name " +
                               "FROM users " +
                               "JOIN roles ON users.role_id = roles.role_id " +
                               "WHERE users.username = @UserName;";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@UserName", UserName);
                var result = cmd.ExecuteScalar() ?? "";
                if (result != null)
                {
                    Role = result.ToString() ?? "";
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: retrieving user role failed: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection(); 
            }

            return Role;
        }

        /// <summary>
        /// Retrieves the stored password hash of the user from the database.
        /// </summary>
        /// <param name="UserName">The username of the user.</param>
        /// <returns>The password hash of the user.</returns>
        public string GetStoredPasswordHash(string UserName)
        {
            string PasswordHash = "";
            
            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT password_hash FROM users WHERE username = @UserName;";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@UserName", UserName);
                var result = cmd.ExecuteScalar() ?? "";
                if (result != null)
                {
                    PasswordHash = result.ToString() ?? "";
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: retrieving password hash failed: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection(); 
            }

            return PasswordHash;
        }

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        /// <param name="UserName">The username of the user to be created.</param>
        /// <param name="Password">The password of the user to be created.</param>
        /// <param name="Role">The role of the user to be created.</param>
        public bool CreateUser(string UserName, string Password, string Role)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "INSERT INTO users (user_id, username, password_hash, role_id) " +
               "VALUES (uuid_generate_v4(), @UserName, @Password, (SELECT role_id FROM roles WHERE name = @Role))";


                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@UserName", UserName);
                cmd.Parameters.AddWithValue("@Password", Password);
                cmd.Parameters.AddWithValue("@Role", Role);
                cmd.ExecuteNonQuery();

                Console.WriteLine($"User Registered");
                return true;

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: couldnt register : {ex.Message}");
                return false;
            }
            finally
            {
                dbConnection.CloseConnection(); 
            }
        }

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        /// <param name="UserName">The username of the user to be deleted.</param>
        public void DeleteUser(string UserName)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "DELETE FROM users WHERE username = @UserName";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@UserName", UserName);
                
                cmd.ExecuteNonQuery();

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: failed to delete user account  : {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection(); 
            }
        }


        /// <summary>
        /// Retrieves all users from the database and returns them as JSON.
        /// </summary>
        /// <returns>A JSON string containing information about all users.</returns>
        public User[] GetAllUsers()
        {
            List<User> UsersList = [];

            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT users.username, users.password_hash, roles.name" +
                    " FROM users " +
                    "INNER JOIN roles ON users.role_id = roles.role_id";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    User user = new()
                    {
                        Username = reader.GetString(0),
                        Password = reader.GetString(1),
                        Role = reader.GetString(2),
                        
                    };

                    UsersList.Add(user);
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: failed to Get all users  : {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection();
            }

            return UsersList.ToArray();
        }

        /// <summary>
        /// Retrieves all roles from the database and returns them as JSON.
        /// </summary>
        /// <returns>A JSON string containing information about all roles.</returns>
        public string[] GetAllRoles()
        {
            List<string> RolesList = [];

            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT name FROM roles; ";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                using NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RolesList.Add(reader.GetString(0));
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: retrieving roles failed: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection(); 
            }

            return RolesList.ToArray();
        }

        /// <summary>
        /// Adds a new role to the database.
        /// </summary>
        /// <param name="rolename">The name of the role to be added.</param>
        public bool AddRoles(string rolename)
        {
            
            try
            {
                dbConnection.OpenConnection();
                string query = "INSERT role_id, name INTO roles VALUES uuid_generate_v4(), @Name ; ";

                using NpgsqlCommand cmd = new(query, dbConnection.Connection);
                cmd.Parameters.AddWithValue("@Name", rolename);
                cmd.ExecuteNonQuery();
                return true;
               
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error :: adding roles failed: {ex.Message}");
                return false;
            }
            finally
            {
                dbConnection.CloseConnection();
            }

        }

        /// <summary>
        /// Checks if a role exists in the database.
        /// </summary>
        /// <param name="rolename">The name of the role to check.</param>
        /// <returns>True if the role exists, otherwise false.</returns>
        public bool RoleExists(string rolename)
        {
            bool roleExists = false;

            try
            {
                dbConnection.OpenConnection();
                string query = "SELECT COUNT(*) FROM roles WHERE name ILIKE @name";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, dbConnection.Connection))
                {
                    cmd.Parameters.AddWithValue("@name", rolename);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    roleExists = count > 0;
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error retrieving role: {ex.Message}");
            }
            finally
            {
                dbConnection.CloseConnection();
            }

            return roleExists;
        }

        /// <summary>
        /// Deletes a role from the database.
        /// </summary>
        /// <param name="rolename">The name of the role to be deleted.</param>
        public bool DeleteRole(string rolename)
        {
            try
            {
                dbConnection.OpenConnection();
                string query = "DELETE FROM roles WHERE name ILIKE @name";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, dbConnection.Connection))
                {
                    cmd.Parameters.AddWithValue("@name", rolename);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Error deleting role: {ex.Message}");
                return false;
            }
            finally
            {
                dbConnection.CloseConnection();
            }
        }



    }
}
