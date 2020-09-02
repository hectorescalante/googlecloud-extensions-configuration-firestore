using GoogleCloud.Extensions.Configuration.Firestore.Core.Helpers;
using System.Collections.Generic;
using System.Text.Json;

namespace GoogleCloud.Extensions.Configuration.Firestore.Core
{
  internal static class ApplicationSettingsExtensions
  {
    public static void SetData(this ApplicationSettingsDocument settingsDocument, string data)
    {
      if (string.IsNullOrEmpty(data)) data = "{}";
      settingsDocument.Data = JsonDocument.Parse(data).RootElement;
    }

    public static Dictionary<string, object> ToDictionary(this JsonElement jsonSettings)
    {
      var serializerOptions = new JsonSerializerOptions();
      serializerOptions.Converters.Add(new NestedObjectDictionaryConverter());
      return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonSettings.GetRawText(), serializerOptions);
    }

    public static string ToJson(this Dictionary<string, object> dictionarySettings)
    {
      var serializerOptions = new JsonSerializerOptions();
      serializerOptions.Converters.Add(new NestedObjectDictionaryConverter());
      return JsonSerializer.Serialize(dictionarySettings, serializerOptions);
    }
  }
}
