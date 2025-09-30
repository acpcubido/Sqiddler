using Microsoft.AspNetCore.OpenApi;

namespace Sqiddler.OpenApi;

public static class OpenApiOptionsExtensions
{
    /// <summary>
    /// Registers the <see cref="SqidsOpenApiTransformer"/> transformer on the current <see cref="OpenApiOptions"/> instance.
    /// </summary>
    public static void AddSqids(this OpenApiOptions options)
    {
        options.AddOperationTransformer<SqidsOpenApiTransformer>();
        options.AddSchemaTransformer<SqidsOpenApiTransformer>();
        options.AddDocumentTransformer<SqidsOpenApiTransformer>();
    }
}
