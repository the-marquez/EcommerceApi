using System.Text;
using Asp.Versioning;
using EcommerceApi.configurations;
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
builder.Services.AddControllers((options) =>
{
    options.CacheProfiles.Add( CacheProfiles.ProfileName10s , CacheProfiles.ProfileConf10s );
    options.CacheProfiles.Add( CacheProfiles.ProfileName20s , CacheProfiles.ProfileConf20s );
} );

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options=> options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

var apiVersioningBuilder = builder.Services.AddApiVersioning((config) =>
{
    config.DefaultApiVersion = new ApiVersion(2, 0);
    config.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new UrlSegmentApiVersionReader() // Necesario para que funcione en la ruta /api/v1/
    );
}).AddMvc()
.AddApiExplorer((config) =>
{
    config.GroupNameFormat = "'v'VVV";
    config.SubstituteApiVersionInUrl = true;
});

apiVersioningBuilder.AddOpenApi();

builder.Services.AddCors((options) =>
{
    options.AddPolicy( PolicyNames.AllowAnyOrigin , config=>
    {
        config.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader();
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
    // Scalar: http://localhost:{port}/scalar
    app.MapScalarApiReference((options) =>
    {
        options.WithTitle("Ecommerce API");
        options.WithTheme( ScalarTheme.Default );
        options.WithDefaultHttpClient(
            ScalarTarget.CSharp,
            ScalarClient.HttpClient
        );
    } );
} 

// app.UseHttpsRedirection();

app.UseCors("AllowAnyOrigin");
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
