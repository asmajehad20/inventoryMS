using Npgsql;

namespace inventoryMSLogic.src.DataAccessLayer
{
    /// <summary>
    /// Represents a connection to the PostgreSQL database.
    /// </summary>
    class DatabaseConnection
    {
        private readonly string ConnectionString = "Host=localhost;Username=postgres;Password=12345;Database=inventoryms";
        public readonly NpgsqlConnection Connection;

        // Constructor
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
                Console.WriteLine($"Error while opening connection: {ex.Message}");
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
                Console.WriteLine($"Error while closing connection: {ex.Message}");
            }
        }

    }
}
