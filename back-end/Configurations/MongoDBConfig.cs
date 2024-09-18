using MongoDB.Driver;

namespace chopify.Configurations
{
    public class MongoDBSettings
    {
        public string DatabaseName { get; set; } = string.Empty;
    }

    public static class MongoDbConfig
    {
        public static IServiceCollection ConfigureMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoConnectionString = configuration.GetValue<string>("MONGODB_CONNECTION_STRING");
            var mongoDatabaseName = configuration.GetValue<string>("MONGODB_DATABASE_NAME");

            if (string.IsNullOrWhiteSpace(mongoConnectionString))
                throw new ArgumentNullException(nameof(mongoConnectionString), "MongoDB connection string is not configured.");

            if (string.IsNullOrWhiteSpace(mongoDatabaseName))
                throw new ArgumentNullException(nameof(mongoDatabaseName), "MongoDB database name is not configured.");

            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));

            services.Configure<MongoDBSettings>(options =>
            {
                options.DatabaseName = mongoDatabaseName;
            });

            return services;
        }
    }
}
