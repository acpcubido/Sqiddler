using NJsonSchema.Generation;
using NSwag.Generation.AspNetCore;
using System.Collections.ObjectModel;

namespace Sqiddler.NSwag;

public static class AspNetCoreOpenApiDocumentGeneratorSettingsExtensions
{
    /// <summary>
    /// Adds the <see cref="SqidsNSwagProcessor"/>
    /// </summary>
    public static void AddSqids(this AspNetCoreOpenApiDocumentGeneratorSettings configure)
    {
        configure.OperationProcessors.Add(new SqidsNSwagProcessor());
        ((Collection<ISchemaProcessor>)configure.SchemaSettings.SchemaProcessors).Insert(0, new SqidsNSwagProcessor());
        configure.DocumentProcessors.Add(new SqidsNSwagProcessor());
    }
}
