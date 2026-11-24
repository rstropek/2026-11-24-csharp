using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

// The serializer code for these types is generated at compile time: no
// reflection at runtime, trimming- and Native-AOT-friendly. The options live
// here as an attribute, so StorageContext.Default is preconfigured with them.
// camelCase like the portal, and strict: a submission with a missing or
// null required field is rejected instead of turning into half a record.
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web,
    WriteIndented = true,
    RespectRequiredConstructorParameters = true,
    RespectNullableAnnotations = true)]
[JsonSerializable(typeof(PortalRequest))]
internal partial class StorageContext : JsonSerializerContext;

public static class Storage
{
    // Callers pass the generated metadata, e.g. StorageContext.Default.PortalRequest.
    public static T Read<T>(string path, JsonTypeInfo<T> typeInfo) =>
        JsonSerializer.Deserialize(File.ReadAllText(path), typeInfo)
        ?? throw new JsonException($"{path} is empty");

    public static void Write<T>(string path, T value, JsonTypeInfo<T> typeInfo) =>
        File.WriteAllText(path, JsonSerializer.Serialize(value, typeInfo));
}
