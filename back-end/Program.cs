using chopify.Configurations;
using chopify.External;
using chopify.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCorsPolicy();
builder.Services.DependencyInjection();
builder.Services.ConfigureMongoDb(builder.Configuration);
builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.AddSingleton(AutoMapperConfig.GetInstance().CreateMapper());
builder.Services.AddSingleton(SpotifyService.Instance);
builder.Services.AddSingleton<Scheduler>();
builder.Services.AddSingleton<GarbashCollectorsConfig>();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Services.GetRequiredService<GarbashCollectorsConfig>();

app.Run();
