using chopify.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

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
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

if (string.IsNullOrWhiteSpace(mongoConnectionString))
    throw new ArgumentNullException("MONGODB_CONNECTION_STRING enviroment is not properly configured.");

if (string.IsNullOrWhiteSpace(mongoDatabaseName))
    throw new ArgumentNullException("MONGODB_DATABASE_NAME enviroment is not properly configured.");

if (string.IsNullOrWhiteSpace(jwtSecretKey))
    throw new ArgumentNullException("JWT_SECRET_KEY enviroment is not properly configured.");

// Register IMongoClient using the connection string from the environment variable
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(mongoConnectionString));

// Register the database name in the settings (you can modify MongoDBSettings if necessary)
builder.Services.Configure<MongoDBSettings>(options =>
{
    options.DatabaseName = mongoDatabaseName;
});

// AutoMapper configuration
builder.Services.AddSingleton(AutoMapperConfig.GetInstance().CreateMapper());

// jwt tokens validation
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "chopify.com.ar",
        ValidAudience = "chopify.com.ar",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

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
