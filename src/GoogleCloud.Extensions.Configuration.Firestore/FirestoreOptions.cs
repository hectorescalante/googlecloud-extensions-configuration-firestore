using GoogleCloud.Extensions.Configuration.Firestore.Core.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace GoogleCloud.Extensions.Configuration.Firestore
{
  public class FirestoreOptions
  {
    private readonly ILogger _logger;

    public FirestoreOptions() { }

    public FirestoreOptions(ILogger logger)
    {
      _logger = logger;
      _logger.LogInformation("Reading options from environment...");
      try
      {
        if (this.IsEnabled())
          this.Bind();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error reading environment variables");
        Enabled = false;
      }
      _logger.LogDebug($"FirestoreOptions: {JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true })}");
    }

    public bool Enabled { get; set; }
    public string ProjectId { get; set; }
    public string SettingsCollection { get; set; }
    public string StagesCollection { get; set; }
    public string TagsCollection { get; set; }
    public string ApplicationName { get; set; }
    public string SettingsFileName { get; set; }
    public string ReleaseStage { get; set; }
    public string Tag { get; set; }
  }
}