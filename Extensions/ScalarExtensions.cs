
using Scalar.AspNetCore;

namespace EcommerceApi.Extensions
{
    public static class ScalarExtensions
    {
        public static WebApplication MapScalarDocumentation( this WebApplication app)
        {
            app.MapScalarApiReference(options =>
            {
                var descriptions = app.DescribeApiVersions();

                for (var i = 0; i < descriptions.Count; i++)
                {
                    var description = descriptions[i];

                    var isDefault = i == descriptions.Count - 1;

                    options.AddDocument(
                        description.GroupName,
                        description.GroupName,
                        isDefault: isDefault
                    );
                }

                options
                    .WithTitle("Ecommerce API")
                    .WithTheme(ScalarTheme.Default)
                    .WithDefaultHttpClient(
                        ScalarTarget.CSharp,
                        ScalarClient.HttpClient
                    );
            });

            return app;
        }
    }
}