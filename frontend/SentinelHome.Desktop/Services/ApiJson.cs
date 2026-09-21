using System.Text.Json;
using System.Text.Json.Serialization;

namespace SentinelHome.Desktop.Services;

internal static class ApiJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
}
