using Xunit;
using Npgsql;

namespace inventoryMSUnitTest
{
    public class DatabaseTests
    {
        

        [Fact]
        public void DatabaseConnection_OpenConnection()
        {
            // Arrange
            string ConnectionString = "Host=localhost;Username=postgres;Password=12345;Database=inventoryms";
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                // Act
                try
                {
                    connection.Open();
                    connection.Close();
                }
                catch (NpgsqlException ex)
                {
                    // Assert
                    Assert.Fail($"Failed to connect to the database: {ex.Message}");
                }
            }

            // Assert
            Assert.True(true, "Database connection succeeded.");


        }

    }
}
