using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace GoogleCloud.Extensions.Configuration.Firestore
{
  public class FirestoreOptions
  {
    private readonly ILogger _logger;

    public FirestoreOptions(ILogger logger)
    {
      _logger = logger;
      _logger.LogInformation("Reading options from environment...");
      try
      {
        Enabled = bool.Parse(Environment.GetEnvironmentVariable("FIRESTORECONFIG_ENABLED") ?? "true");

        if (Enabled)
        {
          ProjectId = Environment.GetEnvironmentVariable("FIRESTORECONFIG_PROJECTID") ?? throw new ArgumentNullException("ProjectId");
          SettingsCollection = "ApplicationSettings";
          StagesCollection = "Stages";
          TagsCollection = "Tags";
          SettingsFileName = "appsettings.json";
          ApplicationName = Environment.GetEnvironmentVariable("FIRESTORECONFIG_APPLICATION") ?? AppDomain.CurrentDomain.FriendlyName;
          ReleaseStage = Environment.GetEnvironmentVariable("FIRESTORECONFIG_STAGE") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? throw new ArgumentNullException("Environment");
          Tag = Environment.GetEnvironmentVariable("FIRESTORECONFIG_TAG") ?? "Default";
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
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

    public string GetApplicationDocumentPath() => $"{SettingsCollection}/{ApplicationName}";
    public string GetStageDocumentPath() => $"{SettingsCollection}/{ApplicationName}/{StagesCollection}/{ReleaseStage}";
    public string GetTagDocumentPath() => $"{SettingsCollection}/{ApplicationName}/{StagesCollection}/{ReleaseStage}/{TagsCollection}/{Tag}";
  }
}