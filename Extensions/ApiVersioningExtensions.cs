
using Asp.Versioning;
using EcommerceApi.configurations;

namespace EcommerceApi.Extensions
{
    public static class ApiVersioningExtensions
    {
        public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
        {
            var apiVersioningBuilder = services.AddApiVersioning(config =>
            {
                //!La version por defecto es 1.0
                // config.DefaultApiVersion = new ApiVersion(2, 0);

                config.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new UrlSegmentApiVersionReader()
                );
            })
            .AddMvc()
            .AddApiExplorer(config =>
            {
                config.GroupNameFormat = "'v'VVV";
                config.SubstituteApiVersionInUrl = true;
            })
            .AddOpenApi(config =>
            {
                config.Document.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });


            return services;
        }
    }
}