using System.Text.Json;

namespace GoogleCloud.Extensions.Configuration.Firestore
{
  public class ApplicationSettingsDocument
  {
    public JsonElement Data { get; set; }
  }
}
