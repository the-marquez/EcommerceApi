
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace EcommerceApi.configurations
{
    internal sealed class BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider
        ) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document, 
            OpenApiDocumentTransformerContext context, 
            CancellationToken cancellationToken
            )
        {
            var authenticationSchemes = 
                    await authenticationSchemeProvider.GetAllSchemesAsync();

            var hasBearerScheme = authenticationSchemes.Any(
                authScheme => authScheme.Name == "Bearer"
            );

            if (!hasBearerScheme)
            {
                return;
            }

            document.Components ??= new OpenApiComponents();

            document.Components.SecuritySchemes ??=
                new Dictionary<string, IOpenApiSecurityScheme>();

            document.Components.SecuritySchemes["Bearer"] =
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT"
                };
        }
    }
}