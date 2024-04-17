using Npgsql;

namespace inventoryMSLogic.src.DataAccessLayer
{
    /// <summary>
    /// Represents a connection to the PostgreSQL database.
    /// </summary>
    public class DatabaseConnection
    {
        private readonly string ConnectionString = "Host=localhost;Username=postgres;Password=12345;Database=inventoryms";
        /// <summary>
        /// Represents a database connection used for interacting with PostgreSQL database.
        /// </summary>
        public readonly NpgsqlConnection Connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnection"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor creates a new <see cref="NpgsqlConnection"/> object using the connection string and assigns it to the Connection property.
        /// </remarks>
        public DatabaseConnection()
        {
            Connection = new NpgsqlConnection(ConnectionString);
        }

        /// <summary>
        /// Opens a connection to the database.
        /// </summary>
        public void OpenConnection()
        {
            try
            {
                Connection.Open();
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Failed to connect to the database: {ex.Message}");
            }
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                Connection.Close();
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Failed to close to the database: {ex.Message}");
            }
        }

    }
}