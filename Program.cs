using EcommerceApi.Data;
using EcommerceApi.Repositories.Contracts;
using EcommerceApi.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connstr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>( (options) => options.UseSqlServer(connstr) );

//Repositories

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddAutoMapper( (config) => config.AddMaps(typeof(Program).Assembly) );

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Scalar: http://localhost:{port}/scalar
    app.MapScalarApiReference((options) =>
    {
        options.WithTitle("Ecommerce API");
        options.WithTheme( ScalarTheme.Alternate );
        options.WithDefaultHttpClient(
            ScalarTarget.CSharp,
            ScalarClient.HttpClient
        );
    } );
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
