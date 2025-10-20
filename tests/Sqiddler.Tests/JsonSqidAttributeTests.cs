using System.Text.Json;

namespace Sqiddler.Tests;

public class JsonSqidAttributeTests
{
    public class SqidModelInt32 { [JsonSqid<SqidModelInt32>] public required int Id { get; set; } }
    public class SqidModelNullableInt32 { [JsonSqid<SqidModelNullableInt32>] public required int Id { get; set; } }
    public class SqidModelInt32Array { [JsonSqid<SqidModelInt32Array>] public required int[] Id { get; set; } }
    public class ModelString { public required string Id { get; set; } }
    public class ModelNullableString { public required string Id { get; set; } }
    public class ModelStringArray { public required string[] Id { get; set; } }

    [Fact]
    public void Should_Serialize_Int32()
    {
        // Arrange
        int id = 12345;
        var encoder = SqidsEncoderFactory.Create<SqidModelInt32, int>();
        var expected = JsonSerializer.Serialize(new ModelString { Id = encoder.Encode(id) });

        // Act
        var actual = JsonSerializer.Serialize(new SqidModelInt32 { Id = id });

        // Assert
        Console.WriteLine(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Deserialize_Int32()
    {
        // Arrange
        var expected = new SqidModelInt32
        {
            Id = 12345
        };
        using var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, expected);
        stream.Position = 0;

        // Act
        var actual = JsonSerializer.Deserialize<SqidModelInt32>(stream);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual.Id);
    }

    [Fact]
    public void Should_Serialize_NullableInt32()
    {
        // Arrange
        int id = 12345;
        var encoder = SqidsEncoderFactory.Create<SqidModelNullableInt32, int>();
        var expected = JsonSerializer.Serialize(new ModelNullableString
        {
            Id = encoder.Encode(id)
        });

        // Act
        var actual = JsonSerializer.Serialize(new SqidModelNullableInt32 { Id = id });

        // Assert
        Console.WriteLine(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Deserialize_NullableInt32()
    {
        // Arrange
        var expected = new SqidModelNullableInt32
        {
            Id = 12345
        };
        var json = JsonSerializer.Serialize(expected);

        // Act
        var actual = JsonSerializer.Deserialize<SqidModelNullableInt32>(json);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual.Id);
    }

    [Fact]
    public void Should_Serialize_Int32Array()
    {
        // Arrange
        int[] id = [12345, 23456, 34567];
        var encoder = SqidsEncoderFactory.Create<SqidModelInt32Array, int>();
        var expected = JsonSerializer.Serialize(new ModelString
        {
            Id = encoder.Encode(id)
        });

        // Act
        var actual = JsonSerializer.Serialize(new SqidModelInt32Array { Id = id });

        // Assert
        Console.WriteLine(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Should_Deserialize_Int32Array()
    {
        // Arrange
        var expected = new SqidModelInt32Array { Id = [12345, 23456, 34567] };
        var json = JsonSerializer.Serialize(expected);

        // Act
        var actual = JsonSerializer.Deserialize<SqidModelInt32Array>(json);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual.Id);
    }

    [Fact]
    public void Should_Serialize_With_Different_Seeds()
    {
        // Arrange
        int id = 12345;
        var encoder = SqidsEncoderFactory.Create<JsonSqidAttributeTests, int>();
        var expected = JsonSerializer.Serialize(new ModelString
        {
            Id = encoder.Encode(id)
        });

        // Act
        var actual = JsonSerializer.Serialize(new SqidModelInt32 { Id = id });

        // Assert
        Console.WriteLine(actual);
        Assert.NotEqual(expected, actual);
    }
}
