using System.Text.Json.Serialization;

namespace MovieManager.Common.Features.Common;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Settings))]
[JsonSerializable(typeof(CommonSettings))]
[JsonSerializable(typeof(ImportSettings))]
internal partial class SettingsJsonContext : JsonSerializerContext { }