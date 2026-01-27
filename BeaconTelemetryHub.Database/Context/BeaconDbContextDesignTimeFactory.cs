using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace BeaconTelemetryHub.Database.Context
{
    /// <summary>
    /// Factory for creating the <see cref="BeaconDbContext"/> at design time.
    /// This allows Entity Framework Core tools in PM Console (like Add-Migration, Database-Update)
    /// to instantiate the context without running the actual application.
    /// </summary>
    internal class BeaconDbContextDesignTimeFactory : IDesignTimeDbContextFactory<BeaconDbContext>
    {
        private const string _connectionStringVarriableName = "MY_SQL_CONNECTION";
        // Using 'Console' instead of 'Serilog' (Serilog not exist in PMC)
        public BeaconDbContext CreateDbContext(string[] args)
        {
            _ = args;
            string? connectionString = Environment.GetEnvironmentVariable(_connectionStringVarriableName);
            if(connectionString == null)
            {
                string exceptionMessage = "Cannot get connection string from environment variables";
                Console.WriteLine(exceptionMessage);
                throw new InvalidOperationException(exceptionMessage);
            }
            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
            }
            catch (Exception ex)
            {
                string message = "Invalid connection string";
                Console.WriteLine(message + $", exception message: {ex.Message}");
                throw new InvalidOperationException(message, ex);
            }
            var optionsBuilder = new DbContextOptionsBuilder<BeaconDbContext>();
            optionsBuilder.UseMySql(connectionString, DatabaseServiceCollectionExtensions.MySqlServerVersion);
            return new BeaconDbContext(optionsBuilder.Options);
        }
    }
}
