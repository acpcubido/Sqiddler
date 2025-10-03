# Sqiddler

[![Nuget](https://img.shields.io/nuget/v/Sqiddler?style=flat-square)](https://www.nuget.org/packages/Sqiddler/)
[![License](https://img.shields.io/github/license/acpcubido/Sqiddler?style=flat-square)](https://github.com/acpcubido/Sqiddler/blob/main/LICENSE)
[![GitHub](https://img.shields.io/badge/-source-181717.svg?logo=GitHub)](https://github.com/acpcubido/Sqiddler)

Sqiddler applies [Sqids](https://sqids.org/dotnet) to serialize integer ids to obfuscated strings. It uses ASP.NET Core's built-in JSON serialization and model binding features to automatically convert between integers and Sqid-strings. It also shuffles the alphabet by entity to make it harder to mix ids.

## Getting Started

Install NuGet package `Sqiddler`

```bash
dotnet add package Sqiddler
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

Install NuGet package `Sqiddler.OpenApi` instead of `Sqiddler`

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
