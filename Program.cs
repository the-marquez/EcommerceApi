using EcommerceApi.Extensions;
using EcommerceApi.Constants;
using EcommerceApi.Data;
using EcommerceApi.Repositories.Contracts;
using EcommerceApi.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Identity;
using EcommerceApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connstr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>( (options) => options.UseSqlServer(connstr) );

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper( (config) => config.AddMaps(typeof(Program).Assembly) );

//Integración de Identity con Entity Framework Core
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddJwtAuthentication( builder.Configuration );

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers((options) =>
{
    options.CacheProfiles.Add( CacheProfiles.ProfileName10s , CacheProfiles.ProfileConf10s );
    options.CacheProfiles.Add( CacheProfiles.ProfileName20s , CacheProfiles.ProfileConf20s );
} );

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddApiVersioningConfiguration(); //extension

builder.Services.AddCors((options) =>
{
    options.AddPolicy( PolicyNames.AllowAnyOrigin , config=>
    {
        config.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    });
} );

builder.Services.AddResponseCaching((options) =>
{
    options.UseCaseSensitivePaths = true;
    options.MaximumBodySize = 1024 * 1024; //1MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().WithDocumentPerVersion();
    app.MapScalarDocumentation(); //extension
} 

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowAnyOrigin");
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
