using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BeaconTelemetryHub.Database.Context;
using BeaconTelemetryHub.Database.DataSink;
using BeaconTelemetryHub.DataContracts.DataStore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace BeaconTelemetryHub.Database
{
    public static class DatabaseServiceCollectionExtensions
    {
        public static readonly MySqlServerVersion MySqlServerVersion = new(new Version(11, 4, 9));
        private const string _mySqlPasswordFileKeyName = "MYSQL_PASSWORD";
        private const string _mySqlConnectionStringKeyName = "DefaultConnection";
        public static void RegisterDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(_mySqlConnectionStringKeyName) ?? throw new InvalidOperationException("Invalid connection string");
            var password = Environment.GetEnvironmentVariable(_mySqlPasswordFileKeyName) ?? 
                               throw new InvalidOperationException($"Cannot read password from enviroment, key name: '{_mySqlPasswordFileKeyName}'");
            var builder = new MySqlConnectionStringBuilder(connectionString)
            {
                Password = password
            };
            services.AddDbContextFactory<BeaconDbContext>(options =>
                                                            options.UseMySql(builder.ToString(),
                                                                             MySqlServerVersion,
                                                                             mySqlOptions =>
                                                                             {
                                                                                 mySqlOptions.EnableRetryOnFailure(
                                                                                              maxRetryCount: 2,
                                                                                              maxRetryDelay: TimeSpan.FromSeconds(5),
                                                                                              errorNumbersToAdd: null);
                                                                             }));
            services.AddSingleton<IBeaconDataSink, BeaconDatabaseDataSink>();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
        }
    }
}
