using chopify.Configurations;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Custom dependency injection
builder.Services.DependencyInjection();

// Get MongoDB connection settings from environment variables
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
var mongoDatabaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");

if (string.IsNullOrEmpty(mongoConnectionString))
    throw new InvalidOperationException("MongoDB connection string is not properly configured.");

if (string.IsNullOrEmpty(mongoDatabaseName))
    throw new InvalidOperationException("MongoDB database name is not properly configured.");

// Register IMongoClient using the connection string from the environment variable
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(mongoConnectionString));

// Register the database name in the settings (you can modify MongoDBSettings if necessary)
builder.Services.Configure<MongoDBSettings>(options =>
{
    options.DatabaseName = mongoDatabaseName;
});

// AutoMapper configuration
builder.Services.AddSingleton(AutoMapperConfig.GetInstance().CreateMapper());

// Add controllers and additional services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
