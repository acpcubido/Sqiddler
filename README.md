# Sqiddler
Use [Sqids](https://sqids.org/dotnet) in ASP.NET Core with attributes

## Getting Started

Install NuGet package `Sqiddler.AspNetCore`

```bash
dotnet add package Sqiddler.AspNetCore
```

Annotate your id properties with `[JsonSqid<TEntity>]` and id route parameters with `SqidParam<TEntity>` to serialize them to Sqid-strings.

```csharp
public class Person
{
  public int Id { get; set; }
}

public class PersonDto
{
  // Id gets serialized to Sqid-string
  [JsonSqid<Person>]
  public int Id { get; set; }

  public string OtherProperty { get; set; }
}

public class PersonEndpoints
{
  public void Map(WebApplication app)
  {
    app.MapGet("person/{id}", GetPerson);
  }

  public static async Task<Ok<MyDto>> GetPerson(ISender sender, SqidParam<Person> id)
  {
    return Task.FromResult(new PersonDto()
    {
      Id = 42,
      OtherProperty = "Hello, World!"
    }
  }
}

```

## Configuration

Configure Sqids in `Program.cs`
```csharp
var builder = WebApplication.CreateBuilder(args);

// configure SqidsOptions from appsettings.json
builder.Configuration.GetSection("Sqids").Bind(SqidsOptionsFactory.Default);

// optimize SqidsOptions.MinLength for TEntity
SqidsOptionsFactory.Configure<Person>(4); // enough for 100_000

var app = builder.Build();
app.Run();
```

Configure `Sqids.SqidsOptions` in `appsettings.json`
```json
"Sqids": {
  // generated from https://sqids.org/playground
  "Alphabet": "kBZDsjNeIWrQUSqExvwgdcnyl05X2F7f3pYVO9MaRhtLoP8GbiuzCm1T6KAJH4"
}
```

## Open API

Install NuGet package `Sqiddler.OpenApi`

```bash
dotnet add package Sqiddler.OpenApi
```

Register Open API in `Program.cs`
```csharp
var builder = WebApplication.CreateBuilder(args);
services.AddOpenApi(options =>
{
  // reflect Sqid-string changes in Open API
  options.AddSqids(options);
});
var app = builder.Build();
app.MapOpenApi();
app.Run();
```
