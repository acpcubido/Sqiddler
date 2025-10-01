# Sqiddler

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-blue)](https://dotnet.microsoft.com/)

A .NET library that seamlessly integrates [Sqids](https://sqids.org/dotnet) with ASP.NET Core to automatically obfuscate integer IDs in your APIs. Sqiddler provides type-safe, entity-specific ID obfuscation that works with JSON serialization, model binding, and OpenAPI documentation.

## What is Sqiddler?

Sqiddler transforms your integer IDs into short, URL-safe, obfuscated strings while maintaining type safety and entity separation. Instead of exposing predictable sequential IDs like `/api/users/123`, you get URLs like `/api/users/jR8k2mN5`.

### Key Features

- **Automatic Conversion**: Seamlessly converts between integers and Sqid strings in JSON and route parameters
- **Type Safety**: Prevents ID mixing between different entities using generic type parameters
- **Entity-Specific Alphabets**: Each entity uses a shuffled alphabet, making it harder to mix IDs
- **OpenAPI Integration**: Automatically updates your API documentation to show string IDs
- **Zero Performance Impact**: Efficient caching and minimal overhead
- **ASP.NET Core Integration**: Works with built-in model binding and JSON serialization

## Installation

Sqiddler is distributed as multiple NuGet packages for different use cases:

### Core Package (Required)

```bash
dotnet add package Sqiddler.AspNetCore
```

### OpenAPI Support (Optional)

For ASP.NET Core's built-in OpenAPI:
```bash
dotnet add package Sqiddler.OpenApi
```

For NSwag:
```bash
dotnet add package Sqiddler.NSwag
```

## Quick Start

### 1. Define Your Entity

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

### 2. Create a DTO with Sqid Serialization

```csharp
public class UserDto
{
    // This property will be serialized as a Sqid string in JSON
    [JsonSqid<User>]
    public int Id { get; set; }
    
    public string Name { get; set; }
}
```

### 3. Use SqidParam in Your Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // The id parameter will automatically accept Sqid strings
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(SqidParam<User> id)
    {
        // Use id.Value to get the integer value
        var user = await GetUserFromDatabase(id.Value);
        
        return new UserDto
        {
            Id = user.Id,  // This will be serialized as a Sqid string
            Name = user.Name
        };
    }
}
```

### 4. The Result

**Request**: `GET /api/users/jR8k2mN5`
**Response**:
```json
{
    "id": "jR8k2mN5",
    "name": "John Doe"
}
```

The integer ID `123` is automatically converted to/from the Sqid string `jR8k2mN5`.

## Configuration

### Basic Configuration

Configure Sqids in your `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure global Sqids options from appsettings.json
builder.Configuration.GetSection("Sqids").Bind(SqidsOptionsFactory.Default);

// Optimize minimum length for specific entities
SqidsOptionsFactory.Configure<User>(4);     // Supports up to ~100,000 users
SqidsOptionsFactory.Configure<Product>(5);  // Supports up to ~10,000,000 products

var app = builder.Build();
```

### Configuration Options

Add to your `appsettings.json`:

```json
{
  "Sqids": {
    "Alphabet": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
    "MinLength": 6,
    "BlockList": ["word1", "word2"]
  }
}
```

**Configuration Options:**

- **`Alphabet`**: Custom alphabet for encoding (default: Sqids default alphabet)
- **`MinLength`**: Minimum length of generated IDs (default: 6)
- **`BlockList`**: Words to avoid in generated IDs

### Recommended MinLength Values

When using the default alphabet:
- **4**: Up to 100,000 IDs
- **5**: Up to 10,000,000 IDs  
- **6**: Up to 100,000,000 IDs (default)
- **7**: Up to `int.MaxValue`

## OpenAPI Integration

### ASP.NET Core OpenAPI

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    // Automatically updates OpenAPI schemas to show string IDs
    options.AddSqids();
});

var app = builder.Build();
app.MapOpenApi();
```

### NSwag

```csharp
services.AddOpenApiDocument(config =>
{
    // Updates NSwag schemas for Sqid properties
    config.AddSqids();
});
```

## Advanced Usage

### Multiple Entity Types

Each entity type gets its own shuffled alphabet:

```csharp
public class OrderDto
{
    [JsonSqid<Order>]
    public int Id { get; set; }
    
    [JsonSqid<User>]
    public int UserId { get; set; }
    
    [JsonSqid<Product>]
    public int ProductId { get; set; }
}
```

Even if the integer values are the same, different entity types will generate different Sqid strings, preventing ID confusion.

### Custom Value Types

Support for different integer types:

```csharp
public class EntityDto
{
    [JsonSqid<Entity>]
    public long Id { get; set; }  // Works with long, int, short, byte, etc.
}

// In controllers
[HttpGet("{id}")]
public ActionResult<EntityDto> Get(SqidParam<Entity, long> id)
{
    // id.Value is a long
}
```

### Arrays and Collections

Sqiddler automatically handles arrays and collections:

```csharp
public class BatchRequest
{
    [JsonSqid<User>]
    public int[] UserIds { get; set; }  // Each ID will be converted
}
```

## How It Works

1. **Entity-Specific Alphabets**: Each entity type gets a unique shuffled alphabet based on the type name
2. **Automatic Conversion**: JSON serialization and model binding automatically convert between integers and Sqid strings
3. **Type Safety**: Generic type parameters ensure you can't accidentally mix User IDs with Product IDs
4. **Caching**: Sqids encoders are cached per entity type for optimal performance
5. **OpenAPI Integration**: Schemas are automatically updated to reflect string IDs in documentation

## Troubleshooting

### Common Issues

**Invalid Sqid format errors**
- Ensure the Sqid string was generated with the same alphabet and configuration
- Check that you're using the correct entity type in `SqidParam<T>`

**IDs appearing as numbers instead of strings**
- Verify you've added the `[JsonSqid<T>]` attribute to DTO properties
- Ensure the property type matches the entity type

**Different Sqid strings for same integer**
- This is expected behavior - different entity types produce different Sqid strings
- Make sure you're using the same entity type consistently

### Performance Considerations

- Sqids encoders are cached per entity type
- Conversion overhead is minimal (microseconds)
- No database changes required - conversion happens at the API boundary

## Examples

See the `examples/` directory for complete sample applications demonstrating:
- Basic CRUD operations with Sqiddler
- Integration with Entity Framework
- OpenAPI documentation generation
- Advanced configuration scenarios

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

### Development Setup

```bash
git clone https://github.com/acpcubido/Sqiddler.git
cd Sqiddler
dotnet restore
dotnet build
dotnet test
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built on top of the excellent [Sqids](https://sqids.org/) library
- Inspired by the need for simple, type-safe ID obfuscation in ASP.NET Core APIs

