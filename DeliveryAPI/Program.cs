using DeliveryAPI.Data;
using DeliveryAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//DB context
builder.Services.AddDbContext<PiaAppMovContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DeliveryDBConnection"))
);

//Services
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<NegocioService>();
builder.Services.AddScoped<CarritoService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
