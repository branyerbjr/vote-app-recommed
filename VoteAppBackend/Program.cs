using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using VoteAppBackend.Data;
using VoteAppBackend.Services;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
// Define una política de CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
builder.Services.AddControllers();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5001") // URL del frontend Flask
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// Configurar PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Obtener la cadena de conexión de Redis desde la configuración o variables de entorno
var redisConnectionString = builder.Configuration.GetValue<string>("RedisConnection") ?? "localhost";

// Registrar StackExchange.Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

// Registrar el servicio de procesamiento de votos
builder.Services.AddHostedService<VoteProcessingService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Deshabilitar la redirección HTTPS en desarrollo
    app.Use((context, next) =>
    {
        context.Request.Scheme = "http";
        return next();
    });
}

// Habilitar CORS antes de otros middlewares que usan CORS (como Autorización)
app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
app.UseAuthorization();
app.MapControllers();
app.Run();
