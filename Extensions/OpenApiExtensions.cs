
using EcommerceApi.configurations;

namespace EcommerceApi.Extensions
{
    public static class OpenApiExtensions
    {
            //!Ya se integro esto en ApiVersioningExtensions.cs
        public static IServiceCollection AddOpenApiConfiguration( this IServiceCollection services)
        { 
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            return services;
        }
    }
}