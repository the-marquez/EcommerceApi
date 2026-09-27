using System.Text;
using EcommerceApi.Constants;
using EcommerceApi.Data;
using EcommerceApi.Repositories.Contracts;
using EcommerceApi.Repositories.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connstr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>( (options) => options.UseSqlServer(connstr) );

//Repositories

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper( (config) => config.AddMaps(typeof(Program).Assembly) );

var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
var key = ( !string.IsNullOrEmpty(secretKey) ) 
                ? secretKey 
                : throw new InvalidOperationException("Secret Key No Configurada!");

builder.Services.AddAuthentication((options) =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer((options) =>
{
    options.RequireHttpsMetadata = false;  //desactiva https, en produccion debe ser true.
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new    SymmetricSecurityKey( Encoding.UTF8.GetBytes( key ) ), //validar firma del token
      ValidateIssuer = false,
      ValidateAudience = false //no se validara la audiencia, a menos que se quiera restringir a alguien
    };
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors((options) =>
{
    options.AddPolicy( PolicyNames.AllowAnyOrigin , config=>
    {
        config.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
} );

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

app.UseCors("AllowAnyOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
