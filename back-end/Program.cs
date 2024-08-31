using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var mongoClient = new MongoClient("mongodb://root:password@192.168.0.29:27017/");
var mongoDatabase = mongoClient.GetDatabase("chopify");

// Registrar la base de datos como servicio
builder.Services.AddSingleton(mongoDatabase);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();